using UnityEngine;

public class FontToggleButton : MonoBehaviour
{
    public void SetRegularFont()
    {
        FontManager.Instance.SetFontRegular();
    }

    public void SetPixelatedFont()
    {
        FontManager.Instance.SetFontPixelated();
    }
}
