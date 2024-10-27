using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float cameraDragSpeed = 5f; // Rychlost posunu kamery
    public float cameraRotateSpeed = 100f; // Rychlost rotace kamery
    public float zoomSpeed = 5f; // Rychlost přiblížení a oddálení
    public Transform rotationPoint; // Bod rotace kamery (můžeš ho přiřadit v Inspectoru)

    // Definice hranic, aby byly nastavitelné v Inspectoru
    [Header("Boundary Settings")]
    public Vector2 xBounds = new Vector2(-50f, 50f); // Minimální a maximální hodnota pro osu X
    public Vector2 zBounds = new Vector2(-50f, 50f); // Minimální a maximální hodnota pro osu Z
    public Vector2 yBounds = new Vector2(10f, 50f);  // Minimální a maximální hodnota pro osu Y (pro omezení výšky kamery)

    private Vector3 dragOrigin; // Pozice myši při začátku tažení

    void Update()
    {
        // Kontrola kliknutí levým tlačítkem myši
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Detekce kliknutí na objekt
                Debug.Log("Kliknuto na objekt: " + hit.transform.name);
            }
            else
            {
                Debug.Log("Raycast nedetekoval žádný objekt.");
            }
        }

        // Rotace kamery pomocí prostředního tlačítka myši
        if (Input.GetMouseButton(2)) // 2 je prostřední tlačítko myši
        {
            float rotateX = Input.GetAxis("Mouse X") * cameraRotateSpeed * Time.deltaTime;
            float rotateY = Input.GetAxis("Mouse Y") * cameraRotateSpeed * Time.deltaTime;

            // Rotuj kolem bodu
            Camera.main.transform.RotateAround(rotationPoint.position, Vector3.up, rotateX); // Rotace kolem Y osy
            Camera.main.transform.RotateAround(rotationPoint.position, Camera.main.transform.right, -rotateY); // Rotace kolem X osy
        }

        // Pohyb kamery pomocí W, A, S, D
        float moveHorizontal = Input.GetAxis("Horizontal"); // A, D
        float moveVertical = Input.GetAxis("Vertical"); // W, S

        // Určení směru pohybu na základě orientace kamery
        Vector3 forward = Camera.main.transform.forward; // Vektor směřující vpřed
        Vector3 right = Camera.main.transform.right; // Vektor směřující vpravo

        forward.y = 0; // Zabráníme, aby pohyb byl vertikální
        right.y = 0; // Zabráníme, aby pohyb byl vertikální
        forward.Normalize(); // Normalizace vektoru vpřed
        right.Normalize(); // Normalizace vektoru vpravo

        // Vypočtení celkového pohybu na základě W, A, S, D
        Vector3 move = (forward * moveVertical + right * moveHorizontal) * cameraDragSpeed * Time.deltaTime;

        // Posun kamery
        Camera.main.transform.position += move;

        // Zoomování kamery pomocí scrollování myši
        float scrollInput = Input.GetAxis("Mouse ScrollWheel"); // Získání hodnoty scrollu
        Camera.main.transform.position += Camera.main.transform.forward * scrollInput * zoomSpeed; // Přiblížení a oddálení

        // Kamera se začne hýbat, když podrží pravé tlačítko myši
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        // Kamera se posouvá, pokud držíme pravé tlačítko myši
        if (Input.GetMouseButton(1))
        {
            Vector3 difference = Camera.main.ScreenToViewportPoint(dragOrigin - Input.mousePosition);

            // Pouze pohyb ve směru X a Z (horizontální pohyb)
            Vector3 moveDrag = new Vector3(difference.x * cameraDragSpeed, 0, difference.y * cameraDragSpeed);

            // Posun kamery bez změny osy Y (výška kamery zůstane stejná)
            Camera.main.transform.position += new Vector3(moveDrag.x, 0, moveDrag.z);

            dragOrigin = Input.mousePosition; // Aktualizace drag origin, aby pohyb byl plynulý
        }

        // Omezit pozici kamery v hranicích
        Vector3 clampedPosition = Camera.main.transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, xBounds.x, xBounds.y);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, yBounds.x, yBounds.y);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, zBounds.x, zBounds.y);

        Camera.main.transform.position = clampedPosition;
    }
}
