using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    [Header("Scene Properties")]
    [Tooltip("Place the scene name here you want to go to.")]
    [SerializeField] private string sceneName;

    public void ChangeToScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
