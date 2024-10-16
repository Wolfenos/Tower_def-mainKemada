using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float cameraDragSpeed = 5f; // Rychlost posunu kamery
    public float cameraRotateSpeed = 100f; // Rychlost rotace kamery
    public float zoomSpeed = 5f; // Rychlost pøiblížení a oddálení
    public Transform rotationPoint; // Bod rotace kamery (mùžeš ho pøiøadit v Inspectoru)

    private Vector3 dragOrigin; // Pozice myši pøi zaèátku tažení

    void Update()
    {
        // Kontrola kliknutí levým tlaèítkem myši
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

        // Rotace kamery pomocí prostøedního tlaèítka myši
        if (Input.GetMouseButton(2)) // 2 je prostøední tlaèítko myši
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

        // Urèení smìru pohybu na základì orientace kamery
        Vector3 forward = Camera.main.transform.forward; // Vektor smìøující vpøed
        Vector3 right = Camera.main.transform.right; // Vektor smìøující vpravo

        forward.y = 0; // Zabráníme, aby pohyb byl vertikální
        right.y = 0; // Zabráníme, aby pohyb byl vertikální
        forward.Normalize(); // Normalizace vektoru vpøed
        right.Normalize(); // Normalizace vektoru vpravo

        // Vypoètení celkového pohybu na základì W, A, S, D
        Vector3 move = (forward * moveVertical + right * moveHorizontal) * cameraDragSpeed * Time.deltaTime;

        // Posun kamery
        Camera.main.transform.position += move;

        // Zoomování kamery pomocí scrollování myši
        float scrollInput = Input.GetAxis("Mouse ScrollWheel"); // Získání hodnoty scrollu
        Camera.main.transform.position += Camera.main.transform.forward * scrollInput * zoomSpeed; // Pøiblížení a oddálení

        // Kamera se zaène hýbat, když podrží pravé tlaèítko myši
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        // Kamera se posouvá, pokud držíme pravé tlaèítko myši
        if (Input.GetMouseButton(1))
        {
            Vector3 difference = Camera.main.ScreenToViewportPoint(dragOrigin - Input.mousePosition);

            // Pouze pohyb ve smìru X a Z (horizontální pohyb)
            Vector3 moveDrag = new Vector3(difference.x * cameraDragSpeed, 0, difference.y * cameraDragSpeed);

            // Posun kamery bez zmìny osy Y (výška kamery zùstane stejná)
            Camera.main.transform.position += new Vector3(moveDrag.x, 0, moveDrag.z);

            dragOrigin = Input.mousePosition; // Aktualizace drag origin, aby pohyb byl plynulý
        }
    }
}