using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MovementEffects;
using TankArena.Constants;
using TankArena.Models;
using APP = UnityEngine.Application;

namespace TankArena.Utils
{
    class EntitiesLoaderUtil
    {

        public static readonly String BASE_DATA_PATH = Path.Combine(APP.dataPath, "Resources");

        private EntitiesLoaderUtil() { }

        public static IEnumerator<float> _LoadAllEntitesAtPath<T>(string baseLoadPath, Func<string, T> generator, Dictionary<String, T> consumer) 
            where T : FileLoadedEntityModel
        {
            var fullPath = Path.Combine(BASE_DATA_PATH, baseLoadPath);
            DBG.Log("Searching for loadable entities of type {0} at {1}", typeof(T).FullName, fullPath);
            foreach (String fileName in Directory.GetFiles(fullPath, "*.json", SearchOption.AllDirectories))
            {
                var entity = generator(fileName);
                consumer.Add(entity.Id, entity);
                //yield after every entity loaded
                yield return Timing.WaitForSeconds(LoadingParameters.LOADING_COOLDOWN_SHORT);
            }
            DBG.Log("Loaded {0} entites of type {1}", consumer.Count, typeof(T).FullName);

            yield return 0.0f;
        }
    }
}
