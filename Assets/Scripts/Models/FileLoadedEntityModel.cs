using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TankArena.Constants;
using EK = TankArena.Constants.EntityKeys;
using TankArena.Utils;
using UnityEngine;
using SimpleJSON;

namespace TankArena.Models
{
    abstract class FileLoadedEntityModel
    {
        public String Id
        {
            get
            {
                return (string)properties[EK.EK_ID];
            }
        }

        public String Name
        {
            get
            {
                return (string)properties[EK.EK_NAME];
            }
        }

        protected Dictionary<String, object> properties;

        public FileLoadedEntityModel(string filePath)
        {
            properties = new Dictionary<string, object>();
            var jsonText = Resources.Load<TextAsset>(filePath);
            var json = JSON.Parse(jsonText.text);
            LoadPropertiesFromJSON(json);
        }

        protected virtual void LoadPropertiesFromJSON(JSONNode json)
        {
            properties[EK.EK_ID] = json[EK.EK_ID].Value;
            properties[EK.EK_NAME] = json[EK.EK_NAME].Value;
        }

        protected object ResolveSpecialContent(string content)
        {
            if (String.IsNullOrEmpty(content))
            {
                return null;
            }
            return SpecialContentResolver.Resolve(content);
        }
    }
}
