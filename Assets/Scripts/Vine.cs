using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VineyVibes
{
    public class Vine : MonoBehaviour
    {
        // the fastest the vine can grow
        public static float MinimumGrowTime = 0.66f;

        // the time it takes to grow the fine
        public float GrowTime
        {
            get { return growTime; }
            set { growTime = value; }
        }

        // a value to check if the vine is growing
        public bool Growing
        {
            get { return growing; }
            set { growing = value; }
        }

        // a value to flag when the vine is done growing
        public bool DoneGrowing
        {
            get { return doneGrowing; }
        }

        public VineType Type
        {
            get { return type; }
            set { type = value; }
        }

        // the time it takes to grow from start to finish
        private float growTime;

        // track whether we are growing
        private bool growing;

        // track whether we are done growing
        private bool doneGrowing;

        // add animation controller 
        private Animator animator;

        // vine does not typically write to this
        private VineType type;

        // function used only by an animation event
        // triggers when the animation is completed
        private void SetDoneGrowing()
        {
            doneGrowing = true;
        }

        // Start is called before the first frame update
        // we might not need this
        private void Start()
        {
            // pick a random animation given the type
            animator = GetComponent<Animator>();

            // set random value in the animator
            animator.SetInteger("RandomNumber", Random.Range(0, 5) + 1);
        }

        // Update is called once per frame
        private void Update()
        {
            // if done growing, disable the script
            if (doneGrowing)
            {
                // disable THIS vine script since we're done handling it.
                this.enabled = false;
                return;
            }
            else
            {
                if (growing)
                {
                    // check grow time lower bound
                    if (growTime < MinimumGrowTime)
                    {
                        growTime = MinimumGrowTime;
                    }

                    // update grow speed
                    // smaller grow time -> larger speed
                    animator.speed = 4.0f / growTime;

                    // play animation
                    animator.SetBool(type.ToString(), true);
                }
            }
            

        }
    }
}