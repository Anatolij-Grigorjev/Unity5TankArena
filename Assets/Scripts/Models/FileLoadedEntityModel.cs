using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TankArena.Constants;
using UnityEngine;

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

        protected Dictionary<String, object> properties;

        public FileLoadedEntityModel(string filePath)
        {
            LoadPropertiesFromJSON(filePath);
        }

        protected void LoadPropertiesFromJSON(string filePath)
        {
            var jsonText = Resources.Load<TextAsset>(filePath);
            
        }

        protected object ResolveSpecialContent(string content)
        {

        }
    }
}
