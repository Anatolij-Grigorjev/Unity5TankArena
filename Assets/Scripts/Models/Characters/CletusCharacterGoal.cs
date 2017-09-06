using System;
using System.Collections;
using System.Collections.Generic;
using TankArena.Constants;
using UnityEngine;

namespace TankArena.Models.Characters
{
    public class CletusCharacterGoal : AbstractCharacterGoal
    {

		private const float NEEDED_MONEY = 50000.0f;
		private readonly string GOAL_TEXT = String.Format("Earn {0}$ to buy back the family farm.", NEEDED_MONEY);

		private readonly List<string> RELEVANT_STATS = new List<string>( new string[] {
			PlayerStatTypes.STAT_TOTAL_EARNED,
			PlayerStatTypes.STAT_TOTAL_SPENT
		});

        public override string GetCharacterGoalDescription()
        {
            return GOAL_TEXT;
        }

        public override List<string> GetRelevantStats()
        {
            return RELEVANT_STATS;
        }

        public override void UpdateProgress(PlayerStats stats)
        {
            goalProgress = Mathf.Clamp(((stats.TotalEarned - stats.TotalSpent) / NEEDED_MONEY), 0.0f, 1.0f);
        }
    }
}
