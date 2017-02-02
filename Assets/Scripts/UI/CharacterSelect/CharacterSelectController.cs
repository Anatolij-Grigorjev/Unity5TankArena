using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TankArena.Constants;
using TankArena.Utils;
using TankArena.Models.Characters;
using UnityEngine.SceneManagement;

namespace TankArena.UI.Characters
{
	public class CharacterSelectController: MonoBehavior
	{

		private const int GRID_ROW_LENGTH = 2;

		private const int GRID_COLS_COUNT = 2;

		public Image[GRID_ROW_LENGTH][GRID_COLS_COUNT] avatarsGrid;

		public List<PlayableCharacter> characterData;

		public Button selectAndPlay;

		private int currentCharacterIndex;

		public int CharacterIndex
		{
			get 
			{
				return currentCharacterIndex;
			}
			set 
			{

			}
		}

		public void Awake()
		{
			//ensure entities loaded
			var entities = EntitiesStore.Instance;

			characterData = entities.Characters;
			if (characterData != null && characterData.Count > 0) 
			{
				//proceed with the loading of avatars into cels grid
				avatarsGrid = new Image[GRID_ROW_LENGTH][GRID_COLS_COUNT];
				for (int i = 0; i < characterData.Count; i++)
				{

				}
			}
		}

		//TODO: handle keyboard input for character selection
		public void Update()
		{

		}

	}
}