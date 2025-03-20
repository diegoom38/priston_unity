using Assets.Models;
using Photon.Pun;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace Assets.Scripts.Manager
{
    // Classe responsável por gerenciar as informações e comportamentos do personagem no jogo
    public class CharacterInfoManager : MonoBehaviourPun, IPunObservable
    {
        private Animator animator; // Referência ao Animator para controlar animações
        private GameObject enemyUI; // UI que representa as informações do inimigo
        private GameObject player; // Referência ao jogador local
        private Dictionary<int, float> damageDealt = new Dictionary<int, float>(); // Dicionário para armazenar o dano causado por cada jogador
        private float resRegenRate = 0.05f; // Taxa de regeneração de recursos por segundo
        private float maxResValue = 1.0f;  // Valor máximo do slider de recursos

        // Dados específicos do inimigo
        [Header("Enemy Data")]
        public float exp; // Experiência que o inimigo dá ao ser derrotado

        // Dados comuns entre jogadores e inimigos
        [Header("Commons Data")]
        public float currentLife; // Vida atual do personagem
        public float maxLife; // Vida máxima do personagem

        [HideInInspector] public Slider sliderRes; // Slider para os recursos do personagem
        [HideInInspector] public Slider sliderMana; // Slider para a mana do personagem
        [HideInInspector] public Slider sliderHp; // Slider para a vida do personagem

        // Método chamado ao iniciar o script
        private void Start()
        {
            animator = GetComponent<Animator>(); // Obtém o componente Animator do objeto
            player = FindLocalPlayer(); // Encontra o jogador local
            InitializeSliders(); // Inicializa os sliders de status
        }

        // Método chamado a cada frame
        private void Update()
        {
            Slider slider = GetSlider(); // Obtém o slider de vida
            if (slider) slider.value = currentLife; // Atualiza o valor do slider de vida
            if (player == null) player = FindLocalPlayer(); // Caso o jogador não seja encontrado, tenta novamente
            RegenerateRes(); // Regenera recursos a cada frame
        }

        // Método para inicializar os sliders de status
        void InitializeSliders()
        {
            // Verifica se a rede está conectada e se o PhotonView é do jogador local
            if (!PhotonNetwork.IsConnectedAndReady || !photonView.IsMine) return;

            // Carrega o prefab do slider de status
            GameObject sliderStatusPrefab = Resources.Load<GameObject>("SlidersStatus");
            GameObject panelCharacter = GameObject.Find("HandleScene/Canvas/panel_character"); // Encontra o painel onde o slider será adicionado

            // Instancia o slider de status no painel
            GameObject sliderStatus = Instantiate(sliderStatusPrefab, panelCharacter.transform);

            // Define um nome único para o SliderStatus do jogador
            sliderStatus.name = $"SlidersStatus_{PhotonNetwork.LocalPlayer.ActorNumber}";

            // Inicializa os sliders específicos
            sliderHp = sliderStatus.transform.Find("slider_hp").GetComponent<Slider>();
            sliderRes = sliderStatus.transform.Find("slider_res").GetComponent<Slider>();
            sliderMana = sliderStatus.transform.Find("slider_mana").GetComponent<Slider>();

            // Configura o slider de vida, se encontrado
            if (sliderHp != null)
            {
                sliderHp.maxValue = maxLife;
                sliderHp.value = currentLife;
            }
        }

        // Método para encontrar o jogador local
        public GameObject FindLocalPlayer()
        {
            return PhotonNetwork.LocalPlayer.TagObject as GameObject;
        }

        private void SetupLifeBar()
        {
            GameObject enemyUIPrefab = Resources.Load<GameObject>("EnemyUI");
            if (enemyUIPrefab == null) return;

            enemyUI = Instantiate(enemyUIPrefab, transform);

            // Acessa o RectTransform para ajustar a posição dentro do Canvas
            RectTransform rectTransform = enemyUI.GetComponent<RectTransform>();
            if (rectTransform)
            {
                rectTransform.localPosition = new Vector3(0, 2f, 0);  // Posição inicial do painel de vida
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 50f); // Ajuste da altura desejada (50f)
            }

            enemyUI.SetActive(false);

            var slider = GetSlider();
            if (slider)
            {
                slider.maxValue = maxLife;
                slider.value = currentLife;
            }

            // Aqui você pode aplicar um pequeno deslocamento para garantir que a barra de vida fique um pouco acima
            enemyUI.transform.position = transform.position + new Vector3(0, 2f, 0);  // Ajuste o valor conforme necessário
        }

        // Método para obter o slider de vida
        private UnityEngine.UI.Slider GetSlider() => enemyUI?.transform.Find("Slider").GetComponent<UnityEngine.UI.Slider>();

        // Método chamado quando o mouse está sobre o inimigo
        private void OnMouseOver()
        {
            if (!PhotonNetwork.IsConnectedAndReady) return;

            if (enemyUI == null) SetupLifeBar();

            enemyUI.SetActive(true);

            Vector3 direction = player.transform.position - enemyUI.transform.position;
            direction.y = 0;

            // Ajusta a rotação para olhar na direção do jogador
            enemyUI.transform.rotation = Quaternion.LookRotation(direction);

            // Desloca a posição da UI um pouco mais acima do GameObject alvo
            enemyUI.transform.position = transform.position + new Vector3(0, 2f, 0);  // Ajuste o valor 2.0f conforme necessário
        }

        // Método chamado quando o mouse sai da área do inimigo
        private void OnMouseExit()
        {
            if (enemyUI != null) enemyUI.SetActive(false); // Desativa a UI do inimigo
        }

        // Método assíncrono para distribuir a experiência entre os jogadores
        private async Task DistributeExp()
        {
            if (damageDealt.Count == 0) return;

            float totalDamage = damageDealt.Values.Sum(); // Soma o total de dano causado

            foreach (var entry in damageDealt)
            {
                int playerId = entry.Key;
                float playerDamage = entry.Value;
                float playerExp = (playerDamage / totalDamage) * exp; // Calcula a experiência a ser distribuída para o jogador

                GameObject playerObj = PhotonNetwork.CurrentRoom.Players[playerId].TagObject as GameObject;

                if (playerObj != null)
                {
                    PersonagemUtils.IncreaseExp(playerExp); // Aumenta a experiência do jogador

                    // Se o jogador local foi o que causou o dano, atualiza a informação do personagem
                    if (PhotonNetwork.LocalPlayer.ActorNumber == playerId)
                    {
                        await AccountCharacters.EditCharacter(PersonagemUtils.LoggedChar);
                    }
                }
            }
        }

        // Método para destruir o inimigo
        private void DestroyEnemy()
        {
            if (photonView.IsMine || PhotonNetwork.IsMasterClient)
            {
                if (PhotonNetwork.IsConnectedAndReady)
                    PhotonNetwork.Destroy(gameObject); // Destrói o objeto na rede
                else
                    Destroy(gameObject); // Destrói localmente, se não estiver na rede
            }
        }

        // Método para aplicar dano ao inimigo
        public void TakeDamage(float damage, string tag)
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                if (tag == "Enemy")
                    photonView.RPC("RPC_TakeDamageEnemy", RpcTarget.All, damage, PhotonNetwork.LocalPlayer.ActorNumber);
                else if (tag == "Player")
                    photonView.RPC("RPC_TakeDamagePlayer", RpcTarget.All, damage);
            }
        }

        // Método RPC para aplicar dano ao inimigo
        [PunRPC]
        private async Task RPC_TakeDamageEnemy(float damage, int playerId)
        {
            ApplyDamageInfo(damage, playerId);

            if (currentLife <= 0)
            {
                await DistributeExp(); // Distribui a experiência ao morrer
                DestroyEnemy(); // Destrói o inimigo
            }
        }

        // Método para aplicar informações de dano (vida reduzida)
        private void ApplyDamageInfo(float damage, int playerId)
        {
            currentLife -= damage; // Subtrai o dano da vida atual

            if (playerId != 0)
            {
                if (!damageDealt.ContainsKey(playerId))
                    damageDealt[playerId] = 0;

                damageDealt[playerId] += damage; // Adiciona o dano ao jogador correspondente
            }
        }

        // Método de sincronização de dados de rede (Photon)
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(currentLife); // Envia a vida atual para os outros jogadores
            }
            else
            {
                currentLife = (float)stream.ReceiveNext(); // Recebe a vida atual de outros jogadores
                UnityEngine.UI.Slider slider = GetSlider();
                if (slider) slider.value = currentLife; // Atualiza o slider de vida
            }
        }

        // Método para regenerar recursos (por exemplo, mana ou energia)
        public void RegenerateRes()
        {
            if (sliderRes != null && sliderRes.value < maxResValue)
            {
                sliderRes.value = Mathf.Min(sliderRes.value + resRegenRate * Time.deltaTime, maxResValue); // Regenera recursos ao longo do tempo
            }
        }

        // Método RPC para aplicar dano ao jogador
        [PunRPC]
        private void RPC_TakeDamagePlayer(float damage)
        {
            currentLife -= damage; // Subtrai o dano da vida do jogador

            if (sliderHp != null)
            {
                sliderHp.value = Mathf.Clamp(sliderHp.value - damage, 0, sliderHp.maxValue); // Atualiza o slider de vida

                if (sliderHp.value <= 0)
                {
                    Die(); // Se a vida do jogador chegar a 0, morre
                }
            }
        }

        // Método para matar o jogador
        private void Die()
        {
            if (photonView.IsMine)
            {
                photonView.RPC("RPC_Die", RpcTarget.All); // Envia a mensagem de morte para todos os jogadores
            }
        }

        // Método RPC para executar a animação de morte
        [PunRPC]
        private void RPC_Die()
        {
            if (animator != null)
            {
                animator.SetTrigger("dying"); // Ativa a animação de morte
            }
        }

        // Método para mostrar o painel de morte
        private void ShowDeathPanel()
        {
            if (!photonView.IsMine) return;

            var uiManager = FindObjectOfType<UIManager>(); // Encontra o UIManager na cena
            uiManager.ShowDeathPanel(); // Exibe o painel de morte
        }
    }
}
