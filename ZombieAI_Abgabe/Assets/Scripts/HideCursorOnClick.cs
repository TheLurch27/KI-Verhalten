using UnityEngine;

public class HideCursorOnClick : MonoBehaviour
{
    private bool isCursorLocked = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Wenn die linke Maustaste gedrückt wird
        {
            Cursor.lockState = CursorLockMode.Locked; // Sperrt den Cursor in die Mitte des Bildschirms
            Cursor.visible = false; // Versteckt den Cursor
            isCursorLocked = true;
        }

        // Optional: Cursor entsperren und sichtbar machen
        if (Input.GetKeyDown(KeyCode.Escape)) // Wenn die Esc-Taste gedrückt wird
        {
            Cursor.lockState = CursorLockMode.None; // Hebt die Sperrung des Cursors auf
            Cursor.visible = true; // Zeigt den Cursor wieder an
            isCursorLocked = false;
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
