using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using EK = TankArena.Constants.EntityKeys;

namespace TankArena.Utils
{
    public class SpriteAnimation
    {
        public bool loops;
		public string nextState;
        public int[] spriteIdx;
        public float[] spriteDuration;

        public static SpriteAnimation FromJSON(
			bool loops, 
			JSONArray framesArray,
			string nextState = null)
        {
            var animation = new SpriteAnimation();
            animation.loops = loops;
			animation.nextState = nextState;
            int size = framesArray.Count;
			animation.spriteDuration = new float[size];
			animation.spriteIdx = new int[size];
            for (int i = 0; i < size; i++) 
			{
				var frame = framesArray[i].AsObject;
				animation.spriteDuration[i] = frame[EK.EK_DURATION].AsFloat;
				animation.spriteIdx[i] = frame[EK.EK_INDEX].AsInt;
			}

			return animation;
        }
    }
}
