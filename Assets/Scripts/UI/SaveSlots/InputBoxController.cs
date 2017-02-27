using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TankArena.Utils;

namespace TankArena.UI 
{
	public class InputBoxController : MonoBehaviour {

		public delegate void ProcessName(string name);

		public Button cancelButton;
		public Button enterButton;
		public InputField inputField;
		public string inputName;
		public ProcessName externalAction;

		private GameObject[] slotGOs;

		// Use this for initialization
		void Start () 
		{
			DBG.Log("Starting input dialogue!");
			if (inputField != null) 
			{
				inputField.onEndEdit.RemoveAllListeners();
				inputField.onEndEdit.AddListener(UpdateName);
				inputField.onValueChanged.RemoveAllListeners();
				inputField.onValueChanged.AddListener(UpdateName);
			}
			slotGOs = new GameObject[3];
			//disable save slots to ensure correct click
			foreach(int slotNum in new int[] {1, 2, 3})
			{
				slotGOs[slotNum - 1] = GameObject.Find("SaveSlot" + slotNum);
			}
			ToggleSaveSlotButtons(false);
			
		}

		private void ToggleSaveSlotButtons(bool enable)
		{
			foreach(GameObject slotGO in slotGOs)
			{
				var button = slotGO.GetComponent<Button>();
				if (button != null)
				{
					button.enabled = enable;
				}
			}
		}

		public void EnterClicked()
		{
			gameObject.SetActive(false);
			ToggleSaveSlotButtons(true);
			if (externalAction != null) 
			{
				externalAction(inputName);
			}
		}

		public void OKClicked() 
		{
			gameObject.SetActive(false);
			ToggleSaveSlotButtons(true);
			if (externalAction != null) 
			{
				externalAction("true");
			}
		}

		public void CancelClicked()
		{
			gameObject.SetActive(false);
			ToggleSaveSlotButtons(true);
			if (externalAction != null) 
			{
				externalAction(null);
			}
		}

		public void UpdateName(string name)
		{
			inputName = name;
		}
		
		// Update is called once per frame
		void Update () 
		{
		
		}
	}
}

