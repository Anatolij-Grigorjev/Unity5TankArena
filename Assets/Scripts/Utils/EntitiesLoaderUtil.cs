using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TankArena.Models;
using UnityEngine;

namespace TankArena.Utils
{
    class EntitiesLoaderUtil
    {

        private static readonly String BASE_DATA_PATH = Path.Combine(Application.dataPath, "Resources");

        private EntitiesLoaderUtil() { }

        public static void loadAllEntitesAtPath<T>(string baseLoadPath, Func<string, T> generator, Dictionary<String, T> consumer) 
            where T : FileLoadedEntityModel
        {
            var fullPath = Path.Combine(BASE_DATA_PATH, baseLoadPath);
            Debug.Log(String.Format("Searching for loadable entities of type {0} at {1}", typeof(T).FullName, fullPath));
            foreach (String fileName in Directory.GetFiles(fullPath, "*.json", SearchOption.AllDirectories))
            {
                var entity = generator(fileName);
                consumer.Add(entity.Id, entity);
            }
            Debug.Log(String.Format("Loaded {0} entites of type {1}", consumer.Count, typeof(T).FullName));
        }
    }
}
