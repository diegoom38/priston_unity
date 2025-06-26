using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoaderManager : MonoBehaviour
{
    private GameObject loadingManagerObj;
    private Slider loadingSlider;
    private TextMeshProUGUI loadingText;

    public void LoadSceneWithLoadingScreen(string sceneName)
    {
        if (loadingManagerObj == null)
        {
            loadingManagerObj = Instantiate(transform.gameObject);
            loadingSlider = loadingManagerObj.GetComponentInChildren<Slider>();
            loadingText = loadingManagerObj.GetComponentInChildren<TextMeshProUGUI>();

            // Garante que o Canvas do loading esteja no topo
            Canvas canvas = loadingManagerObj.GetComponentInChildren<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 999; // Um valor alto para garantir que fique no topo
            }

            // Move para o topo da hierarquia
            loadingManagerObj.transform.SetAsLastSibling();
        }

        loadingManagerObj.SetActive(true);
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (loadingSlider != null) loadingSlider.value = progress;
            if (loadingText != null) loadingText.text = $"{progress * 100:F0}%";

            if (operation.progress >= 0.9f)
            {
                if (loadingSlider != null) loadingSlider.value = 1f;
                if (loadingText != null) loadingText.text = "100%";
                operation.allowSceneActivation = true;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
