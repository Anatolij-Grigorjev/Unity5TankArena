using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;

namespace TankArena.Controllers
{
    public class TankTurretController : MonoBehaviour
    {

        private TankTurret turretData;

        public TankTurret Turret
        {
            get
            {
                return turretData;
            }
            set
            {
                turretData = value;
                turretData.OnTankPosition.CopyToTransform(transform);
                turretData.SetRendererSprite(turretRenderer, 0);
                turretData.SetColliderBounds(turretCollider);
            }
        }

        private SpriteRenderer turretRenderer;
        private BoxCollider2D turretCollider;

        // Use this for initialization
        void Awake()
        {
            turretRenderer = GetComponent<SpriteRenderer>();
            turretCollider = GetComponent<BoxCollider2D>();
            if (turretData != null)
            {
                turretData.SetRendererSprite(turretRenderer, 0);
                turretData.SetColliderBounds(turretCollider);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
