﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TankArena.Constants;
using TankArena.Models.Tank.Weapons;

namespace TankArena.Utils
{
    class SpecialContentResolver
    {
        private static Func<String, TransformState> transformDeserializer = transform => 
        {
            var _9floats = transform.Split(new char[] { ';' }, 9)
                .Select(strNum => { return float.Parse(strNum); }).ToArray();

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
            { "!img;", imgPath => { return Resources.Load<Image>(imgPath); } },
            { "!transf;", transform => { return transformDeserializer(transform); } },
            { "!snd;", soundPath => { return Resources.Load<AudioClip>(soundPath); } },
            { "!wpnslt;", slotDescriptor => 
                {
                    Debug.Log("Working with slot descriptor: " + slotDescriptor);
                    var typeAndTransform = slotDescriptor.Split(new char[] {';'}, 2);
                    WeaponTypes weaponType = (WeaponTypes)int.Parse(typeAndTransform[0]);
                    TransformState transform = typeAndTransform.Length > 1?
                        (TransformState)Resolve(typeAndTransform[1]) : null;

                    return new WeaponSlot(weaponType, transform);
                }
            }
        };

        public static object Resolve(string content)
        {
            Debug.Log("Trying to resolves special content: " + content);
            foreach(KeyValuePair<string, Func<string, object>> resolver in resolvers)
            {
                if (content.StartsWith(resolver.Key))
                {
                    Debug.Log("Got resolver key: " + resolver.Key);
                    var keyAndContent = content.Split(new char[]{';'}, 2);
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
