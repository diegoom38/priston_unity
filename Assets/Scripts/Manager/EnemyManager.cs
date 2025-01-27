using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Manager
{
    public class EnemyManager : MonoBehaviour
    {
        private GameObject player;

        private void Start()
        {
            StartCoroutine(LateStart(1));
        }

        IEnumerator LateStart(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            player = GameObject.FindGameObjectWithTag("Player");

            if(player != null)
                SetEnemiesLifeBar();
        }

        private void SetEnemiesLifeBar()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject enemyUIPrefab = Resources.Load<GameObject>("EnemyUI");

            if (enemyUIPrefab == null)
            {
                Debug.LogError("EnemyUI prefab not found in Resources!");
                return;
            }

            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;

                GameObject enemyUI = Instantiate(enemyUIPrefab);
                enemyUI.transform.position = new Vector3(0, .75f, 0);
                enemyUI.transform.SetParent(enemy.transform, false);
                enemyUI.SetActive(false);

                StartCoroutine(CheckMouseOverEnemy(enemy, enemyUI));
            }
        }

        private IEnumerator CheckMouseOverEnemy(GameObject enemy, GameObject enemyUI)
        {
            while (enemy != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    bool isMouseOver = hit.collider.gameObject == enemy;

                    // Atualiza apenas se o estado do UI mudou
                    if (enemyUI.activeSelf != isMouseOver)
                    {
                        enemyUI.SetActive(isMouseOver);

                        if (isMouseOver)
                        {
                            // Garante que o UI sempre aponte para o jogador
                            Vector3 direction = player.transform.position - enemyUI.transform.position;
                            direction.y = 0; // Mantém a rotação no plano horizontal
                            enemyUI.transform.rotation = Quaternion.LookRotation(direction);
                        }
                    }
                }
                else if (enemyUI.activeSelf)
                {
                    // Oculta o UI caso o Raycast não detecte o inimigo
                    enemyUI.SetActive(false);
                }

                yield return null; // Espera um frame antes de repetir o loop
            }
        }
    }
}