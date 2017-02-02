using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using TankArena.Constants;
using TankArena.Utils;
using TankArena.Models.Characters;
using UnityEngine.SceneManagement;

namespace TankArena.UI.Characters
{
	public class CharacterSelectController: MonoBehaviour
	{

		private const int GRID_ROW_LENGTH = 2;

		private const int GRID_COLS_COUNT = 2;

		public Image[][] avatarsGrid;
		public Image sceneBackground;
		public Text characterName;

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
				int safeIndex = UIUtils.SafeIndex(value, characterData);
				
			}
		}

		public void Awake()
		{
			//ensure entities loaded
			var entities = EntitiesStore.Instance;

			characterData = entities.Characters.Values.ToList();
			if (characterData != null && characterData.Count > 0) 
			{
				//proceed with the loading of avatars into cels grid
				avatarsGrid = new Image[GRID_ROW_LENGTH][];
				for (int i = 0; i < GRID_ROW_LENGTH; i++) 
				{
					avatarsGrid[i] = new Image[GRID_COLS_COUNT];
					for (int j = 0; j < GRID_COLS_COUNT; j++)
					{
						avatarsGrid[i][j] =
						 GameObject.FindGameObjectWithTag(Tags.TAG_CHARACTER_AVATAR(i, j)).GetComponent<Image>();
					}
				}
				for (int i = 0; i < characterData.Count; i++)
				{
					DBG.Log("i:{0}\ni / ROWS: {1}\ni % COLS:{2}", i, i / GRID_ROW_LENGTH, i % GRID_COLS_COUNT);
					avatarsGrid[i / GRID_ROW_LENGTH][i % GRID_COLS_COUNT].sprite = characterData[i].Avatar;
				}
			}
		}

		//TODO: handle keyboard input for character selection
		public void Update()
		{

		}

	}
}