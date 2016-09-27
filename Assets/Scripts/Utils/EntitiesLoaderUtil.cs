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
        private EntitiesLoaderUtil() { }

        public static void loadAllEntitesAtPath<T>(string basePath, Func<string, T> generator, Dictionary<String, T> consumer) 
            where T : FileLoadedEntityModel
        {
            if (consumer == null) { consumer = new Dictionary<string, T>(); };
            foreach (String fileName in Directory.GetFiles(basePath, "*.json", SearchOption.AllDirectories))
            {
                var entity = generator(fileName);
                consumer.Add(entity.Id, entity);
            }
            Debug.Log(String.Format("Loaded {0} entites of type {1}", consumer.Count, typeof(T).FullName));
        }
    }
}
