using System;
using System.Collections;
using System.Collections.Generic;
using TankArena.Constants;
using UnityEngine;

namespace TankArena.Models.Characters
{
    public class MendozaCharacterGoal : AbstractCharacterGoal
    {

		private readonly string GOAL_TEXT = String.Format("Earn adoration of public by continuously fighting without the use of my money");

		private readonly List<string> RELEVANT_STATS = new List<string>( new string[] {
			PlayerStatTypes.STAT_TOTAL_SPENT,
			PlayerStatTypes.STAT_TOTAL_ARENAS_PLAYED
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
			//2.5 % progress for every arena played at, divided by a 75th of the cash spent thus far
            goalProgress = ((2.5f * stats.TotalArenasPlayed) / Mathf.Clamp((stats.TotalSpent) / 75.0f, 1.0f, float.MaxValue)) / 100.0f;
        }
    }
}

