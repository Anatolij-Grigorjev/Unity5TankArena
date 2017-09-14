
using System;
using SimpleJSON;
using TankArena.Constants;
using TankArena.Controllers.Weapons;
using TankArena.Utils;
using UnityEngine;
using EK = TankArena.Constants.EntityKeys;

namespace TankArena.Models.Weapons
{
    public class CompositeProjectileModel: ProjectileModel
    {


        public float ProjectilesSpreadRadius;
        public int ProjectilesCount;
        public ProjectileModel Projectiles;


        public CompositeProjectileModel() : base()
        {

        }

        public static new CompositeProjectileModel ParseFromJSON(JSONClass json)
        {
            var result = new CompositeProjectileModel();
            var model = ProjectileModel.ParseFromJSON(json);
            result.Velocity = model.Velocity;
            result.BoxCollider = model.BoxCollider;
            result.Tag = model.Tag;
            result.ProjectilesSpreadRadius = json[EK.EK_PROJECTILES_SPREAD_RADIUS].AsFloat;
            result.ProjectilesCount = json[EK.EK_PROJECTILES_COUNT].AsInt;
            result.Projectiles = ProjectileModel.ParseFromJSON(json);
            

            return result;
        }


        public override void SetDataToController(ProjectileController controller)
        {
            //divide damage by projectiels count to keep things fair
            Damage /= ProjectilesCount;
            //prepare controller
            base.SetDataToController(controller);
            controller.isComposite = true;
            controller.isDecorative = true;
            var projectilePrefab = Resources.Load<GameObject>(PrefabPaths.PREFAB_PROJECTILE);
            var go = controller.gameObject;
            for (int i = 0; i < ProjectilesCount; i++)
            {
                var child = GameObject.Instantiate(projectilePrefab);
                SetDataToController(child.GetComponent<ProjectileController>());
                child.transform.SetParent(go.transform, false);
                child.transform.position = RandomUtils.RandomVector2D(ProjectilesSpreadRadius, ProjectilesSpreadRadius);
            }

            GameObject.Destroy(controller.gameObject.GetComponent<Rigidbody2D>());
            GameObject.Destroy(controller.GetComponent<Collider2D>());
        }
        
    }
}