using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using TankArena.Constants;
using TankArena.Utils;
using TankArena.Models.Characters;
using UnityEngine.SceneManagement;
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
		public Image sceneBackground;
		public Image characterModel;
		public Text characterName;
		public GameObject loadoutGrid;
		public GameObject loadoutGridItem;

		public List<PlayableCharacter> characterData;
		private List<ShopPurchaseableEntityModel> loadoutItems;

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

			//redo loadout
			loadoutItems.Clear();
			loadoutGrid.ClearChildren();

			loadoutItems.AddRange(model.StartingTank.partsArray);
			loadoutItems.AddRange(model.StartingTank.TankTurret.allWeaponSlots.Select(slot => slot.Weapon).ToArray());

			loadoutItems.ForEach(shopItem => {
				var itemGO = Instantiate(loadoutGridItem, Vector3.zero, Quaternion.identity, loadoutGrid.transform) as GameObject;
				var childTr = itemGO.transform.GetChild(0);
				if (childTr != null) 
				{
					childTr.gameObject.GetComponent<Image>().sprite = shopItem.ShopItem;
				}
			});
		}

		public void Awake()
		{
			loadoutItems = new List<ShopPurchaseableEntityModel>();
			//ensure entities loaded (characters have decoded tanks)
			var entities = EntitiesStore.Instance;
			entities.GetStatus();
			
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
					avatarsGrid[i / GRID_ROW_LENGTH][i % GRID_COLS_COUNT].sprite = characterData[i].Avatar;
				}
			}

			CharacterIndex = 0;
		}

		//TODO: handle keyboard input for character selection
		public void Update()
		{
			var vert = Input.GetAxis(ControlsButtonNames.BTN_NAME_TANK_MOVE);
			if (Mathf.Abs(vert) > 0.5)
			{
				CharacterIndex += (int)(Mathf.Sign(vert) * GRID_COLS_COUNT);
			}
			var horiz = Input.GetAxis(ControlsButtonNames.BTN_NAME_TANK_TURN);
			if (Mathf.Abs(horiz) > 0.5)
			{
				CharacterIndex += (int)(Mathf.Sign(horiz));
			}
		}

	}
}