using UnityEngine;
using TMPro; // Add the TextMeshPro namespace

namespace KemadaTD
{
    public class UIManager : MonoBehaviour
    {
        // Reference to the TMP_Text UI element for displaying ammo count
        public TMP_Text ammoText;

        // Method to update the ammo count in the UI
        public void UpdateAmmoUI(int currentAmmo, int maxAmmo)
        {
            // Ensure the ammoText reference is assigned before trying to update it
            if (ammoText != null)
            {
                // Update the text to display the current ammo count and max ammo
                ammoText.text = "Ammo: " + currentAmmo + "/" + maxAmmo;
            }
            else
            {
                Debug.LogError("Ammo TMP_Text UI element is not assigned in the UIManager.");
            }
        }
    }
}