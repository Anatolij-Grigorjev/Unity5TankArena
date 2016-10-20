using UnityEngine;
using System.Collections;
using System;

namespace TankArena.Controllers
{
    public class CameraFollowObjectController : MonoBehaviour
    {

        private GameObject go;       //variable to store a reference to the game object
        public GameObject Target
        {
            get
            {
                return go;
            }
            set
            {
                go = value;
                RecalcOffset();
            }
        }

        //Object to initialize the Target with, for use in the Editor ONLY
        public GameObject starter;

        private Vector3 offset;         //Private variable to store the offset distance between the object and camera

        // Use this for initialization
        void Start()
        {

            //Calculate and store the offset value by getting the distance between the object's position and camera's position.
            Target = starter;
        }

        // LateUpdate is called after Update each frame
        void LateUpdate()
        {
            // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
            transform.position = go.transform.position + offset;
        }

        public void SetGO(GameObject newGo)
        {
            go = newGo;
            RecalcOffset();
        }

        private void RecalcOffset()
        {
            if (go == null)
            {
                offset = Vector3.zero;
            }
            else
            {
                offset = transform.position - go.transform.position;
            }
        }
    }
}

