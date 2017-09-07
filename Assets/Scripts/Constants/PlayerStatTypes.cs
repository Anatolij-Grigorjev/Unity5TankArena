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
        public const string STAT_KILLS_SINCE_DEATH = "kills_since_death";

        public static string NameForCode(string statCode)
        {
            switch(statCode)
            {
                case STAT_TOTAL_KILLED:
                    return "Total enemy kills: ";
                case STAT_TOTAL_EARNED:
                    return "Total money earned: ";
                case STAT_TOTAL_ARENAS_PLAYED:
                    return "Total arenas played: ";
                case STAT_TOTAL_SPENT:
                    return "Total money spent: ";
                case STAT_LAST_ARENA:
                    return "Last arena played: ";
                case STAT_FINISHED_ARENAS:
                    return "Finished arenas: ";
                case STAT_TOTAL_DEATHS:
                    return "Total deaths: ";
                case STAT_KILLS_SINCE_DEATH:
                    return "Total kills since last death: ";
                default:
                    return statCode;
            }
        }

        public static readonly List<string> NUM_STATS = new List<string>(new string[] {
            STAT_TOTAL_KILLED,
            STAT_KILLS_SINCE_DEATH,
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
            STAT_KILLS_SINCE_DEATH,
            STAT_TOTAL_EARNED,
            STAT_TOTAL_ARENAS_PLAYED,
            STAT_TOTAL_SPENT,
            STAT_LAST_ARENA,
            STAT_FINISHED_ARENAS,
            STAT_TOTAL_DEATHS
        });

    }
}

