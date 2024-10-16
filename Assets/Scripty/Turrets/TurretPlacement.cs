using UnityEngine;

namespace KemadaTD
{
    public class TurretPlacement : MonoBehaviour
    {
        public GameObject turretPrefab; // Prefab věže, kterou chceme postavit

        private void OnMouseDown()
        {
            // Zkontroluje, jestli na tomto místě již není věž
            if (transform.childCount == 0)
            {
                // Umístí věž na toto místo
                GameObject turret = Instantiate(turretPrefab, transform.position, Quaternion.identity);
                turret.transform.parent = transform; // Nastaví jako podřízený objekt pro lepší organizaci
            }
        }
    }
}