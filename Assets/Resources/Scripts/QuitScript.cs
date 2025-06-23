using UnityEditor;
using UnityEngine;

public class QuitScript : MonoBehaviour
{
    public void OnQuitButtonClicked()
    {
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
    }
}
