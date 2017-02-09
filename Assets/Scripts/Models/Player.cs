using UnityEngine;
using PP = TankArena.Constants.PlayerPrefsKeys;
using TankArena.Utils;
using System.IO;
using SimpleJSON;
using TankArena.Models.Characters;
using System;

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

        public Player(string saveFileLocation)
        {
            this.saveLocation = saveFileLocation;
            try 
            {
                var jsonText = File.ReadAllText(this.saveLocation);
                try 
                {
                    JSONNode json = JSONNode.Parse(jsonText);

                    Name = json[PP.PP_NAME].Value;
                    Health = json[PP.PP_HEALTH].AsFloat;
                    Cash = json[PP.PP_CASH].AsFloat;
                    Character = EntitiesStore.Instance.Characters[(json[PP.PP_CHARACTER].Value)];
                    CurrentTank = Tank.Tank.FromCode(json[PP.PP_TANK].Value);

                } catch (Exception ex) 
                {
                    //save game corrupt
                    DBG.Log("Savegame at {0} had corrupt data: {1}, making new character! Exception: {2}",
                         this.saveLocation, jsonText, ex);
                }
            } catch (FileNotFoundException ex)
            {
                //save game doesnt exist yet, will make new save data later
                DBG.Log("No savegame found at {0}, making new character! Exception: {1}",
                    this.saveLocation, ex);
            }

        }

		///<summary>
		///Loads from player prefs and puts into the EntityStore
		///</summary>
		public static Player LoadFromPlayerPrefs(string saveFileLocation)
        {
			Player player = new Player(saveFileLocation);
			return player;
        }

		public static void SaveCurrentPlayer()
        {
			var player = CurrentState.Instance.Player;
            //take the current player customizations and save them into the preferences
            JSONClass saveJson = new JSONClass();
            
            saveJson.Add(PP.PP_NAME, player.Name);
            saveJson.Add(PP.PP_HEALTH, player.Health.ToString());
            saveJson.Add(PP.PP_CASH, player.Cash.ToString());
            saveJson.Add(PP.PP_CHARACTER, player.Character.Id);
            saveJson.Add(PP.PP_TANK, player.CurrentTank.ToCode());

            //persist the file
            File.WriteAllText(player.saveLocation, saveJson.ToString());
        }
	}
}
