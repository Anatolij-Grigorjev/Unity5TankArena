using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;
using System;

namespace TankArena.Controllers
{
    public class TankChassisController : MonoBehaviour
    {

        private TankChassis chassisData;
        public TankEngineController engineController;
        public TankTracksController tracksController;

        private SpriteRenderer chassisRenderer;
        private BoxCollider2D chassisCollider;

        public TankChassis Chassis
        {
            get
            {
                return chassisData;
            }
            set
            {
                chassisData = value;
                chassisData.OnTankPosition.CopyToTransform(transform);
                chassisData.SetRendererSprite(chassisRenderer, 0);
                chassisData.SetColliderBounds(chassisCollider);
                engineController.Engine = chassisData.Engine;
                tracksController.Tracks = chassisData.Tracks;
            }
        }

        // Use this for initialization
        void Awake()
        {
            chassisRenderer = GetComponent<SpriteRenderer>();
            chassisCollider = GetComponent<BoxCollider2D>();
            if (chassisData != null)
            {
                chassisData.SetRendererSprite(chassisRenderer, 0);
                chassisData.SetColliderBounds(chassisCollider);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
