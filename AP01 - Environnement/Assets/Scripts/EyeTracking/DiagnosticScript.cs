using System.Collections;
using System.Collections.Generic;
using Tobii.XR;
using UnityEngine;

public class DiagnosticScript : MonoBehaviour
{

    // PROPRIETES
    [Header("Activation")]
    public bool BlinkDetection = true;

    [Header("Blink Detection Parameters")]
    [Range(10, 500)]
    public int lowerBlinkTime = 300;
    [Range(10, 500)]
    public int upperBlinkTime = 400;

    // PROPRIETES PRIVATES
    private int blinkCounter = 0;
    private int blinkTimer = 0;
    private bool isBlinking = false;

    // METHODES
    void Start()
    {
        if(lowerBlinkTime >= upperBlinkTime)
        {
            Debug.LogError("ATTENTION : Le temps de clignotement est incorrect, veuillez le modifier");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Récupération des données de l'EyeTracking
        var eyeTrackingData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.Local);

        // 1 - Blink Detection
        if (BlinkDetection)
        {
            // SI ON FERME LES YEUX
            if (eyeTrackingData.IsLeftEyeBlinking || eyeTrackingData.IsRightEyeBlinking)
            {
                blinkTimer++;
                isBlinking = true;
            }

            else
            {
                // QUAND ON ROUVRE LES YEUX
                if(isBlinking == true)
                {
                    isBlinking = false;
                    if(blinkTimer > lowerBlinkTime && blinkTimer < upperBlinkTime)
                    {
                        blinkCounter++;
                        Debug.Log("Nouveau Blink détecté ! ");
                        GetComponent<AudioSource>().Play();
                    }
                    blinkTimer = 0;
                }
            }
        }

        // 2 - Détection des saccades

        // 3 - Détection de la taille de la pupille
    }
}
