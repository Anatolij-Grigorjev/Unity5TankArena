using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using TankArena.Constants;
using TankArena.Utils;
using TankArena.Models.Characters;
using System;
using TankArena.Models;

namespace TankArena.UI.Characters
{
    public class CharacterSelectController: MonoBehaviour
	{
		private const int GRID_ROW_LENGTH = 2;

		private const int GRID_COLS_COUNT = 2;
		private static readonly Color COLOR_TINT_SELECTED_AVATAR = Color.cyan;
		static CharacterSelectController() {
			COLOR_TINT_SELECTED_AVATAR.a = 0.5f;
		}

		public Image[][] avatarsGrid;
		public Sprite defaultImage;
		public Image sceneBackground;
		public Image characterModel;
		public Text characterName;
		public Text characterBackstory;
		public Text characterGoal;
		public Text characterMoney;
		public Text playerNameText;
		public LoadoutGridController loadoutGridController;
		public StatsGridController statsGridController;
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
				DBG.Log("Value: {0}, Safe: {1}", value, safeIndex);
				var lastSelectedImage = GetSelectedAvatar(currentCharacterIndex);
				lastSelectedImage.color = Color.white;
				var newImage = GetSelectedAvatar(safeIndex);
				newImage.color = COLOR_TINT_SELECTED_AVATAR;
				currentCharacterIndex = safeIndex;
				

				UpdateUI(currentCharacterIndex);
			}
		}


		private Image GetSelectedAvatar(int index)
		{
			return avatarsGrid[index / GRID_ROW_LENGTH][index % GRID_COLS_COUNT];
		}

		private void UpdateUI(int selectedChar)
		{
			var model = characterData[selectedChar];

			characterName.text = model.Name;
			sceneBackground.sprite = model.Background;
			characterModel.sprite = model.CharacterModel;
			characterMoney.text = "$" + model.StartingCash;
			characterBackstory.text = model.Backstory;
			characterGoal.text = model.GoalTemplate.GetCharacterGoalDescription();

			//refresh loadout
			loadoutGridController.SetLoadoutData(model.StartingTank);
			//refresh stats
			statsGridController.SetStats(model.StartingStats);
		}

		public void Awake()
		{
			//ensure entities loaded (characters have decoded tanks)
			var entities = EntitiesStore.Instance;
			entities.GetStatus();
			characterData = entities.Characters.Values.ToList();
			playerNameText.text = CurrentState.Instance.Player.Name;
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
						avatarsGrid[i][j].sprite = defaultImage;
					}
				}

				//set buttons on the avatars and set no buttons for non-avatar squares
				for (int i = 0; i < characterData.Count; i++)
				{
					var gridImage = avatarsGrid[i / GRID_ROW_LENGTH][i % GRID_COLS_COUNT];
					gridImage.sprite = characterData[i].Avatar;
					gridImage.gameObject.AddComponent<Button>();
					DBG.Log("Button {0} index {1}", characterData[i].Name, i);
					var model = characterData[i];
					gridImage.gameObject.GetComponent<Button>().onClick.AddListener(() => { 
						CharacterIndex = characterData.FindIndex(other => model.Id == other.Id);
					});
				}
			}

			CharacterIndex = 0;
		}

		public void Update()
		{
			
		}

		public void SelectCharacter()
		{
			var model = characterData[CharacterIndex];
			var player = CurrentState.Instance.Player;
			player.Cash = model.StartingCash;
			player.Character = model;
			player.CurrentTank = model.StartingTank;
			player.CurrentStats = model.StartingStats;
			player.PlayerStats = new PlayerStats();
			player.CharacterGoal = (AbstractCharacterGoal)Activator.CreateInstance(model.CharacterGoalType);
			player.CharacterGoal.Init(player.PlayerStats);
			
			CurrentState.Instance.SetPlayer(player);
			
			TransitionUtil.SaveAndStartTransitionTo(SceneIds.SCENE_MENU_ID);
		}

	}
}