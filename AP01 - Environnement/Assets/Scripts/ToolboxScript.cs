using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class ToolboxScript : MonoBehaviour
{
    // SINGLETON
    public static ToolboxScript Toolbox = null;

    // ENUM
    public enum Sons
    {
        Clic = 0
    }

    public enum Scenes
    {
        Main,
        Stroop
    }

    // PROPRIETES
    [Header("Références")]
    private GameObject gazeVisualizer;
    private AudioSource audioSource;

    // PARAMETRES
    [Header("Paramètres EyeTracking")]
    public bool afficherGazeVisualiser = false;

    // METHODES
    void Awake()
    {
        // CONFIG SINGLETON
        if (Toolbox == null)
            Toolbox = this;
        else if (Toolbox != this)
            Destroy(gameObject);

        // GET REFERENCES
        if(audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        if(gazeVisualizer == null)
        {
            gazeVisualizer = GameObject.Find("GazeVisualizer");
        }

        // PARAMETRAGE
        if (!afficherGazeVisualiser)
        {
            gazeVisualizer.SetActive(false);
        }
    }

    void Update()
    {

    }

    public void PlaySound(Sons SoundToPlay)
    {
        string chemin = "Sounds/" + SoundToPlay.ToString();
        audioSource.clip = Resources.Load(chemin, typeof(AudioClip)) as AudioClip;
        audioSource.Play();
    }

    public void LoadScene(Scenes SceneToLoad)
    {
        try
        {
            SceneManager.LoadScene(SceneToLoad.ToString());
        }
        catch
        {
            Debug.LogError("Impossible de charger la scène " + SceneToLoad.ToString() + ", l'avez-vous bien ajouté au projet ?");
        }
    }

    // CHARGEMENT DES SCENES
    public void ChargerStroopTest()
    {
        LoadScene(Scenes.Stroop);
    }
}
