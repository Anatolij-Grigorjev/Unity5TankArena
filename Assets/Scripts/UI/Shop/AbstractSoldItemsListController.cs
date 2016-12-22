using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using TankArena.Models;

public class AbstractSoldItemsListController<T> : MonoBehaviour where T: FileLoadedEntityModel
{

	List<T> items;

	

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
