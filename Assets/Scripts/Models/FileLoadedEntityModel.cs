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
        /// <summary>
        /// The unique game-wide item identifier code, used in cache storage
        /// </summary>
        public String Id
        {
            get
            {
                return (string)properties[EK.EK_ID];
            }
        }

        /// <summary>
        /// Item name, presented in shop and descriptions
        /// </summary>
        public String Name
        {
            get
            {
                return (string)properties[EK.EK_NAME];
            }
        }
        protected String EntityKey
        {
            get
            {
                return "entity";
            }
        }


        protected Dictionary<String, object> properties;

        public FileLoadedEntityModel(string filePath)
        {
            Debug.Log("Trying to load entity at path: " + filePath);
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

        /// <summary>
        /// Handle strings that need to be deserialized into a custom type
        /// </summary>
        /// <param name="content">The string to deserialize</param>
        /// <returns>The content, string itself if deserialization fails</returns>
        protected object ResolveSpecialContent(string content)
        {
            if (String.IsNullOrEmpty(content))
            {
                return null;
            }
            return SpecialContentResolver.Resolve(content);
        }

        //TODO: all entites also need a static FromCode method
        //to reverse these effects
        //static methods cannot be used in inheritance, so need another way for it
        protected virtual String ToCode()
        {
            return String.Format("{0}={1}", EntityKey, Id);
        }

    }
}
