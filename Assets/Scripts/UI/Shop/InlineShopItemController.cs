using UnityEngine;
using System.Collections;
using TankArena.Models;
using TankArena.Utils;

namespace TankArena.UI.Shop
{

	public class InlineShopItemController: MonoBehaviour {

		public DetailedItemController detailsPaneGO;
		private FileLoadedEntityModel data;

		public FileLoadedEntityModel Data
		{
			get 
			{
				return data;
			}
			set 
			{
				data = value;
			}
		}

		// Use this for initialization
		void Start () {
		
		}
		
		/// <summary>
		/// Called every frame while the mouse is over the GUIElement or Collider.
		/// </summary>
		public void OnItemClicked()
		{
			
			// if (ContentPaneGO.activeInHierarchy) 
			// {
			// 	ContentPaneGO.SetActive(false);
			// }
			if (!detailsPaneGO.gameObject.activeInHierarchy)
			{
				detailsPaneGO.gameObject.SetActive(true);
			}
			detailsPaneGO.SetItem(Data);
			
		}
	}
}
