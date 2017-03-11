using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TankArena.Utils;

namespace TankArena.UI 
{
	public class TallyTableRowController : MonoBehaviour 
	{

		public Text enemyDescriptionText;
		public Text enemyWorthText;
		public Text enemyAmountText;
		public Text enemyTotalTallyText;

		// Use this for initialization
		void Start () 
		{
		
		}

		public void PopulateRow(EnemyType enemyType, int amount)
		{
			enemyDescriptionText.text = "\t" + enemyType.Name;
			enemyAmountText.text = "" + amount;
			enemyWorthText.text = "$" + enemyType.Value;
			enemyTotalTallyText.text = "$" + (enemyType.Value * amount);
		}
		
		// Update is called once per frame
		void Update () 
		{
		
		}
	}
}
