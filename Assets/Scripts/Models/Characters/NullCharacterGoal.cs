using System.Collections;
using System.Collections.Generic;
using TankArena.Utils;
using UnityEngine;

namespace TankArena.Models.Characters
{
    public class NullCharacterGoal : AbstractCharacterGoal
    {
        public override string GetCharacterGoalDescription()
        {
            return "Accomplish <null>";
        }

        public override List<string> GetRelevantStats()
        {
            return new List<string>();
        }

        public override void UpdateProgress(PlayerStats stats)
        {
            DBG.Log("NullCharacterGoal received stats: {0}", UIUtils.PrintElements(stats.stats, "\n"));
        }
    }
}