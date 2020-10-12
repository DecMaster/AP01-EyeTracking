using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii;
using Tobii.G2OM;

public abstract class GazeButton : MonoBehaviour, IGazeFocusable
{
    // PROPRIETES PRIVEES
    private bool isFocused = false;

    [Header("Survol")]
    // SURVOL
    // MODIF
        [Range(0.1f, 1f)]
        public float IntensiteAnimation = 0.2f;
        [Range(1f, 5f)]
        public float vitesseSurvol = 4f;
        // TAILLES
        private Vector3 tailleObjetInitial;
        private Vector3 tailleObjetFinale;
        // DONNEES
        private float delta;
        // BOOLEENS
        private bool init_ok = false;
        private bool survol_on = false;
        private bool survol_off = false;

    [Header("Remplissage")]
    // REMPLISSAGE
        // OBJETS
        private GameObject bouton;
        private GameObject frontplate;
        private GameObject objetRemplissage;
        // BOOLEENS
        private bool latence_ok = false; // Booleen permettant de rajouter un temps de latence avant de commencer le remplissage lorsque l'on focus un objet
        private bool instance_ok = false; // Booleen permettant de savoir quand l'instance objetRemplissage est prete
        // COMPTEURS
        private float timer = 0;
        private float timerLatence = 0;
        // TEMPS LIMITES
        [Range(0f, 5f)]
        public float tempsRemplissage = 1.0f;
        [Range(0f, 5f)]
        public float tempsLatence = 0.5f;

    // METHODES UNITY
    public void StartRemplissage()
    {
        if (frontplate == null)
        {
            frontplate = this.gameObject;
        }
        if(bouton == null)
        {
            bouton = this.transform.parent.gameObject;
        }
    }

    public void UpdateRemplissage()
    {
        // LATENCE
        // Si on focus le bouton + que l'instance est prete
        if (isFocused && instance_ok && !latence_ok)
        {
            timerLatence += Time.deltaTime;
            if (timerLatence >= tempsLatence)
            {
                latence_ok = true;      // Le remplissage peut commencer une fois la latence finie
            }
        }

        // REMPLISSAGE
        if (latence_ok) // Si le temps de latence arrive à son terme
        {
            timer += Time.deltaTime;
            if (objetRemplissage != null)
            {
                objetRemplissage.transform.localScale += frontplate.transform.localScale / (tempsRemplissage * 60); // Il faudra tempsFocusLimite * 60 frame pour atteindre la taille
            }

            if (ComparaisonTailles()) // Si la taille de l'effet de remplissage atteint/d�passe celle de l'objet d'origine (temps OK)
            {
                Click();
                timer = 0;
                timerLatence = 0;
                latence_ok = false;
                isFocused = false;
            }
        }

        // SURVOL
        if (survol_on)
        {
            // 1 : INCREMENTATION DELTA
            delta += Time.deltaTime * vitesseSurvol;

            // 2 : RETRECISSEMENT BOUTONS
            bouton.transform.localScale = Vector3.Lerp(tailleObjetInitial, tailleObjetFinale, delta);

            // 4 : CONDITION ARRET : Delta atteint 1
            if (delta >= 1)
            {
                survol_on = false;
                bouton.transform.localScale = tailleObjetFinale;
            }
        }

        // BOUTON RETRECIT LEGEREMENT
        if (survol_off)
        {
            // 1 : DECREMENTATION DELTA
            delta -= Time.deltaTime * vitesseSurvol;

            // 2 : RETRECISSEMENT BOUTONS
            bouton.transform.localScale = Vector3.Lerp(tailleObjetInitial, tailleObjetFinale, delta);

            // 3 : CONDITION ARRET : Delta atteint 0
            if (delta <= 0)
            {
                survol_off = false;
                bouton.transform.localScale = tailleObjetInitial;
            }
        }
    }

    // METHODES REMPLISSAGE
    public bool ComparaisonTailles()
    {
        if (objetRemplissage != null)
        {
            if (objetRemplissage.transform.localScale.x >= frontplate.transform.localScale.x)
                return true;
            if (objetRemplissage.transform.localScale.y >= frontplate.transform.localScale.y)
                return true;
            if (objetRemplissage.transform.localScale.z >= frontplate.transform.localScale.z)
                return true;
        }

        return false;
    }

    public void RemplissageON()
    {
        // INSTANCIEMENT DE L'EFFET DE REMPLISSAGE
        if(frontplate != null)
        {
            objetRemplissage = Instantiate(frontplate, frontplate.transform.position, frontplate.transform.rotation, this.transform.parent);
            objetRemplissage.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/RemplissageMat");
            objetRemplissage.transform.localScale = Vector3.zero;
            instance_ok = true;
            isFocused = true;
        }
    }

    public void RemplissageOFF()
    {
        if (objetRemplissage != null)
        {
            Destroy(objetRemplissage); // On detruit l'effet de remplissage
        }

        // On remet le focus à false
        isFocused = false;
        latence_ok = false;
        instance_ok = false;

        // On reinitialise les timers
        timer = 0;
        timerLatence = 0;
    }

    // METHODES SURVOL

    public void SurvolON()
    {
        if (init_ok == false && bouton != null)
        {
            // RECUP DONNEES ANIMATION SURVOL
            init_ok = true;
            tailleObjetInitial = bouton.transform.localScale;
            tailleObjetFinale = tailleObjetInitial + tailleObjetInitial * IntensiteAnimation;
        }

        // ACTIVATION ANIMATION
        survol_on = true;
    }

    public void SurvolOFF()
    {
        survol_off = true;
        survol_on = false;
    }

    // METHODES TOBII
    public void GazeFocusChanged(bool hasFocus)
    {
        if (hasFocus)
        {
            RemplissageON();
            SurvolON();
        }
        else
        {
            RemplissageOFF();
            SurvolOFF();
        }
    }

    // METHODES INPUT
    public abstract void Click();
}
