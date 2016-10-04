using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TankArena.Controllers {
    public class FreeRoamCameraController : MonoBehaviour
    {

        public float moveSpeed; //static speed of movement in freeroam

        private Dictionary<string, Vector3> staticMoves;
        void Awake()
        {
            staticMoves = new Dictionary<string, Vector3>();
            //TODO: map move diections to amount of move like below, using moveSpeed
            staticMoves.Add("CameraMoveLeft", new Vector3(-moveSpeed, 0.0f));
            staticMoves.Add("CameraMoveRight", new Vector3(moveSpeed, 0.0f));
            staticMoves.Add("CameraMoveUp", new Vector3(0.0f, moveSpeed));
            staticMoves.Add("CameraMoveDown", new Vector3(0.0f, -moveSpeed));

        }

        // Update called once per frame (movement needs to be normalized via deltatime
        void Update()
        {
            foreach (var entry in staticMoves)
            {
                if (Input.GetButton(entry.Key))
                {
                    transform.position += (entry.Value * Time.deltaTime);
                }
            }
        }
    }
}
