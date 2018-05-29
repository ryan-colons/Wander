using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : MonoBehaviour {

	public const string spellTag = "ConductsMagic";
	public static GameObject projectileSpawn;

	public virtual void Cast () {
		// spell happens
	}

}
