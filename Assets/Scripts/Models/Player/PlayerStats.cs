﻿using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using TankArena.Utils;
using UnityEngine;
using PST = TankArena.Constants.PlayerStatTypes;

namespace TankArena.Models
{
    public class PlayerStats
    {
        public Dictionary<string, object> stats;

        private void UpdateCharacterGoal()
        {
            if (CurrentState.Instance.Player != null)
            {
                if (CurrentState.Instance.Player.CharacterGoal != null)
                {
                    CurrentState.Instance.Player.CharacterGoal.UpdateProgress(this);
                }
            }
        }


        public float TotalKills
        {
            get
            {
                return (float)stats[PST.STAT_TOTAL_KILLED];
            }
            set
            {
                stats[PST.STAT_TOTAL_KILLED] = value;
                UpdateCharacterGoal();
            }
        }

        public float TotalKillsSinceLastDeath
        {
            get 
            {
                return (float)stats[PST.STAT_KILLS_SINCE_DEATH];
            }
            set 
            {
                stats[PST.STAT_KILLS_SINCE_DEATH] = value;
                UpdateCharacterGoal();
            }
        }

        public float TotalDeaths
        {
            get
            {
                return (float)stats[PST.STAT_TOTAL_DEATHS];
            }
            set
            {
                stats[PST.STAT_TOTAL_DEATHS] = value;
                UpdateCharacterGoal();
            }
        }

        public string LastArena
        {
            get
            {
                return (string)stats[PST.STAT_LAST_ARENA];
            }
            set
            {
                stats[PST.STAT_LAST_ARENA] = value;
                UpdateCharacterGoal();
            }
        }

        public float TotalEarned
        {
            get
            {
                return (float)stats[PST.STAT_TOTAL_EARNED];
            }
            set
            {
                stats[PST.STAT_TOTAL_EARNED] = value;
                UpdateCharacterGoal();
            }
        }
        public float TotalSpent
        {
            get
            {
                return (float)stats[PST.STAT_TOTAL_SPENT];
            }
            set
            {
                stats[PST.STAT_TOTAL_SPENT] = value;
                UpdateCharacterGoal();
            }
        }

        public float TotalArenasPlayed
        {
            get
            {
                return (float)stats[PST.STAT_TOTAL_ARENAS_PLAYED];
            }
            set
            {
                stats[PST.STAT_TOTAL_ARENAS_PLAYED] = value;
                UpdateCharacterGoal();
            }
        }

        public List<string> FinishedArenas
        {
            get
            {
                return (List<string>)stats[PST.STAT_FINISHED_ARENAS];
            }
        }

        public PlayerStats()
        {
            this.stats = new Dictionary<string, object>();
            //add entry for all future stats
            PST.ALL_STATS.ForEach(code =>
            {
                this.stats.Add(code, null);
            });
            //put 0 for numeric stats
            PST.NUM_STATS.ForEach(code =>
            {
                this.stats.Remove(code);
                this.stats.Add(code, 0.0f);
            });
            //put empty lists for list stats
            PST.LIST_STATS.ForEach(code =>
            {
                this.stats.Remove(code);
                this.stats.Add(code, new List<string>());
            });
        }

        public static PlayerStats FromJSON(JSONClass json)
        {
            PlayerStats stats = new PlayerStats();
            PST.ALL_STATS.ForEach(code =>
            {
                if (PST.NUM_STATS.Contains(code))
                {
                    stats.stats[code] = json[code].AsFloat;
                }
                else
                {
                    if (PST.LIST_STATS.Contains(code))
                    {
                        stats.stats[code] = json[code].AsArray.ToList();
                    }
                    else
                    {
                        stats.stats[code] = json[code].Value;
                    }
                }
            });

            return stats;
        }

        public JSONClass ToJSON()
        {
            JSONClass statsJson = new JSONClass();
            foreach (var stat in stats)
            {
                if (!PST.LIST_STATS.Contains(stat.Key))
                {
                    if (stat.Value == null)
                    {
                        statsJson.Add(stat.Key, "");
                    }
                    else
                    {
                        statsJson.Add(stat.Key, stat.Value.ToString());
                    }
                }
                else
                {
                    statsJson.Add(stat.Key, ((List<string>)stat.Value).ToJsonArray());
                }
            }

            return statsJson;
        }

    }
}

