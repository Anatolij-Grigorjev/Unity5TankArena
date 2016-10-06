using UnityEngine;
using System.Collections;
using TankArena.Models.Characters;
using TankArena.Models.Tank;
using PP = TankArena.Const.PlayerPrefsKeys;
using TankArena.Utils;

namespace TankArena.Controllers
{
    public class PlayerController : MonoBehaviour
    {

        private PlayableCharacter character;
        private Tank tank;

        public float Health { get; set; }
        public float Cash { get; set; }

        // Use this for initialization
        void Awake()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LoadFromPlayerPrefs()
        {

            //construct player character and tank from encoded keys of ids
            //tank encoded as key-value map, key type of component, value is entity id
            //tank needs custom string serializer/deserializer

            //player always has a selected character. this code will come from main menu later, for now hardcoded
            var characterCode = PlayerPrefs.HasKey(PP.PP_CHARACTER) ? PlayerPrefs.GetString(PP.PP_CHARACTER) : "dummy";
            character = (PlayableCharacter) EntitiesStore.Instance.Entities[characterCode];

            var tankCode = PlayerPrefs.HasKey(PP.PP_TANK) ? PlayerPrefs.GetString(PP.PP_TANK) : character.StartingTankCode;
            tank = Tank.FromCode(tankCode);

            //these keys might be absent if new game
            Health = PlayerPrefs.HasKey(PP.PP_HEALTH) ? PlayerPrefs.GetFloat(PP.PP_HEALTH) : character.StartingHealth;
            Cash = PlayerPrefs.HasKey(PP.PP_CASH) ? PlayerPrefs.GetFloat(PP.PP_CASH) : character.StartingCash;
        }
    }
}
