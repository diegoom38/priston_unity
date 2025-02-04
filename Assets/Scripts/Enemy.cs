using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Enemy : MonoBehaviour
    {
        private GameObject player;
        private GameObject enemyUI;

        public float life;
        public float exp;

        private void Start()
        {
            Invoke(nameof(Initialize), 1f);
        }

        private void Update()
        {
            var slider = GetSlider();

            if (slider)            
                slider.value = life;

            if (life <= 0)
            {
                Destroy(gameObject);
                return;
            }
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

            var slider = GetSlider();

            if (slider)
            {
                slider.maxValue = life;
                slider.value = life;
            }
        }

        private Slider GetSlider() => enemyUI?.transform.Find("Slider").GetComponent<Slider>();

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