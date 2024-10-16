using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleRotationController : MonoBehaviour
{
    // Nastaven� v inspectoru
    public KeyCode rotateLeftKey = KeyCode.A;  // Kl�vesa pro ot��en� doleva (proti sm�ru hodin)
    public KeyCode rotateRightKey = KeyCode.D; // Kl�vesa pro ot��en� doprava (po sm�ru hodin)
    public LayerMask rotationLayer;            // Vrstva, na kter� se ot��en� bude aplikovat
    public float rotationAngle = 60f;          // �hel rotace v stupn�ch

    private bool isRotating = false;           // Kontrola, zda je objekt v procesu rotace
    private float targetRotationY;             // C�lov� rotace kolem osy Y
    private Transform selectedObject;          // Aktu�ln� vybran� objekt pro rotaci

    void Update()
    {
        // Kontrola, zda hr�� klikne na objekt k ot��en�
        if (Input.GetMouseButtonDown(0)) // Kliknut� my��
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Zkontrolujeme, zda objekt pat�� na specifikovan� layer
                if (((1 << hit.transform.gameObject.layer) & rotationLayer) != 0)
                {
                    selectedObject = hit.transform; // Nastav�me vybran� objekt
                    targetRotationY = selectedObject.eulerAngles.y; // Aktu�ln� rotace objektu
                }
            }
        }

        // Kontrola, zda byl vybr�n objekt a nen� v procesu rotace
        if (selectedObject != null && !isRotating)
        {
            if (Input.GetKeyDown(rotateLeftKey))
            {
                RotateObject(-rotationAngle); // Oto�en� proti sm�ru hodin
            }
            else if (Input.GetKeyDown(rotateRightKey))
            {
                RotateObject(rotationAngle); // Oto�en� po sm�ru hodin
            }
        }
    }

    void RotateObject(float angle)
    {
        // Nastaven� c�lov� rotace
        targetRotationY += angle;
        isRotating = true;
        StartCoroutine(RotateSmoothly());
    }

    IEnumerator RotateSmoothly()
    {
        float startRotationY = selectedObject.eulerAngles.y;
        float timeElapsed = 0f;
        float duration = 0.5f; // D�lka rotace (p�l sekundy)

        while (timeElapsed < duration)
        {
            float currentRotationY = Mathf.LerpAngle(startRotationY, targetRotationY, timeElapsed / duration);
            selectedObject.eulerAngles = new Vector3(selectedObject.eulerAngles.x, currentRotationY, selectedObject.eulerAngles.z);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Zaji�t�n� p�esn� rotace po skon�en� interpolace
        selectedObject.eulerAngles = new Vector3(selectedObject.eulerAngles.x, targetRotationY, selectedObject.eulerAngles.z);
        isRotating = false;
    }
}