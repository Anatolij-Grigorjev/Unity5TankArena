using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TankArena.Models.Level;
using TankArena.Constants;
using TankArena.Utils;
using UnityEngine.SceneManagement;

public class ArenaSelectController : MonoBehaviour {


	public Image arenaThumbnail;
	public Text arenaNameText;
	public Text arenaDescriptionText;
	public Text arenaPropsText;
	public Button playArenaButton;
	public Button prevArenaButton;
	public Button nextArenaButton;

	public List<LevelModel> arenaModels;
	private Dictionary<string, object> levelInfoMappings = new Dictionary<string, object>() {
			{ UITextKeyMappings.MAPPING_ARENA_NAME, null },
			{ UITextKeyMappings.MAPPING_ARENA_ENEMIES_COUNT, null },
			{ UITextKeyMappings.MAPPING_ARENA_ENEMY_TYPES, null }
	};
	private readonly string LEVEL_INFO_TEMPLATE = "COUNT: {count}\nTYPES: {types}";

	private int currentArenaIndex;
	public int CurrentArenaIndex
	{
		get 
		{
			return currentArenaIndex;
		}
		set 
		{
			if (value != currentArenaIndex) 
			{
				currentArenaIndex = value;
				prevArenaButton.gameObject.SetActive(currentArenaIndex > 0);
				nextArenaButton.gameObject.SetActive(currentArenaIndex < (arenaModels.Count - 1) );
				var selectedModel = arenaModels[Safe(currentArenaIndex)];
				SetArenaModel(selectedModel);
				CurrentState.Instance.CurrentLevel = selectedModel;
				DBG.Log("Selected Arena: {0}", CurrentState.Instance.CurrentLevel.Id);
			}
		}
	}

	private int Safe(int index)
	{
		return Mathf.Clamp(index, 0, arenaModels.Count - 1);
	}

	private void SetArenaModel(LevelModel arena)
	{
		if (arena != null) 
		{
			arenaNameText.text = arena.Name;
			arenaThumbnail.sprite = arena.Thumbnail;
			arenaDescriptionText.text = arena.Description;
			arenaPropsText.text = TextUtils.ApplyPropsToTemplate(LEVEL_INFO_TEMPLATE, MapLevelInfo(arena));
		}
	}
	private Dictionary<string, object> MapLevelInfo(LevelModel level) 
	{
		levelInfoMappings[UITextKeyMappings.MAPPING_ARENA_ENEMIES_COUNT] = level.TotalEnemies;
		levelInfoMappings[UITextKeyMappings.MAPPING_ARENA_ENEMY_TYPES] = string.Join(", ", level.EnemyTypes.ToArray());

		return levelInfoMappings;
	}

	public void NextArenaButtonClick()
	{
		CurrentArenaIndex = Safe(currentArenaIndex + 1);
	}
	public void PrevArenaButtonClick()
	{
		CurrentArenaIndex = Safe(currentArenaIndex - 1);
	}

	// Use this for initialization
	void Start () 
	{
		EntitiesStore.Instance.GetStatus();
		arenaModels = new List<LevelModel>(EntitiesStore.Instance.Levels.Values);
		currentArenaIndex = 999;
		CurrentArenaIndex = 0;
	}


	public void PlaySelectedArena()
	{
		if (CurrentState.Instance.CurrentLevel != null)
		{
			SceneManager.LoadScene(SceneIds.SCENE_LOADING_ID);
		}
	}
	
}
