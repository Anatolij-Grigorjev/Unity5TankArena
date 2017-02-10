using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MovementEffects;
using SimpleJSON;

namespace TankArena.Models.Vehicle
{
    public class LightVehicle: PrefabEntityModel
    {
        public LightVehicle(string path): base(path)
        {

        }


        protected override IEnumerator<float> _LoadPropertiesFromJSON(JSONNode json)
        {
            var handle = Timing.RunCoroutine(base._LoadPropertiesFromJSON(json));
            yield return Timing.WaitUntilDone(handle);
        }
    }

}
