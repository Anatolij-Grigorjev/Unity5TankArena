using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;

namespace TankArena.Controllers
{
    public class TankTurretController : MonoBehaviour
    {

        private TankTurret turret;

        public TankTurret Turret
        {
            get
            {
                return turret;
            }
            set
            {
                turret = value;
                turret.OnTankPosition.CopyToTransform(transform);
                SetDefaultSprite();
            }
        }

        private SpriteRenderer spriteRenderer;

        // Use this for initialization
        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            SetDefaultSprite();
        }

        private void SetDefaultSprite()
        {
            if (spriteRenderer != null && turret != null)
            {
                spriteRenderer.sprite = turret.Sprites[0];
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
