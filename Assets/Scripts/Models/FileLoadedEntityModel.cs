using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TankArena.Constants;

namespace TankArena.Models
{
    abstract class FileLoadedEntityModel
    {
        public String Id
        {
            get
            {
                return (string)properties[EntityKeys.EK_ID];
            }
        }

        public String Name
        {
            get
            {
                return (string)properties[EntityKeys.EK_NAME];
            }
        }

        protected Dictionary<String, Object> properties;

        public FileLoadedEntityModel(string filePath)
        {
            LoadPropertiesFromJSON(filePath);
        }

        private void LoadPropertiesFromJSON(string filePath)
        {

        }
    }
}
