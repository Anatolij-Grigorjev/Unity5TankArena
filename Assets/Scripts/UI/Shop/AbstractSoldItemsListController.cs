using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using TankArena.Models;

public class AbstractSoldItemsListController<T> : MonoBehaviour where T: FileLoadedEntityModel
{

	private List<T> items;

	private List<GameObject> visibleObjectsList;
	

	// Use this for initialization
	void Start () 
	{
		UpdateList();
	}

	public void SetItems(List<T> items) 
	{
		this.items = items;
		UpdateList();
	}

	private void UpdateList()
	{
		//too costy to remove and add items, 
		//update list as we go along, remove non listed items later
		//all items need to be a common GO extension that has associated model id in it to 
		//prove things (model id could be in GO tag?!)
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
