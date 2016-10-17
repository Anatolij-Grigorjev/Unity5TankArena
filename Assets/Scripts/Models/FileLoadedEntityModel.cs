using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TankArena.Constants;
using EK = TankArena.Constants.EntityKeys;
using SK = TankArena.Constants.ItemSeriazlizationKeys;
using DBG = TankArena.Utils.Debug;
using TankArena.Utils;
using UnityEngine;
using SimpleJSON;

namespace TankArena.Models
{
    public abstract class FileLoadedEntityModel
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
        public String EntityKey
        {
            get
            {
                return SK.SK_ENTITY;
            }
        }


        protected Dictionary<String, object> properties;

        private static String CULL_PATH = "Resources" + Path.DirectorySeparatorChar;
        private static int CULL_LENGTH = CULL_PATH.Length;

        public FileLoadedEntityModel(string filePath)
        {
            DBG.Log("Trying to load entity at path: {0}", filePath);
            properties = new Dictionary<string, object>();
            var relativePath = filePath.Substring(filePath.IndexOf(CULL_PATH) + CULL_LENGTH);
            relativePath = relativePath.Substring(0, relativePath.LastIndexOf("."));
            DBG.Log("Transformed path for relative loading: {0}", relativePath);
            var jsonText = Resources.Load<TextAsset>(relativePath) as TextAsset;
            DBG.Log("Loaded Json Text for entity: {0}", jsonText);
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
            var resolved = SpecialContentResolver.Resolve(content);

            return resolved;
        }

    }
}
