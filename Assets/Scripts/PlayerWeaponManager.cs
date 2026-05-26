using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private GameObject initialWeaponPrefab;
    [SerializeField] private Weapon currentWeapon;

    private void Start()
    {
        if (initialWeaponPrefab != null)
        {
            EquipWeapon(initialWeaponPrefab);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
			Debug.Log("PlayerWeaponManager: Attack input detected");
            Attack();
        }
    }

    public void Attack()
    {
        if (currentWeapon != null)
        {
            currentWeapon.Attack();
        }
        else
        {
            Debug.LogWarning("No weapon equipped!");
        }
    }

    public void EquipWeapon(GameObject weaponPrefab)
    {
        // // Destroy current weapon if any
        // if (currentWeapon != null)
        // {
        //     Destroy(currentWeapon.gameObject);
        // }

        if (weaponPrefab == null) return;

        GameObject newWeaponObj = Instantiate(weaponPrefab, weaponHolder != null ? weaponHolder : transform);
        newWeaponObj.transform.localPosition = Vector3.zero;
        newWeaponObj.transform.localRotation = Quaternion.identity;
        
        currentWeapon = newWeaponObj.GetComponent<Weapon>();
        
        if (currentWeapon == null)
        {
            Debug.LogError("Equipped prefab does not have a Weapon component!");
        }
    }
}
