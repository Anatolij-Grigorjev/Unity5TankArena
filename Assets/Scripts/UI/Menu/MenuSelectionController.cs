using UnityEngine;
using System.Collections;
using TankArena.Utils;
using TankArena.Constants;
using UnityEngine.UI;
using System.Collections.Generic;

namespace TankArena.UI
{

	public class MenuSelectionController : MonoBehaviour 
	{

		public PlayerStuffController playerStuffController;
		public GameObject initialOptions;
		public GameObject playerOptions;

		

		void Start ()
		{
			//no character picked yet, main menu
			if (CurrentState.Instance.Player == null)
			{
				playerOptions.SetActive(false);
				initialOptions.SetActive(true);
				playerStuffController.gameObject.SetActive(false);
			} else 
			{
				//character picked, character menu
				playerStuffController.gameObject.SetActive(true);
				playerStuffController.SetPlayerInfo(CurrentState.Instance.Player);
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
		public void PickShop()
		{
			TransitionUtil.StartTransitionTo(SceneIds.SCENE_SHOP_ID);
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
			TransitionUtil.StartTransitionTo(SceneIds.SCENE_SAVE_SLOTS_ID);
		}
		public void PickLoadGame()
		{
			TransitionUtil.StartTransitionTo(SceneIds.SCENE_SAVE_SLOTS_ID, new Dictionary<string, object>() {
				{TransitionParams.PARAM_SLOTS_FOR_LOAD, true}
			});
		}
	}
}
