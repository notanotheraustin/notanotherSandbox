using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public WeaponData weaponData;

    private void OnTriggerEnter(Collider other)
    {
        WeaponManager weaponManager = other.GetComponent<WeaponManager>();
        if (weaponManager != null)
        {
            weaponManager.EquipWeapon(weaponData);
            Destroy(gameObject);
        }
    }
}
