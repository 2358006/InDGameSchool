using UnityEngine;
public class Weapon : MonoBehaviour
{
    public float weaponSpeed = 15f;
    public float weaponLife = 3f;
    public float weaponCooldown = 1f;
    public int weaponAmmo = 15;

    public GameObject weaponBullet;
    public Transform weaponFirePosition;
}
