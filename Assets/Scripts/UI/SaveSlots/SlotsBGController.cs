using UnityEngine;
using System.Collections;
using TankArena.Utils;

namespace TankArena.UI 
{
	public class SlotsBGController : MonoBehaviour 
	{
		public void GoBackToMain()
		{
			TransitionUtil.StartTransitionTo(SceneIds.SCENE_MENU_ID);
		}
	}
}
