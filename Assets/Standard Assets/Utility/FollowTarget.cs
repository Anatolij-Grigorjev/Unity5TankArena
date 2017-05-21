using System;
using UnityEngine;


namespace UnityStandardAssets.Utility
{
    public class FollowTarget : MonoBehaviour
    {
        public Transform target;
        public bool useTargetRotation = false;
        public Vector3 offset = new Vector3(0f, 7.5f, 0f);
        public Vector3 currentOffset;

        void Start()
        {
            currentOffset = offset;
        }


        private void LateUpdate()
        {
            transform.position = target.position + currentOffset;
            if (useTargetRotation)
            {
                transform.rotation = target.rotation;
                //adjust follow position based on turn trig (both X and Y)
                Debug.Log(target.rotation.eulerAngles.z);
                currentOffset = offset + (Mathf.Sign(target.rotation.eulerAngles.z) * new Vector3(
                    Mathf.Cos(target.rotation.eulerAngles.z * Mathf.Deg2Rad) * transform.localScale.x,
                    Math.Abs(Mathf.Sin(target.rotation.eulerAngles.z * Mathf.Deg2Rad)) * transform.localScale.y,
                    0.0f
                ));
            }
        }
    }
}
