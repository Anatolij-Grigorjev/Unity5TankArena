﻿using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using TankArena.Models;
using TankArena.Constants;
using TankArena.Utils;

public abstract class AbstractSoldItemsListController<T> : MonoBehaviour where T: FileLoadedEntityModel
{
	private Dictionary<T, GameObject> visibleObjectsMap;
	
	public RectTransform parentContainer;

	// Use this for initialization
	void Awake () 
	{
		visibleObjectsMap = new Dictionary<T, GameObject>();
		UpdateListView();
	}

	public void SetItems(List<T> items) 
	{
		//add the GO n stuff to the data
		foreach(T item in items) 
		{
			AddItem(item);
		}

		//refresh the actual content pane
		UpdateListView();
	}

	//Add item with required GO
	public void AddItem(T item)
	{
		//no need to do anything if item already there
		if (!ItemInList(item))
		{
			visibleObjectsMap.Add(item, MakeGOForItem(item));
		}
	}

	protected static void SetGOHeight(GameObject theGO)
	{
		var goTransform = theGO.transform as RectTransform;
		goTransform.sizeDelta = new Vector2(0, UIShopItems.PREFERRED_ITEM_HEIGHT);
	}

	protected static void SetGODescription(GameObject theGO, T item)
	{
		var textBorderImageChild = 
				theGO.GetComponentsInChildren<Image>()
				.Where(image => image.CompareTag(Tags.TAG_UI_SHOP_ITEM_TEXT_PARENT))
				.First();
			
		if (textBorderImageChild != null) 
		{
			var imageText = textBorderImageChild.GetComponentInChildren<Text>();
			imageText.text = string.Format(
				"{0} (${1})",
				item.Name,
				item.Price
			);
		}
	}

	//remove item and required GO
	public void RemoveItem(T item) 
	{
		if (ItemInList(item))
		{
			var go = visibleObjectsMap[item];
			Destroy(go, UIShopTiming.SHOP_ITEM_FADEAWAY_SEC);
		}
	}

	//update how it looks onscreen
	private void UpdateListView()
	{
		foreach (T item in visibleObjectsMap.Keys) 
		{
			if (!ItemInList(item))
			{
				visibleObjectsMap.Add(item, MakeGOForItem(item));
			}		
		}
		
		//set content pane height for scrolling
		parentContainer.sizeDelta = new Vector2(0, 
			UIShopItems.PREFERRED_ITEM_HEIGHT * visibleObjectsMap.Values.Count);
	}

	public abstract GameObject MakeGOForItem(T item);

	//key and GO are present
	private bool ItemInList(T item)
	{
		if (item == null || string.IsNullOrEmpty(item.Id)) 
		{
			return false;
		}
		if (visibleObjectsMap == null || visibleObjectsMap.Count == 0)
		{
			return false;
		}
		return visibleObjectsMap.ContainsKey(item) 
			&& visibleObjectsMap[item] != null
			&& visibleObjectsMap[item].transform.parent == parentContainer;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
