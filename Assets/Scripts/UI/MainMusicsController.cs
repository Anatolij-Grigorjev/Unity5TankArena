using System.Collections;
using System.Collections.Generic;
using MovementEffects;
using TankArena.Constants;
using TankArena.Utils;
using UnityEngine;

namespace TankArena.UI
{
    public class MainMusicsController : Singleton<MainMusicsController>
    {

        void Awake()
        {
            rnd = new System.Random();
            source = gameObject.AddComponent<AudioSource>();
            source.loop = true;

            loadMusicsHandle = Timing.RunCoroutine(_LoadMusics());
        }

        private IEnumerator<float> _LoadMusics()
        {
            mainMenuMusic = Resources.Load<AudioClip>(PrefabPaths.AUDIO_CLIP_MENU_MUSIC);
            shopMusic = Resources.Load<AudioClip>(PrefabPaths.AUDIO_CLIP_SHOP_MUSIC);
            arenaMusics = new List<AudioClip>();
            for (int i = 0; i < arenaMusicsCount; i++) {
                arenaMusics.Add(Resources.Load<AudioClip>(PrefabPaths.AUDIO_CLIP_ARENA_MUSIC(i + 1)));
            }

            yield return 0;
        }

        private IEnumerator<float> loadMusicsHandle;
        public AudioClip mainMenuMusic;
        public int arenaMusicsCount = 5;
        public AudioClip shopMusic;
        public List<AudioClip> arenaMusics;
        private System.Random rnd;
        public AudioSource source;

		public void SwitchToMenuMusic()
		{
			Timing.RunCoroutine(_SwitchToMusicAsync(mainMenuMusic));
		}
        public void SwitchToShopMusic()
        {
            Timing.RunCoroutine(_SwitchToMusicAsync(shopMusic));
        }

        public void SwitchToRandomArenaMusic()
        {
            Timing.RunCoroutine(_SwitchToMusicAsync(
                arenaMusics[rnd.Next(arenaMusics.Count)]
            ));
        }

        private IEnumerator<float> _SwitchToMusicAsync(AudioClip music)
        {
            yield return Timing.WaitUntilDone(loadMusicsHandle);
			//simple case 1 - this track already playing or about to play
			if (source.clip == music) {
				if (!source.isPlaying) {
					source.Play();
				}

                yield break;
			}
            //simple case 2 - no current clip or nothing playing,
            //slot it in and play
            if (source.clip == null || !source.isPlaying)
            {
                source.clip = music;
                source.Play();
                yield break;
            }

            // somethin else playing, perform crossfade
			Timing.RunCoroutine(_CrossfadeToTrack(music));
        }

        private IEnumerator<float> _CrossfadeToTrack(AudioClip track, float crossfadeDuration = 0.3f)
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
            source.Play();
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
