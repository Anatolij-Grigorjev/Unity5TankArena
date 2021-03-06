﻿using System;
using System.Collections.Generic;
using UnityEngine;
using EK = TankArena.Constants.EntityKeys;
using SimpleJSON;
using MovementEffects;
using TankArena.Utils;

namespace TankArena.Models.Characters
{
    public class PlayableCharacter : FileLoadedEntityModel
    {

        /// <summary>
        /// Character Avatar, presented in character selection screen and in HUD
        /// </summary>
        public Sprite Avatar
        {
            get
            {
                return (Sprite)properties[EK.EK_AVATAR_IMAGE];
            }
        }
        /// <summary>
        /// Character background image, presented in character selection screen
        /// </summary>
        public Sprite Background
        {
            get
            {
                return (Sprite)properties[EK.EK_BACKGROUND_IMAGE];
            }
        }
        /// <summary>
        /// Character modeled pose image, presented in character selection screen
        /// </summary>
        public Sprite CharacterModel
        {
            get
            {
                return (Sprite)properties[EK.EK_CHARACTER_MODEL_IMAGE];
            }
        }
        public float StartingCash
        {
            get
            {
                return (float)properties[EK.EK_CHARACTER_STARTER_CASH];
            }
        }
        public String Backstory
        {
            get
            {
                return (string)properties[EK.EK_BACKSTORY];
            }
        }
        public String VictoryText
        {
            get 
            {
                return (string)properties[EK.EK_VICTORY_TEXT];
            }
        }
        public Sprite VictoryImage
        {
            get 
            {
                return (Sprite)properties[EK.EK_VICTORY_IMAGE];
            }
        }
        public String StartingTankCode
        {
            get
            {
                return (String)properties[EK.EK_CHARACTER_STARTER_TANK];
            }
        }
        public Type CharacterGoalType;
        public AbstractCharacterGoal GoalTemplate;
        public CharacterStats StartingStats;

        public Tank.Tank StartingTank;

        public PlayableCharacter(string filePath) : base(filePath)
        {

        }

        protected override IEnumerator<float> _LoadPropertiesFromJSON(JSONNode json)
        {
            var handle = Timing.RunCoroutine(base._LoadPropertiesFromJSON(json));
            yield return Timing.WaitUntilDone(handle);
            properties[EK.EK_AVATAR_IMAGE] = ResolveSpecialContent(json[EK.EK_AVATAR_IMAGE].Value);
            properties[EK.EK_BACKGROUND_IMAGE] = ResolveSpecialContent(json[EK.EK_BACKGROUND_IMAGE].Value);
            properties[EK.EK_CHARACTER_MODEL_IMAGE] = ResolveSpecialContent(json[EK.EK_CHARACTER_MODEL_IMAGE].Value);
            properties[EK.EK_CHARACTER_STARTER_CASH] = json[EK.EK_CHARACTER_STARTER_CASH].AsFloat;
            var statsObj = json[EK.EK_CHARACTER_STARTER_STATS].AsObject;
            StartingStats = CharacterStats.ParseJSONBody(statsObj);
            properties[EK.EK_BACKSTORY] = json[EK.EK_BACKSTORY].Value;
            properties[EK.EK_VICTORY_TEXT] = json[EK.EK_VICTORY_TEXT].Value;
            properties[EK.EK_VICTORY_IMAGE] = ResolveSpecialContent(json[EK.EK_VICTORY_IMAGE].Value);
            properties[EK.EK_CHARACTER_STARTER_TANK] = json[EK.EK_CHARACTER_STARTER_TANK].Value;
            try
            {

                CharacterGoalType = Type.GetType(json[EK.EK_CHARACTER_GOAL_LOGIC].Value);
                if (CharacterGoalType == null)
                {
                    throw new Exception("Got NULL Character Goal type!");
                }
            }
            catch (Exception ex)
            {
                DBG.Log("Problem in loading character goal type: {0}, setting to null goal...", ex.Message);
                CharacterGoalType = typeof(NullCharacterGoal);
                
            }
            GoalTemplate = (AbstractCharacterGoal)Activator.CreateInstance(CharacterGoalType);
            yield return 0.0f;
        }

    }
}
