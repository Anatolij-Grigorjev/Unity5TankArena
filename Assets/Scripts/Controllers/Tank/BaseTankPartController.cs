using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TankArena.Models.Tank;

namespace TankArena.Controllers
{
    public abstract class BaseTankPartController<T>: MonoBehaviour where T: TankPart
    {
        private T data;

        [HideInInspector]
        public SpriteRenderer partRenderer;
        [HideInInspector]
        public BoxCollider2D partCollider;
        [HideInInspector]
        public Rigidbody2D partRigidBody;

        public T Model
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
                data.SetDataToController(this);
            }
        }

        public virtual void Awake()
        {
            partCollider = GetComponent<BoxCollider2D>();
            partRenderer = GetComponent<SpriteRenderer>();
            partRigidBody = GetComponent<Rigidbody2D>();
            if (data != null)
            { 
                data.SetDataToController(this);
            }
        }

    }
}
