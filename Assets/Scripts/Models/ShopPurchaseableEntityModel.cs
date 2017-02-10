using System;
using System.Collections.Generic;
using MovementEffects;
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

        protected override IEnumerator<float> _LoadPropertiesFromJSON(JSONNode json)
        {
            var handle = Timing.RunCoroutine(base._LoadPropertiesFromJSON(json));
            yield return Timing.WaitUntilDone(handle);
            properties[EK.EK_PRICE] = json[EK.EK_PRICE].AsFloat;
            properties[EK.EK_SHOP_ITEM_IMAGE] = ResolveSpecialContent(json[EK.EK_SHOP_ITEM_IMAGE].Value);

            yield return 0.0f;
        }

    }
}