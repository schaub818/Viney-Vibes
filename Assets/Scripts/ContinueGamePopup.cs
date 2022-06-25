using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace VineyVibes
{
    // Manages display of the "KEEP GROWING" popup
    public class ContinueGamePopup : MonoBehaviour
    {
        // Popup display parameters
        public float disappearSpeed = 3.0f;
        public float disappearTimer = 0.75f;
        public float moveSpeed = 1.0f;

        // Stores the popup's text mesh
        private TextMeshPro textMesh;

        // The color of the text popup
        private Color textColor;

        // Creates the text popup at the specified position
        public static ContinueGamePopup Create(Vector3 startPosition)
        {
            // Load and instantiate the popup prefab
            Transform popupPrefab = Resources.Load<Transform>("ContinuePopup");
            Transform popupTransform = Instantiate(popupPrefab, startPosition, Quaternion.identity);

            // Get the popup component of the prefab
            ContinueGamePopup continueGamePopup = popupTransform.GetComponent<ContinueGamePopup>();

            return continueGamePopup;
        }

        // Executes when the popup is instantiated
        private void Awake()
        {
            // Stores the initial color of the popup
            textMesh = GetComponent<TextMeshPro>();
            textColor = textMesh.color;
        }

        // Update is called once per frame
        private void Update()
        {
            // Move the popup up
            transform.position += new Vector3(0.0f, moveSpeed) * Time.deltaTime;

            // Update the time left before the popup disappears
            disappearTimer -= Time.deltaTime;

            // Continue fading the popup out if there is time left before it finishes fading
            if (disappearTimer < 0)
            {
                // Fade the popup
                textColor.a -= disappearSpeed * Time.deltaTime;
                textMesh.color = textColor;

                // If the popup is fully transparent, destroy it
                if (textColor.a < 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}