// --------------------------------------------
// Written by Andrés Fernandez
// with contributions from Dave Schaub
// --------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace VineyVibes
{
    public class PointsPopup : MonoBehaviour
    {

        // ALPHA CODE: public variables for easy tweak
        // make these private near release

        // controls the rate at which the popup fades out
        public float disappearSpeed = 3f;

        // after this amount of time, the pop up will start fading out
        public float disappearTimer = 0.75f;

        // the rate at which the popup moves up
        public float moveSpeed = 1f;

        public float pitchBendTo = 1.3f;

        // the text mesh we'll update?
        private TextMeshPro textMesh;

        // the color of the text (rgba)
        private Color textColor;

        // create a new points popup
        public static PointsPopup Create(Vector3 position, int pointAmount)
        {
            // fetch prefab from the resources folder
            Transform pfPointsPopup = Resources.Load<Transform>("PointsPopup");

            // instantiate the object
            Transform pointsPopupTransform = Instantiate(pfPointsPopup, position, Quaternion.identity);

            // fetch the script
            PointsPopup pointsPopup = pointsPopupTransform.GetComponent<PointsPopup>();

            // use setup function
            pointsPopup.Setup(pointAmount);

            // play sound
            FindObjectOfType<AudioManager>().PlaySoundEffect();

            // return the pointspopup
            return pointsPopup;
        }

        // a function that initializes our private variables
        public void Setup(int pointAmount)
        {
            // add points text to text mesh
            textMesh.text = "+" + pointAmount.ToString();

            // update the color of the text mesh
            UpdateTextColor(pointAmount);

        }

        // fetches the textMesh
        private void Awake()
        {
            textMesh = GetComponent<TextMeshPro>();
        }

        // moves text up, fades, destroys
        private void Update()
        {
            transform.position += new Vector3(0, moveSpeed) * Time.deltaTime;

            disappearTimer -= Time.deltaTime;

            if (disappearTimer < 0)
            {
                textColor.a -= disappearSpeed * Time.deltaTime;
                textMesh.color = textColor;

                if (textColor.a < 0)
                {
                    Destroy(gameObject);
                }
            }
        }

        private void UpdateTextColor(int pointAmount)
        {
            // fetch text color
            textColor = textMesh.color;

            // gray
            if (pointAmount == 0)
            {
                textColor = new Color(0.5471698f, 0.5471698f, 0.5471698f, 1.0f);
            }
            // white
            else if (pointAmount == 1)
            {
                textColor.b = 1f;
            }
            // mid yellow
            else if (pointAmount == 2)
            {
                textColor.b = 0.5f;
            }
            // deep yellow
            else if (pointAmount == 3)
            {
                textColor.b = 0f;
            }
            // orange
            else
            {
                textColor.b = 0f;
                textColor.g = 0.5f;
            }

            // update text mesh
            textMesh.color = textColor;
        }

    }
}