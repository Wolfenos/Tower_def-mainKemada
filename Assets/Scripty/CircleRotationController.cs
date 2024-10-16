using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleRotationController : MonoBehaviour
{
    // Nastavení v inspectoru
    public KeyCode rotateLeftKey = KeyCode.A;  // Klávesa pro otáèení doleva (proti smìru hodin)
    public KeyCode rotateRightKey = KeyCode.D; // Klávesa pro otáèení doprava (po smìru hodin)
    public LayerMask rotationLayer;            // Vrstva, na které se otáèení bude aplikovat
    public float rotationAngle = 60f;          // Úhel rotace v stupních

    private bool isRotating = false;           // Kontrola, zda je objekt v procesu rotace
    private float targetRotationY;             // Cílová rotace kolem osy Y
    private Transform selectedObject;          // Aktuálnì vybraný objekt pro rotaci

    void Update()
    {
        // Kontrola, zda hráè klikne na objekt k otáèení
        if (Input.GetMouseButtonDown(0)) // Kliknutí myší
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Zkontrolujeme, zda objekt patøí na specifikovaný layer
                if (((1 << hit.transform.gameObject.layer) & rotationLayer) != 0)
                {
                    selectedObject = hit.transform; // Nastavíme vybraný objekt
                    targetRotationY = selectedObject.eulerAngles.y; // Aktuální rotace objektu
                }
            }
        }

        // Kontrola, zda byl vybrán objekt a není v procesu rotace
        if (selectedObject != null && !isRotating)
        {
            if (Input.GetKeyDown(rotateLeftKey))
            {
                RotateObject(-rotationAngle); // Otoèení proti smìru hodin
            }
            else if (Input.GetKeyDown(rotateRightKey))
            {
                RotateObject(rotationAngle); // Otoèení po smìru hodin
            }
        }
    }

    void RotateObject(float angle)
    {
        // Nastavení cílové rotace
        targetRotationY += angle;
        isRotating = true;
        StartCoroutine(RotateSmoothly());
    }

    IEnumerator RotateSmoothly()
    {
        float startRotationY = selectedObject.eulerAngles.y;
        float timeElapsed = 0f;
        float duration = 0.5f; // Délka rotace (pùl sekundy)

        while (timeElapsed < duration)
        {
            float currentRotationY = Mathf.LerpAngle(startRotationY, targetRotationY, timeElapsed / duration);
            selectedObject.eulerAngles = new Vector3(selectedObject.eulerAngles.x, currentRotationY, selectedObject.eulerAngles.z);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Zajištìní pøesné rotace po skonèení interpolace
        selectedObject.eulerAngles = new Vector3(selectedObject.eulerAngles.x, targetRotationY, selectedObject.eulerAngles.z);
        isRotating = false;
    }
}