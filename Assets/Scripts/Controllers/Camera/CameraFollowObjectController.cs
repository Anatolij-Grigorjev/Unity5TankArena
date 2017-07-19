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

        public Vector2 cameraBoundsMin;
        public Vector2 cameraBoundsMax;

        //Object to initialize the Target with, for use in the Editor ONLY
        public GameObject starter;

        public Vector3 offset;         //Private variable to store the offset distance between the object and camera

        private float cameraMinX;
        private float cameraMinY;
        private float cameraMaxX;
        private float cameraMaxY;
        public bool useBounds;

        // Use this for initialization
        void Start()
        {
            //Calculate and store the offset value by getting the distance between the object's position and camera's position.
            if (starter != null)
            {
                Target = starter;
            }

            RecalcBounds();
        }

        // LateUpdate is called after Update each frame
        void LateUpdate()
        {
            // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
            var newPosition = Target.transform.position + offset;
            if (useBounds)
            {
                newPosition.x = Mathf.Clamp(newPosition.x, cameraMinX, cameraMaxX);
                newPosition.y = Mathf.Clamp(newPosition.y, cameraMinY, cameraMaxY);
            }

            transform.position = newPosition;
        }

        public void SetBounds(Vector2 min, Vector2 max)
        {
            cameraBoundsMin = min;
            cameraBoundsMax = max;
            RecalcBounds();
        }

        private void RecalcBounds()
        {
            cameraMinX = cameraBoundsMin.x;
            cameraMinY = cameraBoundsMin.y;
            cameraMaxX = cameraBoundsMax.x;
            cameraMaxY = cameraBoundsMax.y;
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

