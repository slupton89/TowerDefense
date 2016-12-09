using UnityEngine;

public enum ProjectileType {
	rock, arrow, fireball
};

public class Projectile : MonoBehaviour {

	[SerializeField]
	private int attackStrength;
	[SerializeField]
	private ProjectileType proType;

	public int AttackStrength {
		get {
			return attackStrength;
		}
	}

	public ProjectileType ProType {
		get {
			return proType;
		}
	}
}
