using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
        // Quits the application
        Debug.Log("Quit Game");  // Optional: Log the quit action for debugging
        Application.Quit();

        // If you're in the Unity Editor (this won't run in a build), stop the play mode
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
