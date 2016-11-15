using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TankArena.Utils
{
    /// <summary>
    /// Because an actual Transform can only be a part of a valid GO,
    /// this utility stores transform data and copies to and from it
    /// </summary>
    public class TransformState
    {
        public Vector3 position = new Vector3();
        public Vector3 rotation = new Vector3();
        public Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);

        

        public static TransformState Identity
        {
            get
            {
                return new TransformState();
            }
        }

        public void CopyToTransform(Transform t)
        {
            t.localScale = scale;
            t.localPosition = position;
            t.localRotation = Quaternion.Euler(rotation);
        }

        public void CopyFromTransform(Transform t)
        {
            position = t.localPosition;
            rotation = t.localRotation.eulerAngles;
            scale = t.localScale;
        }

        public static TransformState fromTransform(Transform t)
        {
            var state = new TransformState();
            state.CopyFromTransform(t);

            return state;
        }
    }
}
