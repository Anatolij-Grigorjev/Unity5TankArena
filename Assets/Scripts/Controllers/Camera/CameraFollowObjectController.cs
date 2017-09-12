using UnityEngine;
using System.Collections;
using System;
using TankArena.Utils;

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

        public Vector2 mapBounds;
        public Vector2 mapPosition;

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

        public void SetMapInfo(Vector2 mapBounds, Vector2 mapOrigin)
        {
            this.mapBounds = mapBounds;
            this.mapPosition = mapOrigin;
            RecalcBounds();
        }

        public void SetOrthographicSize(float orthographicSize)
        {
            var camera = this.GetComponent<Camera>();
            camera.orthographicSize = orthographicSize;
            RecalcBounds();
        }

        public float GetOrthographicSize()
        {
            var camera = GetComponent<Camera>();
            return camera.orthographicSize;
        }

        private void RecalcBounds()
        {
            var camera = this.GetComponent<Camera>();
            var vertExtent = camera.orthographicSize;
            //horizontal extent is vertical by screen ratio
            var horizExtent = vertExtent * (Screen.width / Screen.height); 
            DBG.Log("Working with - extents: {0}| Map bounds: {1} | Map origin: {2}", new Vector2(horizExtent, vertExtent), mapBounds, mapPosition);
            

            cameraMinX = mapPosition.x + horizExtent;
            cameraMinY = mapPosition.y - mapBounds.y + vertExtent;
            cameraMaxX = mapPosition.x + mapBounds.x - horizExtent;
            cameraMaxY = mapPosition.y - vertExtent;
            
            //swap as needed
            if (cameraMinX > cameraMaxX) {
                ExtensionMethods.Swap(ref cameraMinX, ref cameraMaxX);
            }
            if (cameraMinY > cameraMaxY) {
                ExtensionMethods.Swap(ref cameraMinY, ref cameraMaxY);
            }

            DBG.Log("Decided bounds - X: {0} | Y: {1}", new Vector2(cameraMinX, cameraMaxX), new Vector2(cameraMinY, cameraMaxY));
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

