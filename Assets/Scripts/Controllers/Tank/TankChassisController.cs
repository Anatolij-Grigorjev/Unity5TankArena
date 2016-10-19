using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;
using System;

namespace TankArena.Controllers
{
    public class TankChassisController : MonoBehaviour
    {

        private TankChassis chassis;
        public TankEngineController engineController;
        public TankTracksController tracksController;

        private SpriteRenderer spriteRenderer;

        public TankChassis Chassis
        {
            get
            {
                return chassis;
            }
            set
            {
                chassis = value;
                SetDefaultChassisSprite();
                engineController.Engine = chassis.Engine;
                tracksController.Tracks = chassis.Tracks;
            }
        }

        private void SetDefaultChassisSprite()
        {
            if (spriteRenderer != null && chassis != null)
            {
                spriteRenderer.sprite = chassis.Sprites[0];
            }
        }

        // Use this for initialization
        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            SetDefaultChassisSprite();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
