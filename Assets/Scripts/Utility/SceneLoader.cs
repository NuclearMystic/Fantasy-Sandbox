using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject loadingScreen;
    public Slider progressBar;

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            progressBar.value = operation.progress;
            yield return null;
        }

        progressBar.value = 1f;

        yield return new WaitForSeconds(0.5f);

        operation.allowSceneActivation = true;
    }
}
