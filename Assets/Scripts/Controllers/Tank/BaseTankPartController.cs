using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TankArena.Models.Tank;
using TankArena.Utils;

namespace TankArena.Controllers
{
    public abstract class BaseTankPartController<T>: MonoBehaviour where T: TankPart
    {
        private T data;

        [HideInInspector]
        public SpriteRenderer partRenderer;
        [HideInInspector]
        public PolygonCollider2D partCollider;
        [HideInInspector]
        public Rigidbody2D partRigidBody;
        [HideInInspector]
        public GameObject parentObject;
        public TankController tankController;
        private bool isAwake = false;
        public T Model
        {
            get
            {
                return data;
            }
            set
            {
                DBG.Log("Setting {0} on {1}", typeof(T).Name, GetType().Name);
                data = value;
                if (isAwake)
                {
                    data.SetDataToController(this);
                }
            }
        }

        public virtual void Awake()
        {
            partCollider = GetComponent<PolygonCollider2D>();
            partRenderer = GetComponent<SpriteRenderer>();
            partRigidBody = GetComponent<Rigidbody2D>();
            if (data != null)
            { 
                data.SetDataToController(this);
            }
            
            isAwake = true;
        }

    }
}
