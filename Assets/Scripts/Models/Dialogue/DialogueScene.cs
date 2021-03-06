﻿using System.Collections.Generic;
using UnityEngine;
using EK = TankArena.Constants.EntityKeys;
using SimpleJSON;
using MovementEffects;

namespace TankArena.Models.Dialogue
{
    public class DialogueScene : FileLoadedEntityModel
    {

        private const float SCENE_DEFAULT_START_TIME = 1.5f;
        private const float SCENE_DEFAULT_END_TIME = 1.5f;
        private const float SCENE_DEFAULT_CHANGE_BG_TIME = 1.0f;
        public Sprite SceneBackground
        {
            get
            {
                return (Sprite)properties[EK.EK_BACKGROUND_IMAGE];
            }
        }
        public float SceneBgInterpolateTime
        {
            get 
            {
                return (float)properties[EK.EK_CHANGE_BG_TIME];
            }
        }
        public float SceneStartTime 
        {
            get
            {
                return (float)properties[EK.EK_START_TIME];
            }
        }
        public float SceneEndTime
        {
            get
            {
                return (float)properties[EK.EK_END_TIME];
            }
        }

        public DialogueSceneActorInfo LeftActor
        {
            get 
            {
                return (DialogueSceneActorInfo)properties[EK.EK_ACTOR_LEFT];
            }
        }
        public DialogueSceneActorInfo RightActor
        {
            get 
            {
                return (DialogueSceneActorInfo)properties[EK.EK_ACTOR_RIGHT];
            }
        }

        public string LevelId
        {
            get 
            {
                return (string)properties[EK.EK_LEVEL];
            }
        }
        public DialoguePosition Position 
        {
            get 
            {
                return (DialoguePosition)properties[EK.EK_POSITION];
            }
        }
        public string CharacterId  
        {
            get 
            {
                return (string)properties[EK.EK_CHARACTER];
            }
        }

        public List<DialogueBeat> dialogueBeats;

        //quick access to specific scene beat
        public DialogueBeat this[int i]
        {
            get { return dialogueBeats[i]; }
        }


        public DialogueScene(string jsonPath) : base(jsonPath) { }

        protected override IEnumerator<float> _LoadPropertiesFromJSON(JSONNode json)
        {
            var handle = Timing.RunCoroutine(base._LoadPropertiesFromJSON(json));
            yield return Timing.WaitUntilDone(handle);
            //GET DIALOGUE SCENE MODEL META INFO
            JSONClass scene = json[EK.EK_SCENE].AsObject;
            properties[EK.EK_BACKGROUND_IMAGE] = ResolveSpecialOrKey(scene[EK.EK_BACKGROUND_IMAGE], EK.EK_BACKGROUND_IMAGE);
            properties[EK.EK_START_TIME] = scene[EK.EK_START_TIME].AsFloat;
            var sceneTiming = json[EK.EK_TIMING].AsObject;
            properties[EK.EK_LEVEL] = sceneTiming[EK.EK_LEVEL].Value;
            properties[EK.EK_CHARACTER] = sceneTiming[EK.EK_CHARACTER].Value;
            properties[EK.EK_POSITION] = DialoguePositionHelper.ForCode(sceneTiming[EK.EK_POSITION].Value);
            if (SceneStartTime == 0.0f) 
            {
                properties[EK.EK_START_TIME] = SCENE_DEFAULT_START_TIME;
            }
            properties[EK.EK_END_TIME] = scene[EK.EK_END_TIME].AsFloat;
            if (SceneEndTime == 0.0f) 
            {
                properties[EK.EK_END_TIME] = SCENE_DEFAULT_END_TIME;
            }
            properties[EK.EK_CHANGE_BG_TIME] = scene[EK.EK_CHANGE_BG_TIME].AsFloat;
            if (SceneBgInterpolateTime == 0.0f) 
            {
                properties[EK.EK_CHANGE_BG_TIME] = SCENE_DEFAULT_CHANGE_BG_TIME;
            }
            
            properties[EK.EK_ACTOR_LEFT] = DialogueSceneActorInfo.parseJSON(scene[EK.EK_ACTOR_LEFT].AsObject);
            properties[EK.EK_ACTOR_RIGHT] = DialogueSceneActorInfo.parseJSON(scene[EK.EK_ACTOR_RIGHT].AsObject);

            dialogueBeats = new List<DialogueBeat>();
            // GET INDIVIDUAL DIALOGUE BEATS
            foreach (JSONNode beatObj in json[EK.EK_BEATS].AsArray)
            {
                dialogueBeats.Add(DialogueBeat.parseJSON(beatObj.AsObject));
            }

            yield return 0.0f;
        }

    }
}
