using System.Collections;
using System.Collections.Generic;
using TankArena.Constants;
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

        // Use this for initialization
        void Start()
        {

			var player = CurrentState.Instance.Player;
			characterModel.sprite = player.Character.CharacterModel;
			statsHeader.text = string.Format("{0} STATS: ", player.Name);
			var importantStats = player.CharacterGoal.GetRelevantStats();
			foreach(var stat in player.PlayerStats.stats)
			{
				var statRowGo = Instantiate(statRow);
				statRowGo.transform.parent = statsRowsContainer.transform;
				statRowGo.transform.localScale = Vector3.one;
				var statText = statRowGo.GetComponent<Text>();
				statText.text = PlayerStatTypes.NameForCode(stat.Key).ToUpper() + stat.Value;
				if (importantStats.Contains(stat.Key))
				{
					statText.color = importantStatColor;
				}
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
