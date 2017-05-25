using System.Collections;
using System.Collections.Generic;
using MovementEffects;
using TankArena.Constants;
using TankArena.Utils;
using UnityEngine;

namespace TankArena.UI
{
    public class MenuMusicsController : Singleton<MenuMusicsController>
    {

        void Awake()
        {
            mainMenuMusic = Resources.Load<AudioClip>(PrefabPaths.AUDIO_CLIP_MENU_MUSIC);
            saveSlotMusic = Resources.Load<AudioClip>(PrefabPaths.AUDIO_CLIP_SAVE_SLOT_MUSIC);

            source = gameObject.AddComponent<AudioSource>();
            source.loop = true;
        }

        public AudioClip mainMenuMusic;
        public AudioClip saveSlotMusic;
        public AudioSource source;

		public void SwitchToMenuMusic()
		{
			SwitchToMusic(mainMenuMusic);
		}

		public void SwitchToSaveMusic()
		{
			SwitchToMusic(saveSlotMusic);
		}

        private void SwitchToMusic(AudioClip music)
        {
			//simple case 1 - this track already playing or about to play
			if (source.clip == music) {
				if (!source.isPlaying) {
					source.Play();
				}
				return;
			}
            //simple case 2 - no current clip or nothing playing,
            //slot it in and play
            if (source.clip == null || !source.isPlaying)
            {
                source.clip = music;
                source.Play();
                return;
            }

            // somethin else playing, perform crossfade
			Timing.RunCoroutine(_CrossfadeToTrack(music));
        }

        private IEnumerator<float> _CrossfadeToTrack(AudioClip track, float crossfadeDuration = 3.0f)
        {
            var clip = source.clip;
            //half duration for fade-out and half for fade-in
            var halfDuration = crossfadeDuration / 2;
            var time = halfDuration;
            while (time > 0)
            {
				source.volume = Mathf.Lerp(1.0f, 0.0f, Mathf.SmoothStep(0.0f, 1.0f, (halfDuration - time) / halfDuration));
				time -= Timing.DeltaTime;
				yield return Timing.WaitForSeconds(Timing.DeltaTime);
            }
			time = halfDuration;
			source.volume = 0.0f;
			//fade back in
			source.clip = track;
			while (time > 0) 
			{
				source.volume = Mathf.Lerp(0.0f, 1.0f, Mathf.SmoothStep(0.0f, 1.0f, (halfDuration - time) / halfDuration));
				time -= Timing.DeltaTime;
				yield return Timing.WaitForSeconds(Timing.DeltaTime);
			}

			source.volume = 1.0f;
        }

    }
}
