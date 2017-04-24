using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using TankArena.Models;
using UnityEngine;

namespace TankArena.Constants
{

    public enum DialogueSignalTypes
    {

        LEFT_ACTOR_ACTION = 2,
        RIGHT_ACTOR_ACTION = 3,
        LEFT_ACTOR_SPEECH = 0,
        RIGHT_ACTOR_SPEECH = 1,
        CHANGE_BACKGROUND = 9
    }

    public static class DialogueSignalTypesHelper
    {
        private static readonly Dictionary<string, DialogueSignalTypes> typesByTag = new Dictionary<string, DialogueSignalTypes>() {
             { "left_action", DialogueSignalTypes.LEFT_ACTOR_ACTION },
             { "left", DialogueSignalTypes.LEFT_ACTOR_SPEECH },
             { "right", DialogueSignalTypes.RIGHT_ACTOR_SPEECH },
             { "right_action", DialogueSignalTypes.RIGHT_ACTOR_ACTION },
             { "change_bg", DialogueSignalTypes.CHANGE_BACKGROUND }
        };

        private static readonly Func<JSONArray, List<object>> actorTrigerParams = (arr) =>
        {
            var results = new List<object>();
            //with only 1 parameter, its the trigger name
            if (arr.Count > 0)
            {
                results.Add(arr[0].Value);
            }
            //2nd parameter is visibility of actor
            if (arr.Count > 1)
            {
                results.Add(arr[1].AsBool);
            }
            return results;
        };

        private static readonly Dictionary<DialogueSignalTypes, Func<JSONArray, List<object>>> typeParamsParsers = new Dictionary<DialogueSignalTypes, Func<JSONArray, List<object>>>() {
           { DialogueSignalTypes.LEFT_ACTOR_ACTION, actorTrigerParams },
           { DialogueSignalTypes.RIGHT_ACTOR_ACTION, actorTrigerParams },
           { DialogueSignalTypes.CHANGE_BACKGROUND, (arr) => {
                var results = new List<object>();

                //only 1 param the background
                if (arr.Count > 0)
                {
                    results.Add(FileLoadedEntityModel.ResolveSpecialOrKey(arr[0].Value, EntityKeys.EK_BACKGROUND_IMAGE));
                }
                if (arr.Count > 1)
                {
                    results.Add(arr[1].AsFloat);
                }

                return results;
           } }
        };

        public static DialogueSignalTypes Parse(string type)
        {
            if (typesByTag.ContainsKey(type))
            {
                return typesByTag[type];
            }
            else
            {
                return DialogueSignalTypes.LEFT_ACTOR_ACTION;
            }
        }

        public static List<object> ParseParams(DialogueSignalTypes typeContext, JSONArray jsonData)
        {
            if (typeParamsParsers.ContainsKey(typeContext))
            {
                return typeParamsParsers[typeContext](jsonData);
            }
            else
            {
                return new List<object>();
            }
        }
    }

}
