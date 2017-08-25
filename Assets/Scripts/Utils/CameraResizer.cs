using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankArena.Utils
{
    public class CameraResizer : MonoBehaviour
    {

        public Camera camera;
        public float maxSize = 220;
        public float minSize = 130;
        public float reactionDelay = 0.75f; //how long to wait before reacting to value changes
        public Func<float> valueProvider;
        public float maxValue;
        public float minValue = 0.0f;
        private float cameraStep; //describes amount of value needed for 1 step of camera
        private float prevVal = 0.0f;
        private float currentReactDelay;
        // Use this for initialization
        void Start()
        {

            RecalcCameraStep();

        }

        // Update is called once per frame
        void Update()
        {
            var newVal = valueProvider();
            //adjust reaction time to movement based on values similarity
            if (Math.Abs(newVal - prevVal) > 0)
            {
                currentReactDelay = Mathf.Clamp(currentReactDelay - Time.deltaTime, 0.0f, reactionDelay);
            }
            else
            {
                currentReactDelay = Mathf.Clamp(currentReactDelay + Time.deltaTime, 0.0f, reactionDelay);
            }
            if (currentReactDelay == 0.0f)
            {
                var shift = (newVal - prevVal) / cameraStep;
                camera.orthographicSize = Mathf.Clamp(camera.orthographicSize + shift, minSize, maxSize);
            }
            prevVal = newVal;
        }

        private void RecalcCameraStep()
        {
            float cameraRange = maxSize - minSize;
            float valueRange = maxValue - minValue;

            cameraStep = valueRange / cameraRange;
        }

        public void SetValues(float minValue, float maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;

            RecalcCameraStep();
        }

    }
}

