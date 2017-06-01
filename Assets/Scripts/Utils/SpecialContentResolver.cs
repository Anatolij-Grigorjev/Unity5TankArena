using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TankArena.Constants;
using TankArena.Models.Weapons;
using CielaSpike;

namespace TankArena.Utils
{
    class SpecialContentResolver
    {

        private static readonly char[] COMMON_ARRAY_DELIM = { ';' };
        //weapon slot contains 2 transforms + type so common delim wont do
        private static readonly char[] WPN_SLT_DELIM = { '|' };

        private static Func<String, TransformState> transformDeserializer = transform =>
        {
            var _9floats = transform.Split(COMMON_ARRAY_DELIM, 9)
                .Select(strNum => float.Parse(strNum)).ToArray();

            float[] positionFloats = new float[] { _9floats[0], _9floats[1], _9floats[2] };
            float[] rotationFloats = new float[] { _9floats[3], _9floats[4], _9floats[5] };
            float[] scaleFloats = new float[] { _9floats[6], _9floats[7], _9floats[8] };

            var ts = new TransformState();
            ts.position = new Vector3(positionFloats[0], positionFloats[1], positionFloats[2]);
            ts.rotation = new Vector3(rotationFloats[0], rotationFloats[1], rotationFloats[2]);
            ts.scale = new Vector3(scaleFloats[0], scaleFloats[1], scaleFloats[2]);

            return ts;
        };

        private SpecialContentResolver() { }

        private static Dictionary<String, Func<String, object>> resolvers = new Dictionary<string, Func<string, object>>()
        {
            { "!img;", imgPath => { return Resources.Load<Sprite>(imgPath) as Sprite; } },
            { "!transf;", transform => { return transformDeserializer(transform); } },
            { "!snd;", soundPath => { return Resources.Load<AudioClip>(soundPath) as AudioClip; } },
            { "!sprites;", sheetPath => { return Resources.LoadAll<Sprite>(sheetPath); } },
            { "!anim_contr;", animContrPath => { return Resources.Load<RuntimeAnimatorController>(animContrPath) as RuntimeAnimatorController; } },
            { "!go;", gameObjectPath => { return Resources.Load<GameObject>(gameObjectPath); } },
            { "!id;", gameObjectId => { return EntitiesStore.Instance.Entities[gameObjectId]; }},
            { "!box;", rectNums =>
                {
                    var _4floats = rectNums.Split(COMMON_ARRAY_DELIM, 4)
                        .Select(floatStr => float.Parse(floatStr)).ToArray();
                    return new Rect(_4floats[0], _4floats[1], _4floats[2], _4floats[3]);
                }
            },
            { "!wpnslt;", slotDescriptor =>
                {
                    var typeAndTransforms = slotDescriptor.Split(WPN_SLT_DELIM, 3);
                    WeaponTypes weaponType = (WeaponTypes)int.Parse(typeAndTransforms[0]);
                    TransformState shopTransform = typeAndTransforms.Length > 1?
                        (TransformState)Resolve(typeAndTransforms[1]) : null;
                    TransformState arenaTransform = typeAndTransforms.Length > 2?
                        (TransformState)Resolve(typeAndTransforms[2]) : null;


                    return new WeaponSlot(weaponType, shopTransform, arenaTransform);
                }
            },
            { "!v3;", vector3 =>
                {
                    var vectorComponents = vector3.Split(COMMON_ARRAY_DELIM, 3)
                        .Select(compStr => float.Parse(compStr)).ToArray();
                    return new Vector3(vectorComponents[0], vectorComponents[1], vectorComponents[2]);
                }
            }
        };

        public static object Resolve(string content)
        {
            DBG.Log("Trying to resolves special content: {0}", content);
            foreach (KeyValuePair<string, Func<string, object>> resolver in resolvers)
            {
                if (content.StartsWith(resolver.Key))
                {
                    DBG.Log("Got resolver key: {0}", resolver.Key);
                    var keyAndContent = content.Split(COMMON_ARRAY_DELIM, 2);
                    if (keyAndContent.Length < 2)
                    {
                        return content;
                    }

                    return resolver.Value(keyAndContent[1]);
                }
            }

            return content;
        }
    }
}
