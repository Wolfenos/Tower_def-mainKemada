using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float cameraDragSpeed = 5f; // Rychlost posunu kamery
    public float cameraRotateSpeed = 100f; // Rychlost rotace kamery
    public float zoomSpeed = 5f; // Rychlost p�ibl�en� a odd�len�
    public Transform rotationPoint; // Bod rotace kamery (m��e� ho p�i�adit v Inspectoru)

    private Vector3 dragOrigin; // Pozice my�i p�i za��tku ta�en�

    void Update()
    {
        // Kontrola kliknut� lev�m tla��tkem my�i
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Detekce kliknut� na objekt
                Debug.Log("Kliknuto na objekt: " + hit.transform.name);
            }
            else
            {
                Debug.Log("Raycast nedetekoval ��dn� objekt.");
            }
        }

        // Rotace kamery pomoc� prost�edn�ho tla��tka my�i
        if (Input.GetMouseButton(2)) // 2 je prost�edn� tla��tko my�i
        {
            float rotateX = Input.GetAxis("Mouse X") * cameraRotateSpeed * Time.deltaTime;
            float rotateY = Input.GetAxis("Mouse Y") * cameraRotateSpeed * Time.deltaTime;

            // Rotuj kolem bodu
            Camera.main.transform.RotateAround(rotationPoint.position, Vector3.up, rotateX); // Rotace kolem Y osy
            Camera.main.transform.RotateAround(rotationPoint.position, Camera.main.transform.right, -rotateY); // Rotace kolem X osy
        }

        // Pohyb kamery pomoc� W, A, S, D
        float moveHorizontal = Input.GetAxis("Horizontal"); // A, D
        float moveVertical = Input.GetAxis("Vertical"); // W, S

        // Ur�en� sm�ru pohybu na z�klad� orientace kamery
        Vector3 forward = Camera.main.transform.forward; // Vektor sm��uj�c� vp�ed
        Vector3 right = Camera.main.transform.right; // Vektor sm��uj�c� vpravo

        forward.y = 0; // Zabr�n�me, aby pohyb byl vertik�ln�
        right.y = 0; // Zabr�n�me, aby pohyb byl vertik�ln�
        forward.Normalize(); // Normalizace vektoru vp�ed
        right.Normalize(); // Normalizace vektoru vpravo

        // Vypo�ten� celkov�ho pohybu na z�klad� W, A, S, D
        Vector3 move = (forward * moveVertical + right * moveHorizontal) * cameraDragSpeed * Time.deltaTime;

        // Posun kamery
        Camera.main.transform.position += move;

        // Zoomov�n� kamery pomoc� scrollov�n� my�i
        float scrollInput = Input.GetAxis("Mouse ScrollWheel"); // Z�sk�n� hodnoty scrollu
        Camera.main.transform.position += Camera.main.transform.forward * scrollInput * zoomSpeed; // P�ibl�en� a odd�len�

        // Kamera se za�ne h�bat, kdy� podr�� prav� tla��tko my�i
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        // Kamera se posouv�, pokud dr��me prav� tla��tko my�i
        if (Input.GetMouseButton(1))
        {
            Vector3 difference = Camera.main.ScreenToViewportPoint(dragOrigin - Input.mousePosition);

            // Pouze pohyb ve sm�ru X a Z (horizont�ln� pohyb)
            Vector3 moveDrag = new Vector3(difference.x * cameraDragSpeed, 0, difference.y * cameraDragSpeed);

            // Posun kamery bez zm�ny osy Y (v��ka kamery z�stane stejn�)
            Camera.main.transform.position += new Vector3(moveDrag.x, 0, moveDrag.z);

            dragOrigin = Input.mousePosition; // Aktualizace drag origin, aby pohyb byl plynul�
        }
    }
}