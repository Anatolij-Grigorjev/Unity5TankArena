using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankArena.Models.Characters
{
    public abstract class AbstractCharacterGoal
    {

        protected float goalProgress;

		public AbstractCharacterGoal() : this(0.0f) {}
        public AbstractCharacterGoal(float progress)
        {
            goalProgress = progress;
        }

		public void Init(float progress) 
		{
			goalProgress = progress;
		}

        public float GetProgress()
        {
            return goalProgress;
        }

		public abstract void UodateProgress(PlayerStats stats);
		public abstract List<string> GetRelevantStats(); // list of statistics codes that are relevant to the current goal

    }

}

