using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankArena.Models
{
	public interface IDamageReceiver {

		 void ApplyDamage(GameObject damager);
		 void ApplyDamage(float damage);

	}

}
