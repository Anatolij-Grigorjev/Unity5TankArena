using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TankArena.Models;
using TankArena.Models.Tank;
using TankArena.Models.Weapons;
using System;
using System.Text;
using System.Collections.Generic;
using TankArena.Constants;
using TankArena.Utils;
using UnityEngine.Events;

namespace TankArena.UI.Shop
{

	public class DetailedItemController : MonoBehaviour {

		public Image itemImage;
		public Text itemDescriptionText;
		public Text itemLabelText;
		public Image itemLabelImage;
		public Button buyItemButton;
		public AudioSource purchaseSound;
		public GameObject usualSwitchButton;
		public GameObject backToItemsButton;
		public GameObject msgBox;
		public TopLevelShopController parentController;
		public WeaponsShopLoadoutController weaponsShopLoadoutController;
		public Text msgBoxText;
		Tank CurrentLoadout;

		private FileLoadedEntityModel data;

		// Use this for initialization
		void Start ()
		{

		}
		
		// Update is called once per frame
		void Update ()
		{
		
		}
		private UnityAction createBaseWeaponPurchaseAction()
		{
			return () => 
			{
				var cash = EntitiesStore.Instance.Player.Cash;
				
				//player can buy this
				if (cash >= data.Price)
				{
					//check if person has the right slots of type
					BaseWeapon dataWeapon = (BaseWeapon)this.data;
					var slotObjects = GameObject.FindGameObjectsWithTag(
						dataWeapon.Type == WeaponTypes.HEAVY? Tags.TAG_SHOP_HEAVY_SLOT : Tags.TAG_SHOP_LIGHT_SLOT
					);
					if (slotObjects == null || slotObjects.Length <= 0) 
					{
						DisplayMessageBox(UIShopButtonTexts.SHOP_NO_APPROPRIATE_SLOTS);
						return;
					}

					//rename button to let them know what to do
					var prevText = itemLabelText.text;
					itemLabelText.text = UIShopButtonTexts.SHOP_CHOSE_WEAPON_SLOT;

					//glow the GOs
					foreach(GameObject slotGo in slotObjects)
					{
						var slotAnimator = slotGo.GetComponent<Animator>();
						slotAnimator.SetTrigger(AnimationParameters.TRIGGER_GLOW_SLOT);
						var slotButton = slotGo.AddComponent<Button>();
						//slot was selected
						slotButton.onClick.AddListener(() => 
						{
							//stop animation
							slotAnimator.SetTrigger(AnimationParameters.TRIGGER_GLOW_SLOT);
							//perform purchase sound
							purchaseSound.Play();
							//change weapon in the right slot
							WeaponSlot slot = weaponsShopLoadoutController.slotsGOs[slotGo];
							//sell the old weapon first
							if (slot.Weapon != null)
							{
								EntitiesStore.Instance.Player.Cash += slot.Weapon.Price;
							}
							slot.Weapon = dataWeapon;
							EntitiesStore.Instance.Player.Cash -= dataWeapon.Price;
							//restore details button text
							itemLabelText.text = prevText;

							parentController.RefreshUI();

							Destroy(slotButton);
						});
					}

				} else {
					DisplayMessageBox(UIShopButtonTexts.SHOP_NOT_ENOUGH_MSG_BOX);		
				}
			};
		}
		private UnityAction createTankPartPurchaseAction()
		{
			return () =>
			{
				var cash = EntitiesStore.Instance.Player.Cash;
				
				//player can buy this
				if (cash >= data.Price)
				{
					if (EntitiesStore.Instance.CurrentTank.HasPart((TankPart)this.data))
					{
						DisplayMessageBox(UIShopButtonTexts.SHOP_ALREADY_HAVE_PART_MSG_BOX);
						return;
					}
					purchaseSound.Play();
					var oldPart = SlotInNewPart((TankPart)data);
					var pricediff = data.Price - oldPart.Price;
					EntitiesStore.Instance.Player.Cash -= pricediff;
					parentController.RefreshUI();
				} else {
					DisplayMessageBox(UIShopButtonTexts.SHOP_NOT_ENOUGH_MSG_BOX);		
				}
			};
		}

