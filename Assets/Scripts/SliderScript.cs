using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    public Slider m_Slider;
    public TextMeshProUGUI InfoTMP;

    public float myVolume;

    private void Start() {
        if (m_Slider.value != PlayerPrefs.GetFloat("MasterVolume")) {
            m_Slider.value = PlayerPrefs.GetFloat("MasterVolume");
        }
    }

    private void Update() {
        CheckSettings();
    }

    private void OnValChange(float myValue) {
        myVolume = myValue;
        PlayerPrefs.SetFloat("MasterVolume", myVolume);
        myVolume = Mathf.Round(myVolume * 100f) / 100f;

        InfoTMP.text = "Volume: " + myVolume + " /1";
    }

    public void CheckSettings() {
        if (AudioListener.volume != PlayerPrefs.GetFloat("MasterVolume")) {
            AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume");
        }
    }
}
