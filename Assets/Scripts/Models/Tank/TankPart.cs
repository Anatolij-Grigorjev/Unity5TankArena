using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EK = TankArena.Constants.EntityKeys;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

namespace TankArena.Models.Tank
{
    abstract class TankPart : FileLoadedEntityModel
    {

        public Transform OnTankPosition
        {
            get
            {
                return (Transform)properties[EK.EK_ON_TANK_POSITION];
            }
        }
        public float Mass
        {
            get
            {
                return (float)properties[EK.EK_MASS];
            }
        }
        public Image ShopItem
        {
            get
            {
                return (Image)properties[EK.EK_SHOP_ITEM_IMAGE];
            }
        }
        public Image GarageItem
        {
            get
            {
                return (Image)properties[EK.EK_GARAGE_ITEM_IMAGE];
            }
        }

        public TankPart(string filePath) : base(filePath)
        {
        }

        protected override void LoadPropertiesFromJSON(JSONNode json)
        {
            base.LoadPropertiesFromJSON(json);

            properties[EK.EK_ON_TANK_POSITION] = ResolveSpecialContent(json[EK.EK_ON_TANK_POSITION].Value);
            properties[EK.EK_MASS] = json[EK.EK_MASS].AsFloat;
            properties[EK.EK_SHOP_ITEM_IMAGE] = ResolveSpecialContent(json[EK.EK_SHOP_ITEM_IMAGE].Value);
            properties[EK.EK_GARAGE_ITEM_IMAGE] = ResolveSpecialContent(json[EK.EK_GARAGE_ITEM_IMAGE].Value);
        }
    }
}
