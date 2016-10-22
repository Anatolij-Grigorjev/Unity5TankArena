using UnityEngine;
using System.Collections;
using TankArena.Models.Characters;
using TankArena.Models.Tank;
using PP = TankArena.Constants.PlayerPrefsKeys;
using TankArena.Utils;

namespace TankArena.Controllers
{
    public class PlayerController : MonoBehaviour
    {

        private PlayableCharacter character;
        public TankController tankController;

        public float Health { get; set; }
        public float Cash { get; set; }

        // Use this for initialization
        void Awake()
        {
            LoadFromPlayerPrefs();
        }

        // Update is called once per frame
        void Update()
        {
            //TODO: write some control up in this bitach that wires to the tank service

        }

        private void SaveToPlayerPrefs()
        {
            //take the current player customizations and save them into the preferences
            PlayerPrefs.SetString(PP.PP_CHARACTER, character.Id);
            PlayerPrefs.SetString(PP.PP_TANK, tankController.Tank.ToCode());
            PlayerPrefs.SetFloat(PP.PP_HEALTH, Health);
            PlayerPrefs.SetFloat(PP.PP_CASH, Cash);

            //flush the prefs
            PlayerPrefs.Save();
        }

        private void LoadFromPlayerPrefs()
        {

            //construct player character and tank from encoded keys of ids
            //tank encoded as key-value map, key type of component, value is entity id

            //player always has a selected character. this code will come from main menu later, for now hardcoded
            var characterCode = PlayerPrefs.HasKey(PP.PP_CHARACTER) ? PlayerPrefs.GetString(PP.PP_CHARACTER) : "dummy";
            //filter search to specific map because its faster AND for type safety
            character = EntitiesStore.Instance.Characters[characterCode];

            //these keys might be absent if new game
            var tankCode = PlayerPrefs.HasKey(PP.PP_TANK) ? PlayerPrefs.GetString(PP.PP_TANK) : character.StartingTankCode;
            tankController.Tank = Tank.FromCode(tankCode);
            Health = PlayerPrefs.HasKey(PP.PP_HEALTH) ? PlayerPrefs.GetFloat(PP.PP_HEALTH) : character.StartingHealth;
            Cash = PlayerPrefs.HasKey(PP.PP_CASH) ? PlayerPrefs.GetFloat(PP.PP_CASH) : character.StartingCash;
        }
    }
}
