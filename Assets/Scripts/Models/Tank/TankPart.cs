using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EK = TankArena.Constants.EntityKeys;
using SK = TankArena.Constants.ItemSeriazlizationKeys;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using TankArena.Utils;

namespace TankArena.Models.Tank
{
    public abstract class TankPart : FileLoadedEntityModel
    {

        /// <summary>
        /// Position of part relative to main tank GO transform
        /// </summary>
        public TransformState OnTankPosition
        {
            get
            {
                return (TransformState)properties[EK.EK_ON_TANK_POSITION];
            }
        }
        /// <summary>
        /// Component physical weight, impacts engine
        /// </summary>
        public float Mass
        {
            get
            {
                return (float)properties[EK.EK_MASS];
            }
        }
        /// <summary>
        /// Component identifying image in shop view
        /// </summary>
        public Image ShopItem
        {
            get
            {
                return (Image)properties[EK.EK_SHOP_ITEM_IMAGE];
            }
        }
        /// <summary>
        /// Component identifying image when purchased and shown on tank
        /// </summary>
        public Image GarageItem
        {
            get
            {
                return (Image)properties[EK.EK_GARAGE_ITEM_IMAGE];
            }
        }
        public Sprite[] Sprites
        {
            get
            {
                return (Sprite[])properties[EK.EK_ENTITY_SPRITESHEET];
            }
        }
        public Rect CollisionBox
        {
            get
            {
                return (Rect)properties[EK.EK_COLLISION_BOX];
            }
        }
        new public String EntityKey
        {
            get
            {
                return SK.SK_TANK_PART;
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
            if (GarageItem == null)
            {
                properties[EK.EK_GARAGE_ITEM_IMAGE] = ResolveSpecialContent(json[EK.EK_SHOP_ITEM_IMAGE].Value);
            }
            properties[EK.EK_ENTITY_SPRITESHEET] = ResolveSpecialContent(json[EK.EK_ENTITY_SPRITESHEET].Value);
            properties[EK.EK_COLLISION_BOX] = ResolveSpecialContent(json[EK.EK_COLLISION_BOX].Value);
        }

        public void SetRendererSprite(SpriteRenderer renderer, int spriteIndex)
        {
            if (renderer != null && Sprites != null)
            {
                renderer.sprite = Sprites[spriteIndex];
            }
        }
        public void SetColliderBounds(BoxCollider2D collider)
        {
            if (collider != null && CollisionBox != null)
            {
                collider.offset = CollisionBox.position;
                collider.size = CollisionBox.size;
            }
        }

    }
}
