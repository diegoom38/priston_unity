using UnityEngine;

namespace Assets.Scripts.Manager
{
    public static class LoadingManager
    {
        public static SceneLoaderManager GetSceneLoader()
        {
            SceneLoaderManager loadingManager = Object.FindObjectOfType<SceneLoaderManager>();

            if (loadingManager == null)
            {
                GameObject loadingManagerObj = Object.Instantiate(Resources.Load<GameObject>("SceneLoader"));
                loadingManagerObj.SetActive(true);
                loadingManager = loadingManagerObj.GetComponent<SceneLoaderManager>();
            }
            else if (!loadingManager.gameObject.activeSelf)
            {
                loadingManager.gameObject.SetActive(true);
            }

            return loadingManager;
        }
    }
}
