using UnityEngine.SceneManagement;
using TankArena.Constants;
using System.Collections.Generic;

namespace TankArena.Utils
{
    public class TransitionUtil
    {
        public static void StartTransitionTo(int sceneId, Dictionary<string ,object> sceneParams = null)
        {
            CurrentState.Instance.NextSceneId = sceneId;
            CurrentState.Instance.CurrentSceneParams.Clear();
            if (sceneParams != null) 
            {
                foreach(var param in sceneParams) 
                {
                    CurrentState.Instance.CurrentSceneParams.Add(param.Key, param.Value);
                }
            }
            SceneManager.LoadScene(SceneIds.SCENE_LOADING_ID);
        }
    }
}