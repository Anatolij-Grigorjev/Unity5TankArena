using System;
using SimpleJSON;
using UnityEngine;
using EK = TankArena.Constants.EntityKeys;

namespace TankArena.Models
{
    public class ShopPurchaseableEntityModel: FileLoadedEntityModel
    {

        ///<summary>
        /// Item price, presented in shops
        /// </summary>
        public float Price
        {
            get 
            {
                return (float)properties[EK.EK_PRICE];
            }
        }
        /// <summary>
        /// Component identifying image in shop view
        /// </summary>
        public Sprite ShopItem
        {
            get
            {
                return (Sprite)properties[EK.EK_SHOP_ITEM_IMAGE];
            }
        }


        public ShopPurchaseableEntityModel(String path): base(path) {}
        public ShopPurchaseableEntityModel(ShopPurchaseableEntityModel model) : base(model) {}

        protected override void LoadPropertiesFromJSON(JSONNode json)
        {
            base.LoadPropertiesFromJSON(json); 
            
            properties[EK.EK_PRICE] = json[EK.EK_PRICE].AsFloat;
            properties[EK.EK_SHOP_ITEM_IMAGE] = ResolveSpecialContent(json[EK.EK_SHOP_ITEM_IMAGE].Value);
        }

    }
}