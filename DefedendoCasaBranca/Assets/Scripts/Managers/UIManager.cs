using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    [Header("Panels")]
    [SerializeField] private GameObject turretShopPanel;
    [SerializeField] private GameObject nodeUIPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI upgradeText;
    [SerializeField] private TextMeshProUGUI sellText;
    [SerializeField] private TextMeshProUGUI turretLevelText;
    [SerializeField] private TextMeshProUGUI totalCoinsText;
    [SerializeField] private TextMeshProUGUI lifesText;
    [SerializeField] private TextMeshProUGUI currentWaveText;
    [SerializeField] private TextMeshProUGUI gameOverTotalCoinsText;

    private Node _currentNodeSelected;

    private void Update()
    {
        totalCoinsText.text = CurrencySystem.Instance.TotalCoins.ToString();
        lifesText.text = LevelManager.Instance.TotalLives.ToString();
        currentWaveText.text = $"Wave {LevelManager.Instance.CurrentWave}";
    }


    public void CloseTurretShopPanel()
    {
        turretShopPanel.SetActive(false);
    }

    public void CloseNodeUIPanel()
    {
        _currentNodeSelected.CloseAttackRangeSprite();
        nodeUIPanel.SetActive(false);
    }

    public void ShowGameOverPanel(){
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
        gameOverTotalCoinsText.text = CurrencySystem.Instance.TotalCoins.ToString();
    }

    public void RestartGame(){
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CloseGameOver(){
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }

    public void UpgradeTurret()
    {
        _currentNodeSelected.Turret.TurretUpgrade.UpgradeTurret();
        UpdateUpgradeText();
        UpdateTurretLevel();
        UpdateSellValue();
    }

    public void SellTurret()
    {
        _currentNodeSelected.SellTurret();
        _currentNodeSelected = null;
        nodeUIPanel.SetActive(false);
    }

    private void ShowNodeUI()
    {
        nodeUIPanel.SetActive(true);
        UpdateUpgradeText();
        UpdateTurretLevel();
        UpdateSellValue();
    }

    private void UpdateUpgradeText()
    {
        upgradeText.text = _currentNodeSelected.Turret.TurretUpgrade.UpgradeCost.ToString();
    }

    private void UpdateTurretLevel()
    {
        turretLevelText.text = $"Level {_currentNodeSelected.Turret.TurretUpgrade.Level}";
    }

    private void UpdateSellValue()
    {
        int sellAmount = _currentNodeSelected.Turret.TurretUpgrade.GetSellValue();
        sellText.text = sellAmount.ToString();
    }


    private void NodeSelected(Node nodeSelected)
    {
        _currentNodeSelected = nodeSelected;
        if (_currentNodeSelected.IsEmpty())
        {
            turretShopPanel.SetActive(true);
        }
        else
        {
            ShowNodeUI();
        }
    }

    private void OnEnable()
    {
        Node.OnNodeSelected += NodeSelected;
    }

    private void OnDisable()
    {
        Node.OnNodeSelected -= NodeSelected;
    }
}
