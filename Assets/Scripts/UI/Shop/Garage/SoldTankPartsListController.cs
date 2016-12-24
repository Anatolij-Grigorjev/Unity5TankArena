using UnityEngine;
using System.Collections;
using TankArena.Models.Tank;
using System;

namespace TankArena.UI.Shop
{
	public class SoldTankPartsListController : AbstractSoldItemsListController<TankPart> {

		public GameObject chassisItemPrefab;
		public GameObject turretItemPrefab;
		public GameObject engineItemPrefab;
		public GameObject tracksItemPrefab;

        public override GameObject MakeGOForItem(TankPart item)
        {
			if (item == null)
			{
				return null;
			}
			var itemType = item.GetType();
			if (itemType.IsAssignableFrom(typeof(TankChassis)))
			{
				throw new NotImplementedException("No shop item implemented for tank chassis!");
			} else if (itemType.IsAssignableFrom(typeof(TankTurret)))
			{
				throw new NotImplementedException("No shop item implemented for tank turret!");
			} else if (itemType.IsAssignableFrom(typeof(TankEngine)))
			{
				throw new NotImplementedException("No shop item implemented for tank engine!");
			} else if (itemType.IsAssignableFrom(typeof(TankTracks)))
			{
				throw new NotImplementedException("No shop item implemented for tank tracks!");
			} else 
			{
            	throw new NotImplementedException("No shop item implemented for unknown generic part!");
			}
        }

        // Use this for initialization
        void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}
