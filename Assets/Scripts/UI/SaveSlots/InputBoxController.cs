using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace TankArena.UI 
{
	public class InputBoxController : MonoBehaviour {

		public delegate void ProcessName(string name);

		public Button cancelButton;
		public Button enterButton;
		public InputField inputField;
		public string inputName;
		public ProcessName externalAction;

		// Use this for initialization
		void Start () 
		{

			inputField.onEndEdit.AddListener(UpdateName);
			inputField.onValueChanged.AddListener(UpdateName);

		}

		public void EnterClicked()
		{
			gameObject.SetActive(false);
			if (externalAction != null) 
			{
				externalAction(inputName);
			}
		}

		public void CancelClicked()
		{
			gameObject.SetActive(false);
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

