using UnityEngine;
using System.Collections;
using TankArena.Models.Characters;
using TankArena.Models.Tank;


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
            //TODO: check keys against list of constants 
            //construct player character and tank from encoded keys of ids
            //tank encoded as key-value map, key type of component, value is entity id
            //tank needs custom string serializer/deserializer
        }
    }
}
