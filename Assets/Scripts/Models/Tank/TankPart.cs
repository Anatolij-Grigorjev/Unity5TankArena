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
using TankArena.Controllers;
using MovementEffects;

namespace TankArena.Models.Tank
{
    public abstract class TankPart : ShopPurchaseableEntityModel
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
        /// Component identifying image when purchased and shown on tank
        /// </summary>
        public Sprite GarageItem
        {
            get
            {
                return (Sprite)properties[EK.EK_GARAGE_ITEM_IMAGE];
            }
        }
        public Sprite[] Sprites
        {
            get
            {
                return (Sprite[])properties[EK.EK_ENTITY_SPRITESHEET];
            }
        }
        public int ActiveSprites
        {
            get 
            {
                return (int)properties[EK.EK_ACTIVE_SPRITES];
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

        public TankPart(TankPart model): base(model) {
            
        }

        protected override IEnumerator<float> _LoadPropertiesFromJSON(JSONNode json)
        {
            var handle = Timing.RunCoroutine(base._LoadPropertiesFromJSON(json));
            yield return Timing.WaitUntilDone(handle);
            properties[EK.EK_ON_TANK_POSITION] = ResolveSpecialContent(json[EK.EK_ON_TANK_POSITION].Value);
            properties[EK.EK_MASS] = json[EK.EK_MASS].AsFloat;
            properties[EK.EK_GARAGE_ITEM_IMAGE] = ResolveSpecialContent(json[EK.EK_GARAGE_ITEM_IMAGE].Value);
            if (GarageItem == null)
            {
                properties[EK.EK_GARAGE_ITEM_IMAGE] = ResolveSpecialContent(json[EK.EK_SHOP_ITEM_IMAGE].Value);
            }
            properties[EK.EK_ENTITY_SPRITESHEET] = ResolveSpecialContent(json[EK.EK_ENTITY_SPRITESHEET].Value);
            properties[EK.EK_ACTIVE_SPRITES] = json[EK.EK_ACTIVE_SPRITES].AsInt > 0? json[EK.EK_ACTIVE_SPRITES].AsInt : 1;
            properties[EK.EK_COLLISION_BOX] = ResolveSpecialContent(json[EK.EK_COLLISION_BOX].Value);

            yield return 0.0f;
        }

        public virtual void SetRendererSprite(SpriteRenderer renderer, int spriteIndex)
        {
            if (renderer != null && Sprites != null)
            {
                renderer.sprite = Sprites[spriteIndex];
            }
        }
        public virtual void SetColliderBounds(PolygonCollider2D collider)
        {
            if (collider != null)
            {
                collider.pathCount = 1;
                var cb = CollisionBox;
                //set the collider based on chasis
                collider.SetPath(0, new Vector2[]
                {
                    cb.position + new Vector2(0, 0),
                    cb.position + new Vector2(0, cb.height),
                    cb.position + new Vector2(cb.width, cb.height),
                    cb.position + new Vector2(cb.width, 0)
                });
            }
        }
        private void DoRigidBody(Rigidbody2D rigidBody)
        {
            if (rigidBody != null)
            {
                SetRigidBodyProps(rigidBody);
            }
        }
        public virtual void SetRigidBodyProps(Rigidbody2D rigidBody)
        {
            rigidBody.mass = Mass;
        }

        public virtual void SetDataToController<T>(BaseTankPartController<T> controller) where T: TankPart
        {
            //using ref to ensure parameter is never null would be nice, but it gets passed as "this", so its ok
            SetRendererSprite(controller.partRenderer, 0);
            SetColliderBounds(controller.partCollider);
            DoRigidBody(controller.partRigidBody);
            if (OnTankPosition != null)
            {
                OnTankPosition.CopyToTransform(controller.transform);
            }
        }

        public virtual String ShopDescription()
        {
            return String.Format("Tank Part: {0}", Name);
        }

    }
}
