using UnityEngine;

[CreateAssetMenu(menuName = "Entities/Zombies", fileName = "New Zombie")]
public class ZombieScriptableObject : ScriptableObject
{
	public GameObject zombieDefault;
	public GameObject zombieAccessory;

	public ZombieType zombieType;

	public float accessoryHealth;
	public float zombieHealth;
	public float zombieHandHealth;
	public float zombieDamage;
	public float zombieSpeed;
	public float attackInterval;

	public bool ovveridDefaultSprite;
	public bool useChildObjects = true;
	public float jumpDuration = 1f;
	public float jumpPower;
	public Sprite newSprite;

	[Range(0, 100)]
	public float coinDropPercent;

	public enum ZombieType
	{
		Normal,
		ConeZombie,
		BucketZombie,
		PoleVaulter
	}
}