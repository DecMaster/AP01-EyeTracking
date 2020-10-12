using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GazeButtonScene : GazeButton
{
    // ENUM
    public enum NomScene {
        Main = 0,
        ProjetCub,
        CrazyColors
    }

    // PROPRIETES
    public NomScene nomScene;
    private TextMeshPro texte;

    // METHODES
    void Start()
    {
        if(texte == null)
        {
            texte = this.transform.parent.Find("Text").GetComponent<TextMeshPro>();
        }
        texte.text = nomScene.ToString();

        StartRemplissage();
    }

    void Update()
    {
        UpdateRemplissage();
    }

    public override void Click()
    {
        Debug.Log("Chargement de la scène : " + nomScene.ToString());
        //SceneManager.LoadScene(nomScene.ToString());
    }
}
