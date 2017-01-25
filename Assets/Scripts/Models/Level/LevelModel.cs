using System;
using UnityEngine;
using TankArena.Models;
using EK = TankArena.Constants.EntityKeys;
using System.Collections.Generic;
using SimpleJSON;

namespace TankArena.Models.Level 
{
    public class LevelModel : FileLoadedEntityModel
    {
        public Sprite Thumbnail
        {
            get 
            {
                return (Sprite)properties[EK.EK_THUMBNAIL];
            }
        }
        public int TotalEnemies
        {
            get 
            {
                return (int)properties[EK.EK_TOTAL_ENEMIES];
            }
        }
        public List<string> EnemyTypes
        {
            get 
            {
                return (List<string>)properties[EK.EK_ENEMY_TYPES];
            }
        }
        public Vector3 PlacementPoint
        {
            get 
            {
                return (Vector3)properties[EK.EK_PLACEMENT_POINT];
            }
        }

        
        public LevelModel(String filepath): base(filepath)
        {

        }

        protected override void LoadPropertiesFromJSON(JSONNode json)
        {
            base.LoadPropertiesFromJSON(json);

            properties[EK.EK_THUMBNAIL] = ResolveSpecialContent(json[EK.EK_THUMBNAIL].Value);
            properties[EK.EK_TOTAL_ENEMIES] = json[EK.EK_TOTAL_ENEMIES].AsInt;
            var list = new List<string>();
            foreach(JSONNode node in json[EK.EK_ENEMY_TYPES].AsArray)
            {
                list.Add(node.Value);
            }
            properties[EK.EK_ENEMY_TYPES] = list;
            properties[EK.EK_PLACEMENT_POINT] = ResolveSpecialContent(json[EK.EK_PLACEMENT_POINT].Value);
        }


    }
}