using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TankArena.Models.Level;
using TankArena.Constants;
using TankArena.Utils;
using System;
using System.Linq;

namespace TankArena.UI.Arena
{
    public class ArenaSelectController : MonoBehaviour
    {


        public Image arenaThumbnail;
        public Image arenaSnapshot;
        public Text arenaNameText;
        public Image playerAvatarImage;
        public Text arenaDescriptionText;
        public Text arenaPropsText;
        public Button playArenaButton;
        public Button prevArenaButton;
        public Button nextArenaButton;
        public GameObject lockedOverlay;
        public Text lockedText;

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
                    nextArenaButton.gameObject.SetActive(currentArenaIndex < (arenaModels.Count - 1));
                    var selectedModel = arenaModels[UIUtils.SafeIndex(currentArenaIndex, arenaModels)];
                    SetArenaModelUI(selectedModel);
                    CurrentState.Instance.CurrentArena = selectedModel;
                    playArenaButton.enabled = CheckArenaPlayable(selectedModel);
                    lockedOverlay.SetActive(!playArenaButton.enabled);
                    if (lockedOverlay.activeInHierarchy)
                    {
                        lockedText.text = String.Format("Complete {0} to unlock",
                            String.Join(", ", selectedModel.UnlockRequirementIds
							.FindAll(id => !CurrentState.Instance.Player.PlayerStats.FinishedArenas.Contains(id))
							.Select(
                                mapId => "\"" + EntitiesStore.Instance.Levels[mapId].Name + "\""
                            ).ToArray()));
                    }
                    DBG.Log("Selected Arena: {0}", CurrentState.Instance.CurrentArena.Id);
                }
            }
        }

        private bool CheckArenaPlayable(LevelModel selectedModel)
        {
            var playerFinished = CurrentState.Instance.Player.PlayerStats.FinishedArenas;
            foreach (String mapId in selectedModel.UnlockRequirementIds)
            {
                if (!playerFinished.Contains(mapId))
                {
                    return false;
                }
            }

            return true;
        }

        public void _BackToMainMenu()
        {
            TransitionUtil.StartTransitionTo(SceneIds.SCENE_MENU_ID);
        }
        private void SetArenaModelUI(LevelModel arena)
        {
            if (arena != null)
            {
                arenaNameText.text = arena.Name;
                arenaThumbnail.sprite = arena.Thumbnail;
                arenaSnapshot.sprite = arena.Snapshot;
                arenaDescriptionText.text = arena.Description;
                arenaPropsText.text = UIUtils.ApplyPropsToTemplate(LEVEL_INFO_TEMPLATE, MapLevelInfo(arena));
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
            CurrentArenaIndex = UIUtils.SafeIndex(currentArenaIndex + 1, arenaModels);
        }
        public void PrevArenaButtonClick()
        {
            CurrentArenaIndex = UIUtils.SafeIndex(currentArenaIndex - 1, arenaModels);
        }

        // Use this for initialization
        void Start()
        {
            EntitiesStore.Instance.GetStatus();
            arenaModels = new List<LevelModel>(EntitiesStore.Instance.Levels.Values);
            arenaModels.Sort((a, b) => a.StageNumber.CompareTo(b.StageNumber));
            currentArenaIndex = 999;
            CurrentArenaIndex = 0;
            playerAvatarImage.sprite = CurrentState.Instance.Player.Character.Avatar;
        }


        public void PlaySelectedArena()
        {
            var arena = CurrentState.Instance.CurrentArena;
            if (arena != null)
            {
                if ( CurrentState.Instance.CurrentDialogueScenesBefore.ContainsKey(arena.Id)) {
                    var dialogue = CurrentState.Instance.CurrentDialogueScenesBefore[arena.Id];
                    TransitionUtil.StartTransitionTo(SceneIds.SCENE_DIALOGUE_ID, 
                        new Dictionary<string, object>{ 
                            {TransitionParams.PARAM_DIALOGUE_SCENE_ID, dialogue.Id},
                            {TransitionParams.PARAM_DIALOGUE_POSITION, dialogue.Position} });
                } else 
                {
                    TransitionUtil.StartTransitionTo(SceneIds.SCENE_ARENA_ID);
                }
            }
        }

    }

}
