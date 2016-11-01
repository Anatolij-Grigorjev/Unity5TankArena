using UnityEngine;
using System.Collections;
using TankArena.Models.Characters;
using TankArena.Models.Tank;
using PP = TankArena.Constants.PlayerPrefsKeys;
using TankArena.Utils;
using DBG = TankArena.Utils.DBG;
using TankArena.Constants;
using System.Collections.Generic;

namespace TankArena.Controllers
{
    public class PlayerController : MonoBehaviour
    {

        private PlayableCharacter character;
        public TankController tankController;

        public Queue<TankCommand> commands;

        public float moveDeadzone;
        //[HideInInspector]
        public float Health { get; set; }
        //[HideInInspector]
        public float Cash { get; set; }

        // Use this for initialization
        void Start()
        {
            LoadFromPlayerPrefs();
            commands = tankController.Commands;
        }

        // Update is called once per frame
        void Update()
        {

            PerformTurretRotation();

            var moveAxis = Input.GetAxis(ControlsButtonNames.BTN_NAME_TANK_MOVE);
            var turnAxis = Input.GetAxis(ControlsButtonNames.BTN_NAME_TANK_TURN);

            if (Mathf.Abs(moveAxis) > moveDeadzone || Mathf.Abs(turnAxis) > moveDeadzone
                || tankController.isMoving())
            {
                commands.Enqueue(new TankCommand(TankCommandWords.TANK_COMMAND_MOVE, new Dictionary<string, object>
                {
                    { TankCommandParamKeys.TANK_CMD_MOVE_KEY, moveAxis },
                    { TankCommandParamKeys.TANK_CMD_TURN_KEY, turnAxis }
                }));
            }

            var brakeHeld = Input.GetButton(ControlsButtonNames.BTN_NAME_HANDBREAK);
            var brakeLetGo = Input.GetButtonUp(ControlsButtonNames.BTN_NAME_HANDBREAK);
            if (brakeHeld || brakeLetGo)
            {
                commands.Enqueue(new TankCommand(TankCommandWords.TANK_COMMAND_BRAKE, new Dictionary<string, object>
                {
                    //this will keep sending true on every frame brake is held and will send false on the last one,
                    //which means brakeletgo was true
                    { TankCommandParamKeys.TANK_CMD_APPLY_BREAK_KEY, brakeHeld }
                }));
            }
        }

        private void PerformTurretRotation() {
        	var turretRotator = tankController.turretController.Rotator;
            Vector2 mousePos = Input.mousePosition;
            var screenPoint = Camera.main.WorldToScreenPoint(turretRotator.position);
            var offset = new Vector2(mousePos.x - screenPoint.x, mousePos.y - screenPoint.y);
            //DBG.Log("Current rotations: turret: {0}, {1}, chassis: {2}, {3}", 
            //    turretRotator.eulerAngles.z, 
            //    turretRotator.localEulerAngles.z,
            //    transform.eulerAngles.z,
            //    transform.localEulerAngles.z);
            var angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg - transform.eulerAngles.z;
            //angle will get calculated based of the difference of main tank rotation and turret rotation
            var wantedRotation = Quaternion.Euler(0, 0, angle - 90);
            //DBG.Log("Wanted z-rotation: {0}", angle);  

            turretRotator.localRotation =
                Quaternion.Lerp(turretRotator.localRotation, wantedRotation, Time.fixedDeltaTime * 1.7f);

            //DBG.Log("Rotator and tank rotation diff {0} - {1} = {2}",
            //    turretRotator.rotation.eulerAngles,
            //    transform.rotation.eulerAngles,
            //    turretRotator.rotation.eulerAngles - transform.rotation.eulerAngles);
        }

        private void SaveToPlayerPrefs()
        {
            //take the current player customizations and save them into the preferences
            PlayerPrefs.SetString(PP.PP_CHARACTER, character.Id);
            PlayerPrefs.SetString(PP.PP_TANK, tankController.Tank.ToCode());
            PlayerPrefs.SetFloat(PP.PP_HEALTH, Health);
            PlayerPrefs.SetFloat(PP.PP_CASH, Cash);

            //flush the prefs
            PlayerPrefs.Save();
        }

        private void LoadFromPlayerPrefs()
        {

            //construct player character and tank from encoded keys of ids
            //tank encoded as key-value map, key type of component, value is entity id

            //player always has a selected character. this code will come from main menu later, for now hardcoded
            var characterCode = PlayerPrefs.HasKey(PP.PP_CHARACTER) ? PlayerPrefs.GetString(PP.PP_CHARACTER) : "dummy";
            //filter search to specific map because its faster AND for type safety
            character = EntitiesStore.Instance.Characters[characterCode];

            //these keys might be absent if new game
            var tankCode = PlayerPrefs.HasKey(PP.PP_TANK) ? PlayerPrefs.GetString(PP.PP_TANK) : character.StartingTankCode;
            tankController.Tank = Tank.FromCode(tankCode);
            Health = PlayerPrefs.HasKey(PP.PP_HEALTH) ? PlayerPrefs.GetFloat(PP.PP_HEALTH) : character.StartingHealth;
            Cash = PlayerPrefs.HasKey(PP.PP_CASH) ? PlayerPrefs.GetFloat(PP.PP_CASH) : character.StartingCash;
        }
    }
}
