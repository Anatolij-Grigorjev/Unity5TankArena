using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using EK = TankArena.Constants.EntityKeys;

namespace TankArena.Utils
{

    public class CharacterStats
    {
		public const int MAX_LEVEL = 5;

		private int atk;
		private int mov;
		private int reg;
		private float atkModifier;
		private float movModifier;
		private float regModifier;

		public int ATK
		{
			get 
			{
				return atk;
			}
			set 
			{
				atk = value;
				atkModifier = GetValueModifier(atk);
			}
		}
		public int MOV 
		{
			get 
			{
				return mov;
			}
			set 
			{
				mov = value;
				movModifier = GetValueModifier(mov);
			}
		}

		public int REG 
		{
			get 
			{
				return reg;
			}
			set 
			{
				reg = value;
				regModifier = GetValueModifier(reg);
			}
		}

		private static float GetValueModifier(int value)
		{
			return 1.0f + ((value - 1) / 10.0f);
		}

		public float ATKModifier { get { return atkModifier; } }
		public float MOVModifier { get { return movModifier; } }
		public float REGModifier { get { return regModifier; } }

		public CharacterStats(int atk, int mov, int reg)
		{
			ATK = atk;
			MOV = mov;
			REG = reg;
		}

		public static CharacterStats ParseJSONBody(JSONClass statsObj)
		{
			DBG.Log("BODY: {0}", statsObj);
			var atk = statsObj[EK.EK_STAT_ATTACK].AsInt;
			var mov = statsObj[EK.EK_STAT_MOVE].AsInt;
			var reg = statsObj[EK.EK_STAT_REGEN].AsInt;

            return new CharacterStats(atk, mov, reg);
		}

		public JSONClass ToJSON()
		{
			var statsObj = new JSONClass();
			statsObj.Add(EK.EK_STAT_ATTACK, "" + ATK);
			statsObj.Add(EK.EK_STAT_MOVE, "" + MOV);
			statsObj.Add(EK.EK_STAT_REGEN, "" + REG);

			return statsObj;
		}
       
    }
}
