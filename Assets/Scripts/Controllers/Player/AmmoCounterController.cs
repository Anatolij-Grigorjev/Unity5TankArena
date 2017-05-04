using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TankArena.Models.Weapons;
using TankArena.Controllers.Weapons;

namespace TankArena.Controllers
{ 
    public class AmmoCounterController : MonoBehaviour
    {

        private readonly Color ACTIVE_WPN_COLOR = Color.green;
        private readonly Color RELOAD_WPN_COLOR = Color.cyan;
        private readonly Color INACTIVE_WPN_COLOR = Color.yellow;
        public const  float IMAGE_HEIGHT = 50.0f;

        public Image sliderForeground;
        public Slider sliderCore;
        public Image weaponAvatar;
        public int weaponIndex = 0;
        public BaseWeaponController weaponController;

        // Use this for initialization
        void Start () {
            
        }
        
        // Update is called once per frame
        void Update () {
        
        }

        public void SetProgress(float newVal)
        {
            sliderCore.value = newVal;
        }

        public void StartUsage()
        {
            sliderCore.wholeNumbers = true;
            sliderCore.maxValue = weaponController.clipSize;
            sliderCore.value = weaponController.currentClipSize;
            sliderForeground.color =  
                weaponController.currentClipSize > 0? ACTIVE_WPN_COLOR : RELOAD_WPN_COLOR;
        }

        public void StartReload()
        {
            sliderCore.wholeNumbers = false;
            sliderCore.maxValue = weaponController.reloadTime;
            sliderCore.value = 0.0f;
            sliderForeground.color = RELOAD_WPN_COLOR;
        }

        public void SetInactive(bool inactive)
        {
            sliderForeground.color = inactive? INACTIVE_WPN_COLOR : ACTIVE_WPN_COLOR;
        }

    }
}
