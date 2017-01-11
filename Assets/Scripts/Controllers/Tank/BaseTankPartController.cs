﻿using System;
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
        public PolygonCollider2D partCollider;
        [HideInInspector]
        public Rigidbody2D partRigidBody;
        [HideInInspector]
        public GameObject parentObject;
        private bool isAwake = false;
        public T Model
        {
            get
            {
                return data;
            }
            set
            {
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
