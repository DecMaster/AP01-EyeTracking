// Copyright © 2018 – Property of Tobii AB (publ) - All Rights Reserved

using Tobii.G2OM;
using UnityEngine;
using UnityEngine.UI;

namespace Tobii.XR.Examples
{
    /// <summary>
    /// A gaze aware button that is interacted with the touchpad button on the Vive controller.
    /// </summary>
    [RequireComponent(typeof(UIGazeButtonGraphics))]
    public class UIFixationGazeButton : MonoBehaviour, IGazeFocusable
    {

        // Durée avant la confirmation
        [Range(1,3)]
        public float DureeAvantConfirmation = 1f;
        [Range(0.5f, 2)]
        public float Latence = 0.5f;

        // Event called when the button is clicked.
        public UIButtonEvent OnButtonClicked;

        // Effet de remplissage progressif
        public Image Bouton;

        // Effet de remplissage progressif
        public Image Remplissage;

        // The touchpad button on the Vive controller.
        private const ControllerButton TouchpadButton = ControllerButton.Touchpad;

        // Haptic strength for the button click.
        private const ushort HapticStrength = 1000;

        // The state the button is currently  in.
        private ButtonState _currentButtonState = ButtonState.Idle;

        // Private fields.
        private float delta = 0f;
        private bool _hasFocus;
        private UIGazeButtonGraphics _uiGazeButtonGraphics;

        private void Start()
        {
            // Store the graphics class.
            _uiGazeButtonGraphics = GetComponent<UIGazeButtonGraphics>();

            // Initialize click event.
            if (OnButtonClicked == null)
            {
                OnButtonClicked = new UIButtonEvent();
            }
            if(Remplissage == null)
            {
                Remplissage = transform.Find("Remplissage").GetComponent<Image>();
            }
            if(Bouton == null)
            {
                Bouton = GetComponent<Image>();
            }
            Remplissage.fillAmount = 0f;
            Remplissage.rectTransform.sizeDelta = new Vector2(Bouton.rectTransform.rect.width, Bouton.rectTransform.rect.height);
        }

        private void Update()
        {
            // Timer au bout duquel le bouton est cliqué
            if (_hasFocus)
            {
                // INCREMENTATION
                delta += Time.deltaTime;

                if(delta >= Latence)
                {
                    // REMPLISSAGE
                    Remplissage.fillAmount += Time.deltaTime / (DureeAvantConfirmation);
                }

                // TEST CONDITION ARRET
                if (Remplissage.fillAmount >= 1)
                {
                    if (OnButtonClicked != null)
                    {
                        OnButtonClicked.Invoke(gameObject);
                    }
                    delta = 0f;
                    Remplissage.fillAmount = 0f;
                }
            }
        }

        /// <summary>
        /// Updates the button state and starts an animation of the button.
        /// </summary>
        /// <param name="newState">The state the button should transition to.</param>
        private void UpdateState(ButtonState newState)
        {
            var oldState = _currentButtonState;
            _currentButtonState = newState;

            // Variables for when the button is pressed or clicked.
            var buttonPressed = newState == ButtonState.PressedDown;
            var buttonClicked = (oldState == ButtonState.PressedDown && newState == ButtonState.Focused);

            // If the button is being pressed down or clicked, animate the button click.
            if (buttonPressed || buttonClicked)
            {
                _uiGazeButtonGraphics.AnimateButtonPress(_currentButtonState);
            }
            // In all other cases, animate the visual feedback.
            else
            {
                _uiGazeButtonGraphics.AnimateButtonVisualFeedback(_currentButtonState);
            }
        }

        /// <summary>
        /// Method called by Tobii XR when the gaze focus changes by implementing <see cref="IGazeFocusable"/>.
        /// </summary>
        /// <param name="hasFocus"></param>
        public void GazeFocusChanged(bool hasFocus)
        {
            // Don't use this method if the component is disabled.
            if (!enabled) return;

            delta = 0f;
            _hasFocus = hasFocus;
            Remplissage.fillAmount = 0f;

            UpdateState(hasFocus ? ButtonState.Focused : ButtonState.Idle);
        }
    }
}
