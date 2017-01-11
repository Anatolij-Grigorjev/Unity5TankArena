using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;
using TankArena.Utils;

namespace TankArena.Controllers
{
    public class TankEngineController : BaseTankPartController<TankEngine> {

        public AudioSource audioIdle;
        public AudioSource audioRevving;

	    // Use this for initialization
	    public override void Awake () {

            base.Awake();

            DBG.Log("Engine Controller Awoke!");
	    }
	
	    // Update is called once per frame
	    void Update () {
	        
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
