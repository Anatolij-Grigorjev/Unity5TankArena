using System;
using System.Collections;
using System.Collections.Generic;
using TankArena.Constants;
using TankArena.Models;
using TankArena.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace TankArena.UI
{
    public class StatsBGController : MonoBehaviour
    {

		public Image characterModel;
		public Text statsHeader;
		public GameObject statRow;
		public GameObject statsRowsContainer;
		private Color importantStatColor = Color.red;
		private Player player;

        // Use this for initialization
        void Start()
        {

			player = CurrentState.Instance.Player;
			characterModel.sprite = player.Character.CharacterModel;
			statsHeader.text = string.Format("{0}({1}) STATS: ", player.Name, player.Character.Name);
			
			PopulateStatsList();
        }

		public void PopulateStatsList()
		{
			var player = CurrentState.Instance.Player;
			var importantStats = player.CharacterGoal.GetRelevantStats();
			statsRowsContainer.ClearChildren();
			//add goal row at start of stats
			var goalRowGo = Instantiate(statRow);
			goalRowGo.transform.SetParent(statsRowsContainer.transform, false);
			goalRowGo.transform.localScale = Vector3.one;
			var text = goalRowGo.GetComponent<Text>();
			text.text = String.Format("I WANT TO: {0}", player.CharacterGoal.GetCharacterGoalDescription());
			text.color = Color.white;

			foreach(var stat in player.PlayerStats.stats)
			{
				var statRowGo = Instantiate(statRow);
				statRowGo.transform.SetParent(statsRowsContainer.transform, false);
				statRowGo.transform.localScale = Vector3.one;
				var statText = statRowGo.GetComponent<Text>();
				statText.text = PlayerStatTypes.NameForCode(stat.Key).ToUpper() + ValueToString(stat.Value);
				if (importantStats.Contains(stat.Key))
				{
					statText.color = importantStatColor;
				}
			}
		}

		private string ValueToString(object value)
		{
			if (value is ICollection)
			{
				return UIUtils.PrintElements((ICollection)value);
			} else 
			{
				return value != null? value.ToString() : "";
			}
		}

        // Update is called once per frame
        void Update()
        {
			if (Input.anyKey)
			{
				this.gameObject.SetActive(false);
			}
        }
    }

}