		private void DisplayMessageBox(String message)
		{
			msgBoxText.text = message;
			msgBox.SetActive(true);
		}

		//Slot in new part and return old one
        private TankPart SlotInNewPart(TankPart newPart)
        {
            var Current = EntitiesStore.Instance.CurrentTank;

			var dataType = newPart.GetType();
			if (typeof(TankChassis).IsAssignableFrom(dataType))
			{
				var oldChassis = Current.TankChassis;
				Current.TankChassis = (TankChassis)newPart;
				return oldChassis;
			} else if (typeof(TankTurret).IsAssignableFrom(dataType))
			{
				var oldTurret = Current.TankTurret;
				Current.TankTurret = (TankTurret)newPart;
				return oldTurret;
			} else if (typeof(TankEngine).IsAssignableFrom(dataType))
			{
				var oldEngine = Current.TankEngine;
				Current.TankEngine = (TankEngine)newPart;
				return oldEngine;
			} else if (typeof(TankTracks).IsAssignableFrom(dataType))
			{
				var oldTracks = Current.TankTracks;
				Current.TankTracks = (TankTracks)newPart;
				return oldTracks;
			} else 
			{
				string error = String.Format("SOMETHING IS VERY WRONG!!! PERSON PAID FOR TANK PART THAT IS NOT TANK PART!!!"+
					" THING: {0}, TYPE: {1}", newPart, dataType);
				DBG.Log(error);
				DisplayMessageBox(error);
			}
			
			return newPart;
        }

        public void SetItem(FileLoadedEntityModel entity)
		{	
			CurrentLoadout = EntitiesStore.Instance.CurrentTank;
			//remove listeners before we add the right one
			buyItemButton.onClick.RemoveAllListeners();

			usualSwitchButton.SetActive(false);
			backToItemsButton.SetActive(true);
			this.data = entity;
			var dataType = data.GetType();

			var labelColor = UIShopItems.ITEM_LABEL_COLOR_OTHER;
			var labelText = UIShopItems.ITEM_LABEL_TEXT_OTHER;
			Sprite itemSprite = null;

			if (typeof(TankPart).IsAssignableFrom(dataType))
			{
				//this is a tank part
				TankPart dataPart = (TankPart)this.data;
				itemSprite = dataPart.ShopItem;
				if (typeof(TankChassis).IsAssignableFrom(dataType))
				{
					labelColor = UIShopItems.ITEM_LABEL_COLOR_TANK_PART_CHASSIS;
					labelText = UIShopItems.ITEM_LABEL_TEXT_TANK_PART_CHASSIS;
				} else if (typeof(TankTurret).IsAssignableFrom(dataType))
				{
					labelColor = UIShopItems.ITEM_LABEL_COLOR_TANK_PART_TURRET;
					labelText = UIShopItems.ITEM_LABEL_TEXT_TANK_PART_TURRET;
				} else if (typeof(TankEngine).IsAssignableFrom(dataType))
				{
					labelColor = UIShopItems.ITEM_LABEL_COLOR_TANK_PART_ENGINE;
					labelText = UIShopItems.ITEM_LABEL_TEXT_TANK_PART_ENGINE;
				} else if (typeof(TankTracks).IsAssignableFrom(dataType))
				{
					labelColor = UIShopItems.ITEM_LABEL_COLOR_TANK_PART_TRACKS;
					labelText = UIShopItems.ITEM_LABEL_TEXT_TANK_PART_TRACKS;
				}
				
				buyItemButton.onClick.AddListener(createTankPartPurchaseAction());

			} else if (typeof(BaseWeapon).IsAssignableFrom(dataType))
			{
				//this is a weapon
				BaseWeapon dataWeapon = (BaseWeapon)this.data;
				itemSprite = dataWeapon.ShopItem;

				labelColor = dataWeapon.Type == WeaponTypes.HEAVY? 
					UIShopItems.ITEM_LABEL_COLOR_WEAPON_HEAVY : UIShopItems.ITEM_LABEL_COLOR_WEAPON_LIGHT;
				labelText = dataWeapon.Type == WeaponTypes.HEAVY?
					UIShopItems.ITEM_LABEL_TEXT_WEAPON_HEAVY : UIShopItems.ITEM_LABEL_TEXT_WEAPON_LIGHT;

				buyItemButton.onClick.AddListener(createBaseWeaponPurchaseAction());
			} 

			itemLabelImage.color = labelColor;
			itemLabelText.text = labelText + String.Format(" ({0}$)", entity.Price);
			if (itemSprite != null)
			{
				itemImage.sprite = itemSprite;
			}
			itemDescriptionText.text = StringifyProperties(this.data);
		}

