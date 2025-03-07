using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Transform weaponHolder;
    public WeaponData equippedWeapon;
    private GameObject currentWeaponInstance;
    public float dropForce = 5f;
    public Vector3 equippedPosition = new Vector3(0.42f, -0.21f, 0.51f);
    public Vector3 equippedRotation = new Vector3(0f, -90f, 0f);

    void Start()
    {
        if (equippedWeapon != null)
        {
            EquipWeapon(equippedWeapon);
        }
    }

    void Update()
{
    if (Input.GetKeyDown(KeyCode.G))
    {
        UnequipWeapon();
    }
}


    public void EquipWeapon(WeaponData newWeapon)
    {
        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
        }

        equippedWeapon = newWeapon;
        currentWeaponInstance = Instantiate(equippedWeapon.weaponPrefab, weaponHolder);
        currentWeaponInstance.transform.localPosition = equippedPosition;
        currentWeaponInstance.transform.localRotation = Quaternion.Euler(equippedRotation);
    }

   public void UnequipWeapon()
{
    if (currentWeaponInstance != null)
    {
        currentWeaponInstance.transform.parent = null;
        
        Rigidbody rb = currentWeaponInstance.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = currentWeaponInstance.AddComponent<Rigidbody>();
        }
        rb.AddForce(weaponHolder.forward * dropForce, ForceMode.Impulse);

        WeaponPickup pickupScript = currentWeaponInstance.GetComponent<WeaponPickup>();
        if (pickupScript == null)
        {
            pickupScript = currentWeaponInstance.AddComponent<WeaponPickup>();
        }
        pickupScript.weaponData = equippedWeapon;

        currentWeaponInstance = null;
        equippedWeapon = null;
    }
}

}
