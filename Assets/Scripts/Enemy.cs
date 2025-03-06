using Assets.Models;
using Assets.Scripts.Manager;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace Assets.Scripts
{
    public class Enemy : MonoBehaviourPun
    {
        private GameObject player;
        private GameObject enemyUI;

        public float life;
        public float exp;

        private void Start()
        {
            Invoke(nameof(Initialize), 1f);
        }

        private async void Update()
        {
            // Atualiza a barra de vida do inimigo
            Slider slider = GetSlider();

            if (slider)
                slider.value = life;

            // Verifica se a vida do inimigo chegou a zero
            if (life <= 0)
            {
                DestroyEnemy();
                await IncreaseExp();
                return;
            }
        }

        private async Task IncreaseExp()
        {
            // Aumenta a experiência do jogador local
            if (PhotonNetwork.IsConnectedAndReady && photonView.IsMine)
            {
                PersonagemUtils.IncreaseExp(exp);
                await AccountCharacters.EditCharacter(PersonagemUtils.LoggedChar);
            }
        }

        private void Initialize()
        {
            // Busca o jogador local
            player = FindLocalPlayer();

            if (player != null)
                SetupLifeBar();
        }

        private GameObject FindLocalPlayer()
        {
            // Busca todos os objetos com a tag "Player"
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject p in players)
            {
                PhotonView photonView = p.GetComponent<PhotonView>();
                if (photonView != null && photonView.IsMine)
                {
                    return p; // Retorna o jogador local
                }
            }

            return null; // Retorna null se o jogador local não for encontrado
        }

        private void SetupLifeBar()
        {
            // Carrega o prefab da UI do inimigo
            GameObject enemyUIPrefab = Resources.Load<GameObject>("EnemyUI");

            if (enemyUIPrefab == null) return;

            // Instancia a UI do inimigo
            enemyUI = Instantiate(enemyUIPrefab, transform);
            enemyUI.transform.localPosition = new Vector3(0, 0.75f, 0);
            enemyUI.SetActive(false);

            // Configura a barra de vida
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
            // Ativa a UI do inimigo quando o mouse está sobre ele
            if (enemyUI == null || player == null) return;

            enemyUI.SetActive(true);

            // Rotaciona a UI para ficar de frente para o jogador
            Vector3 direction = player.transform.position - enemyUI.transform.position;
            direction.y = 0;
            enemyUI.transform.rotation = Quaternion.LookRotation(direction);
        }

        private void OnMouseExit()
        {
            // Desativa a UI do inimigo quando o mouse sai
            if (enemyUI != null)
                enemyUI.SetActive(false);
        }

        private void DestroyEnemy()
        {
            // Destroi o inimigo na rede
            if (PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}