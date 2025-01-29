using UnityEngine;

namespace Assets.Scripts
{
    public class EnemyController : MonoBehaviour
    {
        private GameObject player;
        private GameObject enemyUI;

        private void Start()
        {
            Invoke(nameof(Initialize), 1f);
        }

        private void Initialize()
        {
            player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
                SetupLifeBar();
        }

        private void SetupLifeBar()
        {
            GameObject enemyUIPrefab = Resources.Load<GameObject>("EnemyUI");

            if (enemyUIPrefab == null) return;

            enemyUI = Instantiate(enemyUIPrefab, transform);
            enemyUI.transform.localPosition = new Vector3(0, 0.75f, 0);
            enemyUI.SetActive(false);
        }

        private void OnMouseOver()
        {
            if (enemyUI == null || player == null) return;

            enemyUI.SetActive(true);

            Vector3 direction = player.transform.position - enemyUI.transform.position;
            direction.y = 0;
            enemyUI.transform.rotation = Quaternion.LookRotation(direction);
        }

        private void OnMouseExit()
        {
            if (enemyUI != null)
                enemyUI.SetActive(false);
        }
    }
}