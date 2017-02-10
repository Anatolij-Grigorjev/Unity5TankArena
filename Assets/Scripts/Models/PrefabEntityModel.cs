using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MovementEffects;
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

        protected override IEnumerator<float> _LoadPropertiesFromJSON(JSONNode json)
        {
            var handle = Timing.RunCoroutine(base._LoadPropertiesFromJSON(json));
            yield return Timing.WaitUntilDone(handle);
            properties[EK.EK_ENTITY_PREFAB] = ResolveSpecialContent(json[EK.EK_ENTITY_PREFAB].Value);

            yield return 0.0f;
        }
    }
}
