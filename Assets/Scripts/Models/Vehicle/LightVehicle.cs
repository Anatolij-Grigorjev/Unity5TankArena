using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

namespace TankArena.Models.Vehicle
{
    public class LightVehicle: PrefabEntityModel
    {
        public LightVehicle(string path): base(path)
        {

        }


        protected override void LoadPropertiesFromJSON(JSONNode json)
        {
            base.LoadPropertiesFromJSON(json);


        }
    }

}
