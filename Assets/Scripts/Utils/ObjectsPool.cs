using System;
using System.Collections.Generic;
using TankArena.Controllers.Weapons;
using TankArena.Models.Weapons;
using UnityEngine;

namespace TankArena.Utils
{
    public class ObjectsPool : MonoBehaviour
    {
        public GameObject pooledPrefab;
        [HideInInspector]
        public List<GameObject> Instances;
        public int instancesCount;
        public ProjectileModel propsMap;

        public void Start()
        {
            DBG.Log("Initialising pool with {0} instances!", instancesCount);
            Instances = new List<GameObject>(instancesCount);
            for (int i = 0; i < instancesCount; i++)
            {
                makeInstance();
            }
        }

        public GameObject GetFirsReadyInstance(bool expand = false)
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
            }
            else
            {
                //expanding Instnaces count for the needs of the people
                instancesCount++;
                return makeInstance();
            }
        }

        /// <summary>
        ///Makes an instance, populates its properties from the model and adds it to the list
        /// </summary>
        private GameObject makeInstance()
        {
            var instance = Instantiate(pooledPrefab) as GameObject;
            propsMap.SetDataToController(instance.GetComponent<ProjectileController>());
            instance.SetActive(false);
            Instances.Add(instance);

            return instance;
        }

    }
}