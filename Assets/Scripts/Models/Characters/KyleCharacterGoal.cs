using System;
using System.Collections;
using System.Collections.Generic;
using TankArena.Constants;
using UnityEngine;

namespace TankArena.Models.Characters
{
    public class KyleCharacterGoal : AbstractCharacterGoal
    {

        

		private const float NEEDED_KILLS = 50;
        private float lastDeaths = 0;
		private readonly string GOAL_TEXT = String.Format("Best {0} opponents wihtout dying to find the true meaning of self-confidence", NEEDED_KILLS);

		private readonly List<string> RELEVANT_STATS = new List<string>( new string[] {
			PlayerStatTypes.STAT_KILLS_SINCE_DEATH,
			PlayerStatTypes.STAT_TOTAL_DEATHS
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
      
                goalProgress = Mathf.Clamp(stats.TotalKillsSinceLastDeath / NEEDED_KILLS, 0.0f, 1.0f);
            
        }
    }
}