        private string StringifyProperties(FileLoadedEntityModel data)
        {
			bool isCompareable = typeof(TankPart).IsAssignableFrom(data.GetType()) && CurrentLoadout != null;

			StringBuilder builder = new StringBuilder();
			var displayKeys = EntityKeys.ENTITY_KEYS_DISPLAY_MAP;
			foreach(String key in data.properties.Keys)
			{
				if (displayKeys.ContainsKey(key)) 
				{
					var displayValue = GetDisplayValue(data.properties[key]);
					builder.Append(displayKeys[key]);
					builder.Append(": ");
					builder.Append(displayValue);

					//only compare Tank Parts
					if (isCompareable)
					{
						//is comparable, can display comparison info
						float diff = GetDiff((TankPart)data, key);
						if (diff != 0.0) 
						{
							//diff not zero, so makes sense to compare visually
							builder.Append(" (");
							bool isBelow = diff < 0.0;
							builder.Append(isBelow? '▼' : '▲');
							if (!isBelow) builder.Append('+');
							builder.Append(diff);
							builder.Append(")");
						}
					}
					builder.Append("\n");
				}
			}


			return builder.ToString();
        }

        private object GetDisplayValue(object v)
        {
			//return value unkown thing
			if (v == null) 
			{
				return UIShopItems.ITEM_PROPERTY_UNKNOWN;
			}
			var vType = v.GetType();
			
			//this is a collection, take length instead of collection itself
			if (typeof(ICollection).IsAssignableFrom(vType))
			{
				
				return ((ICollection)v).Count;
			}

			//return value as-is
			return v;
        }

        private float GetDiff(TankPart data, string key)
        {
			if (CurrentLoadout == null) 
			{
				return 0.0f;
			}

			TankPart currentPart = null;
			var dataType = data.GetType();

			if (typeof(TankChassis).IsAssignableFrom(dataType)) 
				currentPart = CurrentLoadout.TankChassis; 
			else if (typeof(TankTurret).IsAssignableFrom(dataType)) 
				currentPart = CurrentLoadout.TankTurret;
			else if (typeof(TankEngine).IsAssignableFrom(dataType)) 
				currentPart = CurrentLoadout.TankEngine;
			else if (typeof(TankTracks).IsAssignableFrom(dataType)) 
				currentPart = CurrentLoadout.TankTracks;

			if (currentPart == null) 
			{
				return 0.0f;
			}

			var thisValue = GetDisplayValue(data.properties[key]);
			var currentValue = GetDisplayValue(currentPart.properties[key]);

			if (currentValue.GetType().IsAssignableFrom(typeof(int)) ||
				currentValue.GetType().IsAssignableFrom(typeof(float)))
				{
					if (currentValue.GetType().IsAssignableFrom(typeof(int)))
					{
						return (int)thisValue - (int)currentValue;
					} else 
					{
						return (float)thisValue - (float)currentValue;
					}
				} else 
				{
					return 0.0f;
				}
        }
    }
}
