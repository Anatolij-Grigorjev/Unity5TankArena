using UnityEngine;
using System.Collections;
using PP = TankArena.Constants.PlayerPrefsKeys;
using TankArena.Utils;
using TankArena.Models.Characters;

namespace TankArena.Models
{
	public class Player 
	{

		public float Health;
		public float Cash;
		public Tank.Tank CurrentTank;
		public PlayableCharacter Character;

		///<summary>
		///Loads from player prefs and puts into the EntityStore
		///</summary>
		public static void LoadFromPlayerPrefs()
        {
			Player player = new Player();
            //construct player character and tank from encoded keys of ids
            //tank encoded as key-value map, key type of component, value is entity id

            //player always has a selected character. this code will come from main menu later, for now hardcoded
            var characterCode = PlayerPrefs.HasKey(PP.PP_CHARACTER) ? PlayerPrefs.GetString(PP.PP_CHARACTER) : "lugnut";
            //filter search to specific map because its faster AND for type safety
            var character = EntitiesStore.Instance.Characters[characterCode];

            //these keys might be absent if new game
            var tankCode = PlayerPrefs.HasKey(PP.PP_TANK) ? PlayerPrefs.GetString(PP.PP_TANK) : character.StartingTankCode;
			player.CurrentTank = Tank.Tank.FromCode(tankCode);
            player.Health = PlayerPrefs.HasKey(PP.PP_HEALTH) ? PlayerPrefs.GetFloat(PP.PP_HEALTH) : character.StartingHealth;
            player.Cash = PlayerPrefs.HasKey(PP.PP_CASH) ? PlayerPrefs.GetFloat(PP.PP_CASH) : character.StartingCash;
			player.Character = character;

			EntitiesStore.Instance.Player = player;
			EntitiesStore.Instance.CurrentTank = player.CurrentTank;
        }

		public static void SaveToPlayerPrefs()
        {
			var player = EntitiesStore.Instance.Player;
            //take the current player customizations and save them into the preferences
            PlayerPrefs.SetString(PP.PP_CHARACTER, player.Character.Id);
            PlayerPrefs.SetString(PP.PP_TANK, player.CurrentTank.ToCode());
            PlayerPrefs.SetFloat(PP.PP_HEALTH, player.Health);
            PlayerPrefs.SetFloat(PP.PP_CASH, player.Cash);

            //flush the prefs
            PlayerPrefs.Save();
        }
	}
}
