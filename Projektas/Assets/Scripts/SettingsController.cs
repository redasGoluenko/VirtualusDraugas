using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsController : MonoBehaviour
{
    private bool isActive = false; // Track UI state

    private void Start()
    {
        // Disable all child objects (UI elements)
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(isActive);
        }
    }
    void Update()
    {
        // Check if the "+" key is pressed
        if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            ToggleUI();
        }
    }

    void ToggleUI()
    {
        isActive = !isActive; // Toggle the state

        // Enable or disable all child objects (UI elements)
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(isActive);
        }
    }
}
