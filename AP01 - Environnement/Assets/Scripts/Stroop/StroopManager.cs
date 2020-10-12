using System.Collections;
using System.Collections.Generic;
using TMPro;
using Tobii.XR.Examples;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class StroopManager : MonoBehaviour
{
    // ENUM
    enum Sounds
    {
        correct = 0,
        incorrect,
        clic
    }

    // REFERENCES
    [Header("Références Objets")]
    public GameObject ExplainationWindow;
    public GameObject SpeechCheckWindow;
    public GameObject StroopTestWindow;
    public Text motRougeSpeech;
    public Text motVertSpeech;
    public Text motBleuSpeech;
    public Text motJauneSpeech;
    public GameObject BoutonOKSpeech;
    public Text motStroop;
    public Image correctMark;
    public Image incorrectMark;

    [Header("Paramètres")]
    public string[] mots = new string[] { "ROUGE", "VERT", "BLEU", "JAUNE" };
    [HideInInspector]
    public List<Color> couleurs = new List<Color>();
    public Color rouge = Color.red;
    public Color vert = Color.green;
    public Color bleu = Color.blue;
    public Color jaune = Color.yellow;
    [Range(1,5)]
    public float tempsLimite = 3f;
    [Range(1, 3)]
    public float tempsEntreMots = 1f;

    // PROPRIETES PRIVATE
    private AudioSource audioSource;
    private bool stroopStarted = false;
    private GameObject currentWindow;

    // SPEECH DETECTION
    private bool speechEnabled = false;
    private bool redOK = false;
    private bool greenOK = false;
    private bool blueOK = false;
    private bool yellowOK = false;

    // STROOP
    private float timer;
    private int countdown = 3;
    private string currentColor;

    // METHODES UNITY

    private void Start()
    {
        // GET REFERENCES
        if(ExplainationWindow == null)
        {
            ExplainationWindow = GameObject.Find("ExplainationWindow");
        }
        if (SpeechCheckWindow == null)
        {
            SpeechCheckWindow = GameObject.Find("SpeechCheckWindow");
        }
        audioSource = GetComponent<AudioSource>();

        // REMPLISSAGE LISTE
        couleurs.Add(rouge);
        couleurs.Add(vert);
        couleurs.Add(bleu);
        couleurs.Add(jaune);

        // RESET STROOP
        motStroop.text = "";
        correctMark.enabled = false;
        incorrectMark.enabled = false;

        // APPEL PREMIERE FENETRE
        ResetWindows();
        CallExplainationWindow();
    }

    private void Update()
    {
        if (stroopStarted)
        {
            timer += Time.deltaTime;
            if(timer >= tempsLimite)
            {
                timer = 0;
                stroopStarted = false;
                Incorrect();
            }
        }
    }

    public void StopStroop()
    {
        stroopStarted = false;
    }

    public void Countdown()
    {
        if (countdown == 0)
        {
            stroopStarted = true;
            NewWord();
        }
        else
        {
            motStroop.text = countdown.ToString();
            motStroop.color = Color.white;
            countdown--;
            Invoke("Countdown", 1);
            PlaySound(Sounds.clic);
        }
    }

    private void NewWord()
    {
        // INIT
        int i = -1;
        int j = -1;
        timer = 0f;
        stroopStarted = true;
        string newWord = string.Empty;
        Color newColor = Color.white;
        correctMark.enabled = false;
        incorrectMark.enabled = false;

        // CHOIX MOT & COULEUR
        do
        {
            i = Random.Range(0, 4);
            j = Random.Range(0, 4);
        }
        while (newWord == mots[i] || newColor == couleurs[j]);
        newWord = mots[i];
        newColor = couleurs[j];

        // SETUP COULEUR
        if(newColor == rouge)
        {
            currentColor = "rouge";
        }
        if (newColor == bleu)
        {
            currentColor = "bleu";
        }
        if (newColor == vert)
        {
            currentColor = "vert";
        }
        if (newColor == jaune)
        {
            currentColor = "jaune";
        }

        // AFFICHAGE
        motStroop.text = newWord;
        motStroop.color = newColor;
    }

    private void Correct()
    {
        stroopStarted = false;
        correctMark.enabled = true;
        PlaySound(Sounds.correct);
        Invoke("NewWord", tempsEntreMots);
    }

    private void Incorrect()
    {
        stroopStarted = false;
        incorrectMark.enabled = true;
        PlaySound(Sounds.incorrect);
        Invoke("NewWord", tempsEntreMots);
    }

    private void SpeechCheck(PhraseRecognizedEventArgs args)
    {
        if(!redOK && args.text == "rouge")
        {
            motRougeSpeech.color = rouge;
            PlaySound(Sounds.correct);
            redOK = true;
        }
        if (!greenOK && args.text == "vert")
        {
            motVertSpeech.color = vert;
            PlaySound(Sounds.correct);
            greenOK = true;
        }
        if (!blueOK && args.text == "bleu")
        {
            motBleuSpeech.color = bleu;
            PlaySound(Sounds.correct);
            blueOK = true;
        }
        if (!yellowOK && args.text == "jaune")
        {
            motJauneSpeech.color = jaune;
            PlaySound(Sounds.correct);
            yellowOK = true;
        }

        // ACTIVATION BOUTON OK
        if(!stroopStarted && redOK && greenOK && blueOK && yellowOK)
        {
            ColorUtility.TryParseHtmlString("008DBF", out Color c);
            BoutonOKSpeech.GetComponent<Image>().color = c;
            BoutonOKSpeech.GetComponent<UIFixationGazeButton>().enabled = true;
        }

        // STROOP
        if (stroopStarted)
        {
            if (currentColor == args.text)
            {
                Correct();
            }
            else
            {
                Incorrect();
            }
        }
    }

    private void PlaySound(Sounds sound)
    {
        string chemin = "Sounds/" + sound.ToString();
        audioSource.clip = Resources.Load(chemin, typeof(AudioClip)) as AudioClip;
        audioSource.Play();
    }

    // METHODES UI
    public void ResetWindows()
    {
        ExplainationWindow.SetActive(false);
        SpeechCheckWindow.SetActive(false);
        StroopTestWindow.SetActive(false);
    }

    // Appelle la fenêtre d'explication
    public void CallExplainationWindow()
    {
        if(currentWindow != null)
        {
            currentWindow.SetActive(false);
        }
        currentWindow = ExplainationWindow;
        currentWindow.SetActive(true);
    }

    // Appelle la fenêtre de teste de la reconnaissance vocale
    public void CallSpeechCheck()
    {
        if (currentWindow != null)
        {
            currentWindow.SetActive(false);
        }
        currentWindow = SpeechCheckWindow;
        currentWindow.SetActive(true);
        DetectionVoix.instance.Init();
        DetectionVoix.instance.recognizer.OnPhraseRecognized += SpeechCheck;
        speechEnabled = true;

        motRougeSpeech.color = Color.gray;
        motVertSpeech.color = Color.gray;
        motBleuSpeech.color = Color.gray;
        motJauneSpeech.color = Color.gray;
        BoutonOKSpeech.GetComponent<Image>().color = Color.gray;
        BoutonOKSpeech.GetComponent<UIFixationGazeButton>().enabled = false;
    }

    public void CallStroopTestWindow()
    {
        if (currentWindow != null)
        {
            currentWindow.SetActive(false);
        }
        if (!speechEnabled)
        {
            speechEnabled = true;
            DetectionVoix.instance.Init();
            DetectionVoix.instance.recognizer.OnPhraseRecognized += SpeechCheck;
        }
        currentWindow = StroopTestWindow;
        currentWindow.SetActive(true);
        Countdown();
    }
}
