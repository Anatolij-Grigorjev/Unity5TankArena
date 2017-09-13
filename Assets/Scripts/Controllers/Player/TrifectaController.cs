using System;
using System.Collections;
using System.Collections.Generic;
using MovementEffects;
using TankArena.Constants;
using TankArena.Controllers.Weapons;
using TankArena.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace TankArena.Controllers
{
    public class TrifectaController : MonoBehaviour
    {

        public TrifectaStates defaultState = TrifectaStates.STATE_TNK;
        private TrifectaStates state;
        public Image currentSprite;
        public AudioSource trifectaSound;
        public Animator trifectaAnimator;
        private Dictionary<TrifectaStates, Action> codeToAnimactions;
        private Dictionary<TrifectaStates, Action> codeFromAnimactions;
        private Dictionary<string, TrifectaStates> buttonChecksMapping;
        private Dictionary<TrifectaStates, int> codeToStateIndex;
        public GameObject playerTank;

        private float toTurCameraZoom = 130.0f;
        private float regularCameraZoom;
        const float ANIMATION_LENGTH = 1.0f;

        public TrifectaStates CurrentState
        {
            get
            {
                return state;
            }
            set
            {
                trifectaAnimator.SetTrigger(AnimationParameters.TRIGGER_TRIFECTA_RESET);
                trifectaAnimator.SetInteger(AnimationParameters.INT_TRIFECTA_NEXT_STATE, codeToStateIndex[value]);
                ResolveTankAnimations(state, value);
                trifectaSound.Play();
                state = value;
            }
        }

        // Use this for initialization
        void Start()
        {
            regularCameraZoom = Camera.main.orthographicSize;
            buttonChecksMapping = new Dictionary<string, TrifectaStates>()
            {
                { ControlsButtonNames.BTN_NAME_TRIFECTA_REC, TrifectaStates.STATE_REC},
                { ControlsButtonNames.BTN_NAME_TRIFECTA_TNK, TrifectaStates.STATE_TNK},
                { ControlsButtonNames.BTN_NAME_TRIFECTA_TUR, TrifectaStates.STATE_TUR}
            };

            codeToStateIndex = new Dictionary<TrifectaStates, int>()
            {
                { TrifectaStates.STATE_TNK, 1 },
                { TrifectaStates.STATE_REC, 3 },
                { TrifectaStates.STATE_TUR, 2 }
            };

            codeToAnimactions = new Dictionary<TrifectaStates, Action>()
            {
                { TrifectaStates.STATE_REC, () =>  {
                    ReloadWeapons();
                    ToggleWeaponVisibility(false);
                }},
                { TrifectaStates.STATE_TUR, () =>  {
                    //set high turret body mass on transform
                    var body = playerTank.GetComponent<Rigidbody2D>();
                    body.mass = 9999.99f;
                    body.drag = 99.99f;
                    var engine = playerTank.GetComponentInChildren<TankEngineController>();
                    engine.isMoving = false;
                    engine.Model.currentAcceleration = 0.0f;
                    playerTank.GetComponent<PlayerController>().commands.Enqueue(
                        TankCommand.OneParamCommand(
                            TankCommandWords.TANK_COMMAND_BRAKE, TankCommandParamKeys.TANK_CMD_APPLY_BREAK_KEY, false));
                    ToggleTracksRotated(true);
                    ToggleCameraZoomChange(toTurCameraZoom);
                }}
            };

            codeFromAnimactions = new Dictionary<TrifectaStates, Action>()
            {
                { TrifectaStates.STATE_REC, () => {
                    ToggleWeaponVisibility(true);
                } },
                { TrifectaStates.STATE_TUR, () =>  {
                    //unset high turret mass post transform
                    var body = playerTank.GetComponent<Rigidbody2D>();
                    body.mass = Utils.CurrentState.Instance.CurrentTank.Mass;
                    body.drag = Utils.CurrentState.Instance.CurrentTank.TankTracks.Coupling;
                    ToggleTracksRotated(false);
                    ToggleCameraZoomChange(regularCameraZoom);
                }}
            };

            CurrentState = defaultState;
        }

        private void ToggleCameraZoomChange(float newZoom)
        {

            Timing.RunCoroutine(_PerformCameraZoomChange(Camera.main.GetComponent<CameraFollowObjectController>(), newZoom));
        }

        private void ReloadWeapons()
        {
            var controller = playerTank.GetComponent<PlayerController>();
            //issue reload command
            controller.commands.Enqueue(new TankCommand(TankCommandWords.TANK_COMMAND_RELOAD));
        }

        private void ToggleTracksRotated(bool turret)
        {
            var tracks = playerTank.GetComponentInChildren<TankTracksController>();
            var initialPos = tracks.Model.OnTankPosition.position;
            var newPosition = turret ?
            new Vector3(initialPos.x, initialPos.y * -1.0f, initialPos.z)
            : initialPos;

            var newRotation = turret ?
            Quaternion.Euler(0.0f, 0.0f, 90.0f)
            : Quaternion.Euler(Vector3.zero);

            Timing.RunCoroutine(_PerformTracksRotation(tracks, newPosition, newRotation));
        }

        private void ToggleWeaponVisibility(bool visible)
        {
            var turret = playerTank.GetComponentInChildren<TankTurretController>();
            var weapons = turret.GetComponentsInChildren<BaseWeaponController>();

            //do invisibility animation, slowly push weapon into turret and render behind
            //also make it invisible slowly
            foreach (var weapon in weapons)
            {
                weapon.weaponSpriteRenderer.sortingOrder = visible ?
                    SortingLayerConstants.WEAPON_DEFAULT_LAYER_ORDER
                    : turret.gameObject.GetComponent<SpriteRenderer>().sortingOrder - 1;
                Timing.RunCoroutine(_PerformWeaponHideMovement(
                    weapon,
                    visible ? weapon.WeaponSlot.ArenaTransform.position : Vector3.zero,
                    !visible)
                );
            }

        }

        private IEnumerator<float> _PerformCameraZoomChange(CameraFollowObjectController camera, float newZoom)
        {
            var completion = 0.0f;
            while (completion < ANIMATION_LENGTH)
            {
                camera.SetOrthographicSize(Mathf.Lerp(camera.GetOrthographicSize(), newZoom, Mathf.SmoothStep(0.0f, 1.0f, completion)));
                completion += Timing.DeltaTime;
                yield return Timing.WaitForSeconds(Timing.DeltaTime);
            }

            camera.SetOrthographicSize(newZoom);
        }

        private IEnumerator<float> _PerformTracksRotation(TankTracksController tracks, Vector3 toPos, Quaternion toRot)
        {
            var completion = 0.0f;
            while (completion < ANIMATION_LENGTH)
            {
                tracks.transform.localRotation = Quaternion.Lerp(tracks.transform.localRotation, toRot, Mathf.SmoothStep(0.0f, 1.0f, completion));
                tracks.transform.localPosition = Vector3.Lerp(tracks.transform.localPosition, toPos, Mathf.SmoothStep(0.0f, 1.0f, completion));
                completion += Timing.DeltaTime;
                yield return Timing.WaitForSeconds(Timing.DeltaTime);
            }

            tracks.transform.localRotation = toRot;
            tracks.transform.localPosition = toPos;
        }

        private IEnumerator<float> _PerformWeaponHideMovement(BaseWeaponController weapon, Vector3 to, bool hide)
        {

            var colorTo = hide ? Color.clear : Color.white;
            var completion = 0.0f;
            while (completion < ANIMATION_LENGTH)
            {
                weapon.weaponSpriteRenderer.color = Color.Lerp(weapon.weaponSpriteRenderer.color, colorTo, Mathf.SmoothStep(0.0f, 1.0f, completion));
                weapon.transform.localPosition = Vector3.Lerp(weapon.transform.localPosition, to, Mathf.SmoothStep(0.0f, 1.0f, completion));
                completion += Timing.DeltaTime;
                yield return Timing.WaitForSeconds(Timing.DeltaTime);
            }

            weapon.transform.localPosition = to;
            weapon.weaponSpriteRenderer.color = colorTo;
            // weapon.weaponSpriteRenderer.enabled = !hide;
        }

        // Update current trifecta mode
        void Update()
        {
            foreach (var mapping in buttonChecksMapping) { checkAndChangeMode(mapping.Key, mapping.Value); }
        }

        private void checkAndChangeMode(string buttonName, TrifectaStates neededState)
        {
            var isPressed = Input.GetButton(buttonName);
            if (isPressed && CurrentState != neededState)
            {
                CurrentState = neededState;
            }
        }

        private void ResolveTankAnimations(TrifectaStates oldState, TrifectaStates newState)
        {
            if (codeFromAnimactions.ContainsKey(oldState))
            {
                codeFromAnimactions[oldState]();
            }
            if (codeToAnimactions.ContainsKey(newState))
            {
                codeToAnimactions[newState]();
            }
        }
    }

}
