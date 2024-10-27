using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float cameraDragSpeed = 5f; // Rychlost posunu kamery
    public float cameraRotateSpeed = 100f; // Rychlost rotace kamery
    public float zoomSpeed = 5f; // Rychlost přiblížení a oddálení
    public Transform rotationPoint; // Bod rotace kamery (můžeš ho přiřadit v Inspectoru)

    [Header("Camera Movement Boundaries")]
    public float minX = -50f;
    public float maxX = 50f;
    public float minY = 5f;
    public float maxY = 50f;
    public float minZ = -50f;
    public float maxZ = 50f;

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
                Debug.Log("Kliknuto na objekt: " + hit.transform.name);
            }
            else
            {
                Debug.Log("Raycast nedetekoval žádný objekt.");
            }
        }

        // Rotace kamery pomocí prostředního tlačítka myši
        if (Input.GetMouseButton(2))
        {
            float rotateX = Input.GetAxis("Mouse X") * cameraRotateSpeed * Time.deltaTime;
            float rotateY = Input.GetAxis("Mouse Y") * cameraRotateSpeed * Time.deltaTime;

            Camera.main.transform.RotateAround(rotationPoint.position, Vector3.up, rotateX); // Rotace kolem Y osy
            Camera.main.transform.RotateAround(rotationPoint.position, Camera.main.transform.right, -rotateY); // Rotace kolem X osy
        }

        // Pohyb kamery pomocí W, A, S, D
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 move = (forward * moveVertical + right * moveHorizontal) * cameraDragSpeed * Time.deltaTime;
        Vector3 newPosition = Camera.main.transform.position + move;

        // Apply boundary limits (including Y axis)
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

        Camera.main.transform.position = newPosition;

        // Zoomování kamery pomocí scrollování myši
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        Vector3 zoomPosition = Camera.main.transform.position + Camera.main.transform.forward * scrollInput * zoomSpeed;

        // Apply boundary limits for zoom as well
        zoomPosition.y = Mathf.Clamp(zoomPosition.y, minY, maxY);
        Camera.main.transform.position = zoomPosition;

        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 difference = Camera.main.ScreenToViewportPoint(dragOrigin - Input.mousePosition);
            Vector3 moveDrag = new Vector3(difference.x * cameraDragSpeed, 0, difference.y * cameraDragSpeed);
            Vector3 newDragPosition = Camera.main.transform.position + new Vector3(moveDrag.x, 0, moveDrag.z);

            // Apply boundary limits for drag movement
            newDragPosition.x = Mathf.Clamp(newDragPosition.x, minX, maxX);
            newDragPosition.y = Mathf.Clamp(newDragPosition.y, minY, maxY);
            newDragPosition.z = Mathf.Clamp(newDragPosition.z, minZ, maxZ);

            Camera.main.transform.position = newDragPosition;

            dragOrigin = Input.mousePosition;
        }
    }
}
