using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;
using UnityEngine;
using EK = TankArena.Constants.EntityKeys;

namespace TankArena.Models
{
    public abstract class PrefabEntityModel: FileLoadedEntityModel
    {


        public GameObject EntityPrefab
        {
            get
            {
                return (GameObject)properties[EK.EK_ENTITY_PREFAB];
            }
        }


        public PrefabEntityModel(string path): base(path)
        {

        }

        protected override void LoadPropertiesFromJSON(JSONNode json)
        {
            base.LoadPropertiesFromJSON(json);

            properties[EK.EK_ENTITY_PREFAB] = ResolveSpecialContent(json[EK.EK_ENTITY_PREFAB].Value);
        }
    }
}
