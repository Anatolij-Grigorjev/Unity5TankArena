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
            CurrentState.Instance.CurrentSceneParams = sceneParams;
            SceneManager.LoadScene(SceneIds.SCENE_LOADING_ID);
        }
    }
}