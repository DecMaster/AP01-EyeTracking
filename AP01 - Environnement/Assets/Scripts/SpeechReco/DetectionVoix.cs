using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class DetectionVoix : MonoBehaviour
{
    // Singleton
    public static DetectionVoix instance = null;

    // Proprietes
    public string[] keywords = new string[] { "rouge", "vert", "bleu", "jaune" };
    public ConfidenceLevel confidence = ConfidenceLevel.Medium;
    [HideInInspector]
    public KeywordRecognizer recognizer;

    // Initiation du Singleton
    private void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    // Initialisation de la reconnaissance vocale
    public void Init()
    {
        recognizer = new KeywordRecognizer(keywords, confidence);
        recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
        recognizer.Start();
    }

    // Méthode appellée lorsque l'un des keywords est reconnu
    private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log("Mot détecté : " + args.text);
    }

    // On arrête le recognizer lorsque l'on quitte l'application
    private void OnApplicationQuit()
    {
        if (recognizer != null && recognizer.IsRunning)
        {
            recognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
            recognizer.Stop();
        }
    }
}
