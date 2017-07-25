using System.Collections;
using System.Collections.Generic;
using TankArena.Utils;
using UnityEngine;

public class ObjectShakeController : MonoBehaviour {
	
	/// <summary>
	/// Controllers that impact the position of this GO, get disabled when this activates
	/// </summary>
	public List<MonoBehaviour> motionControlControllers; 
	/// <summary>
	/// Total movement jolts in a shake sequence
	/// </summary>
	public int totalShakes;
	private int currentShakesLeft;
	/// <summary>
	/// Movement intensity shift between shakes. 
	/// A value below 1.0 means the jolts gradually get smaller, a value above means they get larger
	/// </summary>
	public float shakeIntensifier;
	/// <summary>
	/// Largest initial movement, sets the base for the shakes to come. This value changes via the 
	/// shake intensifier between frames.
	/// </summary>
	public Vector2 maxSpread;
	private Vector2 currentMaxSpread;
	private Vector3 initialTransformPosition; // base position for shakes, recorded on enable

	
	void OnEnable()
	{
		ResetStateEnabled();
	}

	void Start () 
	{
		ResetStateDisabled();
	}
	
	void Update () 
	{
		if (currentShakesLeft > 0)
		{
			Vector3 jolt = Random.insideUnitCircle;
			jolt.x *= Mathf.Abs(currentMaxSpread.x);
			jolt.y *= Mathf.Abs(currentMaxSpread.y);
			this.transform.position = initialTransformPosition + jolt;

			currentShakesLeft--;
			currentMaxSpread *= shakeIntensifier;
		} else 
		{
			ResetStateDisabled();
		}
	}

	/// <summary>
	/// Set disabled shaker state, reset variables, enable motion controllers and disable this one
	/// </summary>
	private void ResetStateDisabled()
	{
		ResetVars();
		ToggleMotionControllers(true);
		this.enabled = false;
	}

	private void ResetVars()
	{
		currentShakesLeft = Random.Range(totalShakes / 2,(int)(totalShakes * 1.5f));
		currentMaxSpread = maxSpread;
	}

	private void ToggleMotionControllers(bool enable)
	{
		motionControlControllers.ForEach(controller => {
			if (!controller.enabled == enable) {
				controller.enabled = enable;
			}
		});
	}

	/// <summary>
	/// Set enabed shaker state, reset variables, disable motion controllers
	/// </summary>
	private void ResetStateEnabled() 
	{
		ResetVars();
		ToggleMotionControllers(false);
		initialTransformPosition = this.transform.position;
	}
}
