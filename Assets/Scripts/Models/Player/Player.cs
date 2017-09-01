using UnityEngine;
using PP = TankArena.Constants.PlayerPrefsKeys;
using TankArena.Utils;
using System.IO;
using SimpleJSON;
using TankArena.Models.Characters;
using System;
using System.Collections.Generic;

namespace TankArena.Models
{
    public class Player
    {
        public string Name;
        private string saveLocation;
        public float Health;
        public float Cash;
        public Tank.Tank CurrentTank;
        public PlayableCharacter Character;
        public CharacterStats CurrentStats;
        public PlayerStats PlayerStats;
        public AbstractCharacterGoal CharacterGoal;

        //null-safe way to get progress of goal
        public float GetGoalProgress()
        {
            if (CharacterGoal == null)
            {
                return 0.0f;
            }
            else
            {
                return CharacterGoal.GetProgress();
            }
        }

        public Player(string saveFileLocation)
        {
            this.saveLocation = saveFileLocation;
            this.PlayerStats = new PlayerStats();
        }

        public static Player LoadPlayerFromLocation(string location)
        {
            var player = new Player(location);
            PopulatePlayerFromLocation(player, location);
            return player;
        }

        private static void PopulatePlayerFromLocation(Player player, string location)
        {
            try
            {
                var jsonText = File.ReadAllText(location);
                try
                {
                    JSONNode json = JSONNode.Parse(jsonText);

                    player.Name = json[PP.PP_NAME].Value;
                    player.Health = json[PP.PP_HEALTH].AsFloat;
                    player.Cash = json[PP.PP_CASH].AsFloat;
                    player.Character = EntitiesStore.Instance.Characters[(json[PP.PP_CHARACTER].Value)];
                    player.CurrentTank = Tank.Tank.FromCode(json[PP.PP_TANK].Value);
                    player.CurrentStats = CharacterStats.ParseJSONBody(json[PP.PP_CHAR_STATS].AsObject);
                    player.PlayerStats = PlayerStats.FromJSON(json[PP.PP_STATISTICS].AsObject);
                    player.CharacterGoal = (AbstractCharacterGoal)Activator.CreateInstance(player.Character.CharacterGoalType);
                    float progress = json[PP.PP_GOAL_PROGRESS].AsFloat;
                    player.CharacterGoal.Init(progress);

                    player.CharacterGoal.UpdateProgress(player.PlayerStats);

                }
                catch (Exception ex)
                {
                    //save game corrupt
                    DBG.Log("Savegame at {0} had corrupt data: {1}, making new character! Exception: {2}",
                         location, jsonText, ex);
                }
            }
            catch (FileNotFoundException ex)
            {
                //save game doesnt exist yet, will make new save data later
                DBG.Log("No savegame found at {0}, making new character! Exception: {1}",
                    location, ex);
            }
        }

        public static void LoadCurrentPlayer()
        {
            var player = CurrentState.Instance.Player;
            PopulatePlayerFromLocation(player, player.saveLocation);
        }

        public static void SaveCurrentPlayer()
        {
            var player = CurrentState.Instance.Player;

            DBG.Log("Saving player: {0}", player);
            //take the current player customizations and save them into the preferences
            JSONClass saveJson = new JSONClass();

            //player always has a name
            saveJson.Add(PP.PP_NAME, player.Name);
            saveJson.Add(PP.PP_HEALTH, player.Health.ToString());
            saveJson.Add(PP.PP_CASH, player.Cash.ToString());
            if (player.Character != null)
            {
                saveJson.Add(PP.PP_CHARACTER, player.Character.Id);
            }
            if (player.CurrentTank != null)
            {
                saveJson.Add(PP.PP_TANK, player.CurrentTank.ToCode());
            }
            if (player.CurrentStats != null)
            {
                saveJson.Add(PP.PP_CHAR_STATS, player.CurrentStats.ToJSON());
            }
            if (player.PlayerStats != null)
            {
                saveJson.Add(PP.PP_STATISTICS, player.PlayerStats.ToJSON());
            }
            
            saveJson.Add(PP.PP_GOAL_PROGRESS, player.GetGoalProgress().ToString());
             

            //persist the file
            File.WriteAllText(player.saveLocation, saveJson.ToString());
        }
    }
}
