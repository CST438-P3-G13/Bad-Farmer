using UnityEngine;
using TMPro;

public class FontManager : MonoBehaviour
{
    public static FontManager Instance;

    public TMP_FontAsset regularFont;
    public TMP_FontAsset pixelatedFont;

    private TMP_FontAsset currentFont;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this manager across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentFont = regularFont;
    }

    public TMP_FontAsset GetCurrentFont()
    {
        return currentFont;
    }

    public void SetFontRegular()
    {
        currentFont = regularFont;
        UpdateAllTMPTexts();
    }

    public void SetFontPixelated()
    {
        currentFont = pixelatedFont;
        UpdateAllTMPTexts();
    }

    private void UpdateAllTMPTexts()
    {
        // Find all TMP Text objects in the scene
        TextMeshProUGUI[] textObjects = FindObjectsOfType<TextMeshProUGUI>(true); // Includes inactive objects
        foreach (var text in textObjects)
        {
            text.font = currentFont;
            text.UpdateFontAsset(); // Ensure changes take effect
        }
    }
}
