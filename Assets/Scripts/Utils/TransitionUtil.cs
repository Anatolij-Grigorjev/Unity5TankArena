using UnityEngine.SceneManagement;
using TankArena.Constants;

namespace TankArena.Utils
{
    public class TransitionUtil
    {
        public static void StartTransitionTo(int sceneId)
        {
            CurrentState.Instance.NextSceneId = sceneId;
            SceneManager.LoadScene(SceneIds.SCENE_LOADING_ID);
        }
    }
}