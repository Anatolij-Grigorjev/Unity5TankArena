using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using TankArena.Utils;
using System.Collections.Generic;
using MovementEffects;

namespace TankArena.UI
{
	public class TallyTableController : MonoBehaviour 
	{

		public GameObject tallyTableRowPrefab;
		public BgHeaderController headerController;
		public Text totalText;
		private IEnumerator<float> cashoutHandle;
		private float cashOutRate;
		public float cashOutRateCoef = 50.0f;
		private float totalSum;

		// Use this for initialization
		void Start () 
		{
			var stats = CurrentState.Instance.CurrentArenaStats;
		
			foreach (KeyValuePair<EnemyType, int> stat in stats) 
			{
				//skip non-handled enemies
				if (stat.Value > 0)
				{
					var row = Instantiate(
						tallyTableRowPrefab
						, transform.position
						, Quaternion.identity
						, transform) as GameObject;
					
					row.GetComponent<TallyTableRowController>().PopulateRow(stat.Key, stat.Value);
				}
			}

			totalSum = stats.Sum(stat => stat.Key.Value * stat.Value);
			SetTotalTally(totalSum);
			//always takes 100 ticks to cash out
			cashOutRate = totalSum / cashOutRateCoef;
		}

		public void SetTotalTally(float total)
		{
			totalText.text = "$" + total;
		}
		
		// Update is called once per frame
		void Update () 
		{
			if (!Input.anyKeyDown) 
			{
				//user pressed key and we didnt start money cahsing yet
				if (cashoutHandle == null)
				{
					cashoutHandle = Timing.RunCoroutine(_CashOutTally(), Segment.FixedUpdate);
				}
			}
		}

		private IEnumerator<float> _CashOutTally()
		{
			while (totalSum > 0.0f)
			{
				var delta = Mathf.Clamp(cashOutRate, 0.0f, totalSum);

				totalSum -= delta;

				CurrentState.Instance.Player.Cash += delta;
				
				headerController.SetCash(CurrentState.Instance.Player.Cash);

				SetTotalTally(totalSum);

				yield return Timing.WaitForSeconds(Timing.DeltaTime);
			}

			//total over
		}
	}
}
