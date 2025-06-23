using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public void SceneLoad(int numberScene)
    {
        SceneManager.LoadScene(numberScene);
    }
}
