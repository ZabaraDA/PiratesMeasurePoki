using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchSceneScript : MonoBehaviour
{
    public void SceneLoad(int numberScene)
    {
        SceneManager.LoadScene(numberScene);
    }
}
