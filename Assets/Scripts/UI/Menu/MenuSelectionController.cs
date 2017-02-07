using UnityEngine;
using System.Collections;
using TankArena.Utils;
using UnityEngine.UI;

namespace TankArena.UI
{

	public class MenuSelectionController : MonoBehaviour 
	{

		public Image playerAvatar;

		// Use this for initialization
		void Start ()
		{
			if (CurrentState.Instance.Player.Character == null)
			{
				TransitionUtil.StartTransitionTo(SceneIds.SCENE_CHARACTER_SELECT_ID);
			}
			playerAvatar.sprite = CurrentState.Instance.Player.Character.Avatar;
		}
		
		// Update is called once per frame
		void Update () 
		{
		
		}

		public void PickArena() 
		{
			if (CurrentState.Instance.Player.Character == null)
			{
				TransitionUtil.StartTransitionTo(SceneIds.SCENE_CHARACTER_SELECT_ID);
			} else 
			{
				TransitionUtil.StartTransitionTo(SceneIds.SCENE_ARENA_SELECT_ID);
			}
		}
		public void PickOptions()
		{
			//TODO: Options?!
			DBG.Log("Options?!...");
		}
		public void PickExit()
		{
			Application.Quit();
		}
	}
}
