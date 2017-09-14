using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankArena.Constants
{
	public enum WeaponHitTypes  
	{
		/*
		Weapon hit type that describes weapons where the target gets hit by the prjectile proper
		and gets dmaagedfrom that, used in slower firing weapons
		 */
		PROJECTILE,
		/*
		Weapon hit type that describes where taget gets hit by a raycast with teh projectile being
		a decorative feedback visual, used in quick fire rate weapons
		 */
		TARGET,
		/*
		PROJECTILE hit type with the projectile itself being a non-atomic combination 
		of several simpler projectiles */
		COMPOSITE
		
	}

}
