using UnityEngine.SceneManagement;
using TankArena.Constants;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;
using TankArena.UI;
using System;

namespace TankArena.Utils
{
    public class TransitionUtil
    {
        public static void StartTransitionTo(int sceneId, Dictionary<string, object> sceneParams = null)
        {
            CurrentState.Instance.NextSceneId = sceneId;
            CurrentState.Instance.CurrentSceneParams.Clear();
            if (sceneParams != null)
            {
                foreach (var param in sceneParams)
                {
                    CurrentState.Instance.CurrentSceneParams.Add(param.Key, param.Value);
                }
            }
            SceneManager.LoadScene(SceneIds.SCENE_LOADING_ID);
            LoadNextSceneMusic(sceneId);
        }

        private static void LoadNextSceneMusic(int sceneId)
        {
            switch(sceneId)
            {
                case SceneIds.SCENE_ARENA_ID: 
                    MainMusicsController.Instance.SwitchToRandomArenaMusic();
                    break;
                case SceneIds.SCENE_SAVE_SLOTS_ID:
                    MainMusicsController.Instance.SwitchToSaveMusic();
                    break;
                case SceneIds.SCENE_SHOP_ID:
                    MainMusicsController.Instance.SwitchToShopMusic();
                    break;
                default:
                    MainMusicsController.Instance.SwitchToMenuMusic();
                    break;
            }
        }

        public static void WaitAndStartTransitionTo(int sceneId,
                                                    float waitTime,
                                                    Dictionary<string, object> sceneParams = null)
        {
            if (waitTime > 0.0f)
            {
                Timing.RunCoroutine(_TransitionPostWait(waitTime, sceneId, sceneParams));
            }
            else
            {
                StartTransitionTo(sceneId, sceneParams);
            }
        }

        public static void SaveAndStartTransitionTo(int sceneId, Dictionary<string, object> sceneParams = null)
        {
            var canvas = GameObject.FindGameObjectWithTag(Tags.TAG_UI_CANVAS) as GameObject;
            if (canvas == null)
            {
                DBG.Log("Cant save when not on canvas! SKIPPING SAVE!");
                StartTransitionTo(sceneId, sceneParams);
            }
            var saveText =
             GameObject.Instantiate(EntitiesStore.Instance.SavingTextPrefab
             , Vector3.zero
             , Quaternion.identity
             , canvas.transform) as GameObject;
            //assign position again for anchored because Unity
            saveText.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

            var saveHandle = Timing.RunCoroutine(saveText.GetComponent<SaveTextController>()._PerformSave());

            Timing.RunCoroutine(_TransitionPostSave(saveHandle, sceneId, sceneParams));

        }

        private static IEnumerator<float> _TransitionPostWait(
            float waitTime,
            int sceneId,
            Dictionary<string, object> sceneParams = null)
        {
            DBG.Log("Waiting {0} before scene transition", waitTime);
            yield return Timing.WaitForSeconds(waitTime);

            StartTransitionTo(sceneId, sceneParams);
        }

        private static IEnumerator<float> _TransitionPostSave(
            IEnumerator<float> saveHandle,
            int sceneId,
            Dictionary<string, object> sceneParams = null)
        {
            yield return Timing.WaitUntilDone(saveHandle);

            //saving done, do transition
            StartTransitionTo(sceneId, sceneParams);
        }
    }
}