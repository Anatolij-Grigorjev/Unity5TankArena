using System;
using UnityEngine;

namespace TankArena.Utils
{
    class ObjectsPool: MonoBehavior
    {
        public GameObject pooledPrefab;
        [HideInInspector]
        public List<GameObject> Instances;
        public int instancesCount;

        public void Start() 
        {
            Instances = new List<GameObject>(instancesCount);
            for ( int i = 0 ; i < prefabInstances; i++ )
            {
                var instance = Instantiate(pooledPrefab) as GameObject;
                instance.SetActive(false);
                Instances.Add(instance);
            }
        }

        public GameObject GetFirsReadyInstance(boolean expand = false)
        {
            for (int i = 0; i < instancesCount; i++)
            {
                if (!Instances[i].activeInHierarchy)
                {
                    return Instances[i];
                }
            }
            if (!expand)
            {
                return null;
            } else
            {
                //expanding Instnaces count for the needs of the people
                instancesCount++;
                var instance = Instantiate(pooledPrefab);
                instance.SetActive(false);
                Instances.Add(instance);

                return instance;
            }
        }

    }
}