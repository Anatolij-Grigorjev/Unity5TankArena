using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using TankArena.Utils;
using System.Collections.Generic;
using MovementEffects;
using TankArena.Constants;

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
        void Start()
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
            //always takes cashout ticks to cash out
            cashOutRate = totalSum / cashOutRateCoef;
        }

        public void SetTotalTally(float total)
        {
            totalText.text = "$" + total;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.anyKeyDown)
            {
                //user pressed key and we didnt start money cahsing yet
                if (cashoutHandle == null)
                {
                    cashoutHandle = Timing.RunCoroutine(_CashOutTally(), Segment.FixedUpdate);
                }
                else
                {
                    if (totalSum != 0.0f)
                    {
                        //cashout started but money still flowing - speed it along
                        TransferTotalToPlayerDelta(totalSum);
                    }
                    else
                    {
                        //total sum cashed out, lets go back to arenas
                        if (CurrentState.Instance.CurrentDialogueScenesAfter.ContainsKey(CurrentState.Instance.CurrentArena.Id)) 
                        {
                            var dialogue = CurrentState.Instance.CurrentDialogueScenesAfter[CurrentState.Instance.CurrentArena.Id];
                            TransitionUtil.SaveAndStartTransitionTo(SceneIds.SCENE_DIALOGUE_ID,
                            new Dictionary<string, object>{ 
                                {TransitionParams.PARAM_DIALOGUE_SCENE_ID, dialogue.Id},
                                {TransitionParams.PARAM_DIALOGUE_POSITION, dialogue.Position}});
                        } else 
                        {
                            TransitionUtil.SaveAndStartTransitionTo(SceneIds.SCENE_ARENA_SELECT_ID);
                        }
                    }
                }
            }
        }

        private void TransferTotalToPlayerDelta(float delta)
        {
            totalSum -= delta;

            CurrentState.Instance.Player.Cash = Mathf.Clamp(
                CurrentState.Instance.Player.Cash + delta, 0.0f, float.MaxValue);

            headerController.SetCash(CurrentState.Instance.Player.Cash);

            SetTotalTally(totalSum);

        }

        private IEnumerator<float> _CashOutTally()
        {
            while (totalSum != 0.0f)
            {
                var delta = totalSum > 0.0f ?
					 Mathf.Clamp(cashOutRate, 0.0f, totalSum)
					 : Mathf.Clamp(cashOutRate, totalSum, 0.0f);

                TransferTotalToPlayerDelta(delta);

                yield return Timing.WaitForSeconds(Timing.DeltaTime);
            }

            //total over
        }
    }
}
