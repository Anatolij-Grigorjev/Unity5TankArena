
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
            model.CopyPropsTo(result);
            result.ProjectilesSpreadRadius = json[EK.EK_PROJECTILES_SPREAD_RADIUS].AsFloat;
            result.ProjectilesCount = json[EK.EK_PROJECTILES_COUNT].AsInt;
            result.Projectiles = ProjectileModel.ParseFromJSON(json);

            return result;
        }


        public override void SetDataToController(ProjectileController controller)
        {
            //prepare controller
            base.SetDataToController(controller);
            controller.isComposite = true;
            controller.isDecorative = true;
            controller.sprites = null;
            controller.spriteDurationTimes = null;
            var projectilePrefab = Resources.Load<GameObject>(PrefabPaths.PREFAB_PROJECTILE);
            var go = controller.gameObject;
            //divide damage by projectiels count to keep things fair
            Damage = Damage / ProjectilesCount;
            for (int i = 0; i < ProjectilesCount; i++)
            {
                var child = GameObject.Instantiate(projectilePrefab);
                base.SetDataToController(child.GetComponent<ProjectileController>());
                child.transform.SetParent(go.transform, false);
                child.transform.localScale = Vector3.one;
                child.transform.position = RandomUtils.RandomVector2D(ProjectilesSpreadRadius, ProjectilesSpreadRadius);
            }
            //bring initial damage back since this code is run on every bullet prep and it needs to be fair
            Damage = Damage * ProjectilesCount;
            GameObject.Destroy(controller.gameObject.GetComponent<Rigidbody2D>());
            GameObject.Destroy(controller.GetComponent<Collider2D>());
            GameObject.Destroy(controller.spriteRenderer);
        }
        
    }
}