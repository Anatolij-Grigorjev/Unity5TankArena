using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using TankArena.Models;
using TankArena.Models.Tank;
using TankArena.Models.Tank.Weapons;
using TankArena.Models.Characters;

namespace TankArena.Utils
{
    class EntitiesStore : Singleton<EntitiesStore>
    {
        protected EntitiesStore() { }

        public Dictionary<String, FileLoadedEntityModel> Entities { get { return loadedEntities; }  }
        public Dictionary<String, PlayableCharacter> Characters { get { return loadedCharacters; } }
        public Dictionary<String, TankPart> TankParts { get { return loadedTankParts; } }
        public Dictionary<String, BaseWeapon> Weapons { get { return loadedWeapons; } }

        private Dictionary<String, FileLoadedEntityModel> loadedEntities;
        private Dictionary<String, PlayableCharacter> loadedCharacters;
        private Dictionary<String, TankPart> loadedTankParts;
        private Dictionary<String, BaseWeapon> loadedWeapons;

        public void Awake()
        {
            loadedEntities = new Dictionary<string, FileLoadedEntityModel>();

            EntitiesLoaderUtil.loadAllEntitesAtPath<PlayableCharacter>("", path => { return new PlayableCharacter(path); }, loadedCharacters);
            loadedCharacters.ToList().ForEach(x => loadedEntities.Add(x.Key, x.Value));
            
        }

        public void GetStatus()
        {
            //print amounts of loaded entities
            //as status info. Good for eager lazy loading
            Debug.Log(String.Format("Loaded total of {0} entities.\n", loadedEntities.Count));
        }

       
    }
}
