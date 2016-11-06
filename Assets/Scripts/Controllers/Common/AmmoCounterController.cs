using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TankArena.Models.Weapons;

public class AmmoCounterController : MonoBehaviour {

    private readonly Color ACTIVE_WPN_COLOR = Color.green;
    private readonly Color RELOAD_WPN_COLOR = Color.cyan;

    public Image sliderForeground;
    public Slider sliderCore;
    public Image weaponAvatar;
    

	// Use this for initialization
	void Awake () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetProgress(float newVal)
    {
        sliderCore.value = newVal;
    }

    public void SetWeapon(BaseWeapon weapon)
    {
        weaponAvatar.sprite = weapon.ShopItem;
        StartUsage(weapon);
    }

    public void StartUsage(BaseWeapon weapon)
    {
        sliderCore.wholeNumbers = true;
        sliderCore.maxValue = weapon.ClipSize;
        sliderCore.value = weapon.currentClipSize;
        sliderForeground.color =  weapon.currentClipSize > 0? ACTIVE_WPN_COLOR : RELOAD_WPN_COLOR;
    }

    public void StartReload(BaseWeapon weapon)
    {
        sliderCore.wholeNumbers = false;
        sliderCore.maxValue = weapon.ReloadTime;
        sliderCore.value = 0.0f;
        sliderForeground.color = RELOAD_WPN_COLOR;
    }



}
