using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    /**
     * Muda scene de acordo com string passada
     * scene -> nome da cena a ser ativada
     */
    public void mudaScene(string scene) {
        SceneManager.LoadScene(scene);
    }

    /**
     * Muda scene ativa de volta para homepage (Menu) 
     */
    public void mudaSceneHome()
    {
        SceneManager.LoadScene(0);
    }

}
