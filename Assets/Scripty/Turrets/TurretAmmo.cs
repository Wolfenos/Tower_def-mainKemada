using UnityEngine;

namespace KemadaTD
{
    public class TurretAmmo : MonoBehaviour
    {
        public int maxAmmo = 10;   // Maximum ammo the turret can hold
        private int currentAmmo;   // Current ammo count
        public float reloadTime = 5f; // Time to reload ammo (optional)
        private bool hasAmmo = true; // Flag for whether the turret has ammo

        // Reference to the UIManager to update the ammo UI
        public UIManager uiManager;

        private void Start()
        {
            currentAmmo = maxAmmo; // Initialize with full ammo
            UpdateUI(); // Update the UI when the game starts
        }

        // Call this method when the turret shoots
        public bool UseAmmo()
        {
            // Only reduce ammo if there's ammo available
            if (hasAmmo && currentAmmo > 0)
            {
                currentAmmo--; // Decrease ammo by 1
                UpdateUI(); // Update the UI with the new ammo count

                if (currentAmmo <= 0)
                {
                    hasAmmo = false;
                    StopShooting(); // Stop the turret from shooting if out of ammo
                }
                return true;
            }
            return false;
        }

        // Stop the turret from shooting when it runs out of ammo
        private void StopShooting()
        {
            Turret turret = GetComponent<Turret>();
            if (turret != null)
            {
                turret.canShoot = false; // Assuming the turret script has a canShoot flag
            }
        }

        // Reload the ammo (optional, can be used to implement a reload feature)
        public void ReloadAmmo()
        {
            currentAmmo = maxAmmo;
            hasAmmo = true;
            UpdateUI();
        }

        // Updates the UI with the current ammo count
        private void UpdateUI()
        {
            if (uiManager != null)
            {
                uiManager.UpdateAmmoUI(currentAmmo, maxAmmo);
            }
        }
    }
}