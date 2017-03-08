using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;
using TankArena.Utils;

namespace TankArena.Controllers
{
    public class TankEngineController : BaseTankPartController<TankEngine>
    {

        public AudioSource audioIdle;
        public AudioSource audioRevving;

        [HideInInspector]
        public bool isMoving;

        //max acceleration from model, adjusted for mass and torque balance
        public float adjustedMaxAcceleration;
        // Use this for initialization
        public override void Awake()
        {

            base.Awake();
            isMoving = false;

            DBG.Log("Engine Controller Ready!");

        }

        
        void Update()
        {
            //update acceleration changes if the thing is moving
            if (isMoving)
            {
    
                if (Model.currentAcceleration < adjustedMaxAcceleration) 
                {
                    Model.currentAcceleration =
                        Mathf.Clamp(
                        Model.currentAcceleration + Model.AccelerationRate * Time.deltaTime,
                        0.0f,
                        adjustedMaxAcceleration);
                }
            } else
            {
                if (Model.currentAcceleration > 0.0f) 
                {
                    Model.currentAcceleration = 
                        Mathf.Clamp(
                            Model.currentAcceleration - Model.DeaccelerationRate * Time.deltaTime,
                            0.0f,
                            adjustedMaxAcceleration
                        );
                }
            }
        }

        public void StartIdle()
        {
            StopStartAudio(audioRevving, audioIdle);
        }

        public void StartRevving()
        {
            StopStartAudio(audioIdle, audioRevving);
        }

        private void StopStartAudio(AudioSource stopMe, AudioSource playMe)
        {
            if (stopMe.isPlaying)
            {
                stopMe.Stop();
                stopMe.loop = false;
            }
            if (!playMe.isPlaying)
            {
                playMe.Play();
                playMe.loop = true;
            }
        }
    }
}
