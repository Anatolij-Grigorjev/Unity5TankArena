﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using EK = TankArena.Constants.EntityKeys;
using SimpleJSON;

namespace TankArena.Models.Characters
{
    class PlayableCharacter : FileLoadedEntityModel
    {

        /// <summary>
        /// Character Avatar, presented in character selection screen and in HUD
        /// </summary>
        public Image Avatar
        {
            get
            {
                return (Image)properties[EK.EK_AVATAR_IMAGE];
            }
        }
        /// <summary>
        /// Character background image, presented in character selection screen
        /// </summary>
        public Image Background
        {
            get
            {
                return (Image)properties[EK.EK_BACKGROUND_IMAGE];
            }
        }
        /// <summary>
        /// Character modeled pose image, presented in character selection screen
        /// </summary>
        public Image CharacterModel
        {
            get
            {
                return (Image)properties[EK.EK_CHARACTER_MODEL_IMAGE];
            }
        }

        public PlayableCharacter(string filePath) : base(filePath)
        {

        }

        protected override void LoadPropertiesFromJSON(JSONNode json)
        {
            base.LoadPropertiesFromJSON(json);

            properties[EK.EK_AVATAR_IMAGE] =  ResolveSpecialContent(json[EK.EK_AVATAR_IMAGE].Value);
            properties[EK.EK_BACKGROUND_IMAGE] = ResolveSpecialContent(json[EK.EK_BACKGROUND_IMAGE].Value);
            properties[EK.EK_CHARACTER_MODEL_IMAGE] = ResolveSpecialContent(json[EK.EK_CHARACTER_MODEL_IMAGE].Value);
        }
       
    }
}