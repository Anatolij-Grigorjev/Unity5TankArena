using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankArena.Constants
{
    public class PlayerStatTypes
    {

        public const string STAT_TOTAL_KILLED = "total_kills";
        public const string STAT_TOTAL_EARNED = "total_earned";
        public const string STAT_TOTAL_ARENAS_PLAYED = "total_plays";
        public const string STAT_TOTAL_SPENT = "total_spent";
        public const string STAT_LAST_ARENA = "last_played_arena";
        public const string STAT_FINISHED_ARENAS = "finished_arenas";
        public const string STAT_TOTAL_DEATHS = "total_deaths";

        public static readonly List<string> NUM_STATS = new List<string>(new string[] {
            STAT_TOTAL_KILLED,
            STAT_TOTAL_EARNED,
            STAT_TOTAL_DEATHS,
            STAT_TOTAL_ARENAS_PLAYED,
            STAT_TOTAL_SPENT
        });

        public static readonly List<string> LIST_STATS = new List<string>(new string[] {
            STAT_FINISHED_ARENAS
        });

        public static readonly List<string> ALL_STATS = new List<string>(new string[] {
            STAT_TOTAL_KILLED,
            STAT_TOTAL_EARNED,
            STAT_TOTAL_ARENAS_PLAYED,
            STAT_TOTAL_SPENT,
            STAT_LAST_ARENA,
            STAT_FINISHED_ARENAS,
            STAT_TOTAL_DEATHS
        });

    }
}

