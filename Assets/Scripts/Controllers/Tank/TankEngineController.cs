using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;
using TankArena.Utils;

namespace TankArena.Controllers
{
    public class TankEngineController : BaseTankPartController<TankEngine> {

        public AudioSource audioIdle;
        public AudioSource audioRevving;
        public AudioSource audioBoost;

        private const float BOOST_COOLDOWN = 2.5f;
        private float currentBoostCooldown;

	    // Use this for initialization
	    public override void Awake () {

            base.Awake();
            currentBoostCooldown = 0.0f;
            DBG.Log("Engine Controller Awoke!");
	    }
	
	    // Update is called once per frame
	    void Update () {
	        if (currentBoostCooldown > 0.0f)
            {
                currentBoostCooldown -= Time.deltaTime;
            }
	    }

        public float ProcessBoost()
        {
            if (currentBoostCooldown <= 0.0f) 
            {
                //do a boost
                audioBoost.Play();
                currentBoostCooldown = BOOST_COOLDOWN;

                return 29.5f;
            }

            return 1.0f;
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
