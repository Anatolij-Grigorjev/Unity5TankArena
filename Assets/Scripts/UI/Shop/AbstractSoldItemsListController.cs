using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using TankArena.Models;
using TankArena.Constants;

public abstract class AbstractSoldItemsListController<T> : MonoBehaviour where T: FileLoadedEntityModel
{
	private Dictionary<T, GameObject> visibleObjectsMap;
	
	public Transform parentContainer;

	// Use this for initialization
	void Start () 
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
			visibleObjectsMap[item] = MakeGOForItem(item);
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
				visibleObjectsMap[item] = MakeGOForItem(item);
			}		
		}

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
