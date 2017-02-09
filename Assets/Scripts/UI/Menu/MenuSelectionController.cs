using UnityEngine;
using System.Collections;
using TankArena.Utils;
using UnityEngine.UI;

namespace TankArena.UI
{

	public class MenuSelectionController : MonoBehaviour 
	{

		public Image playerAvatar;
		public GameObject initialOptions;
		public GameObject playerOptions;

		// Use this for initialization
		void Start ()
		{
			//no character picked yet
			if (CurrentState.Instance.Player == null)
			{
				playerOptions.SetActive(false);
				initialOptions.SetActive(true);
				playerAvatar.gameObject.SetActive(false);
				TransitionUtil.StartTransitionTo(SceneIds.SCENE_CHARACTER_SELECT_ID);
			} else 
			{
				playerAvatar.gameObject.SetActive(true);
				playerAvatar.sprite = CurrentState.Instance.Player.Character.Avatar;
				playerOptions.SetActive(true);
				initialOptions.SetActive(false);
			}
		}
		

		public void PickArena() 
		{
			if (CurrentState.Instance.Player == null)
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
		public void PickBack()
		{
			CurrentState.Instance.ClearPlayer();
			TransitionUtil.StartTransitionTo(SceneIds.SCENE_MENU_ID);
		}
		public void PickStartGame()
		{
			TransitionUtil.StartTransitionTo(SceneIds.SCENE_CHARACTER_SELECT_ID);
		}
	}
}
