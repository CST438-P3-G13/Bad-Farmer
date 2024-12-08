using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FontSizeController : MonoBehaviour
{
    public Dropdown fontSizeDropdown; // Assign the dropdown in the inspector
    private Dictionary<Text, int> defaultFontSizes = new Dictionary<Text, int>();

    private void Start()
    {
        // Cache default font sizes using the new FindObjectsByType method
        var texts = FindObjectsByType<Text>(FindObjectsSortMode.None); 
        foreach (var text in texts)
        {
            if (!defaultFontSizes.ContainsKey(text))
            {
                defaultFontSizes[text] = text.fontSize; // Store default size
            }
        }

        // Add a listener to the dropdown
        fontSizeDropdown.onValueChanged.AddListener(OnFontSizeChanged);
    }

    private void OnFontSizeChanged(int option)
    {
        // Determine scale factor based on dropdown value
        float scaleFactor = (option == 0) ? 1.0f : 1.5f; // Regular or Large

        // Update font sizes dynamically
        foreach (var entry in defaultFontSizes)
        {
            entry.Key.fontSize = Mathf.RoundToInt(entry.Value * scaleFactor);
        }
    }
}
