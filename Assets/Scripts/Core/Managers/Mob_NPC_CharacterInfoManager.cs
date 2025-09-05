using Assets.Models;
using Assets.ViewModels;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

namespace Assets.Scripts.Manager
{
    public class Mob_NPC_CharacterInfoManager : MonoBehaviourPun, IPunObservable
    {
        #region Fields

        private Animator animator;
        private GameObject player;
        private readonly Dictionary<int, float> damageDealt = new();
        private const float ResRegenRate = 0.05f;
        private const float MaxResValue = 1f;

        [Header("Enemy Data")]
        public float exp;

        [Header("Common Data")]
        public float currentLife;
        public float maxLife;
        public bool isMobOrNpc;
        public string publicName;
        public int id;

        [HideInInspector] public Slider sliderRes;
        [HideInInspector] public Slider sliderMana;
        [HideInInspector] public Slider sliderHp;
        [HideInInspector] public Enemy enemy;

        private static GameObject currentMobNpcSlider;

        #endregion

        #region Unity Methods

        private async void Start()
        {
            if (isMobOrNpc) {
                enemy = await EnemyData.GetEnemyById(id);
                return; 
            }

            animator = GetComponent<Animator>();
            player = FindLocalPlayer();
            InitializeSliders();
        }

        private void Update()
        {
            if (sliderHp != null) sliderHp.value = currentLife;

            if (player == null) player = FindLocalPlayer();

            RegenerateRes();
        }

        [PunRPC]
        private void RPC_SetPublicName(string name)
        {
            publicName = name;
        }

        #endregion

        #region Slider Setup

        void HideSliders()
        {
            GameObject.Find("HandleScene/Canvas/panel_selected_mob_npc").SetActive(false);
        }

        private void InitializeSliders()
        {
            if (!PhotonNetwork.IsConnectedAndReady || !photonView.IsMine) return;

            var sliderPrefab = Resources.Load<GameObject>("UI/SlidersStatus");
            var characterPanel = GameObject.Find("HandleScene/Canvas/panel_character");

            if (sliderPrefab == null || characterPanel == null) return;

            var sliderInstance = Instantiate(sliderPrefab, characterPanel.transform);
            sliderInstance.name = $"SlidersStatus_{PhotonNetwork.LocalPlayer.ActorNumber}";
            sliderHp = sliderInstance.transform.Find("slider_hp")?.GetComponent<Slider>();
            sliderRes = sliderInstance.transform.Find("slider_res")?.GetComponent<Slider>();
            sliderMana = sliderInstance.transform.Find("slider_mana")?.GetComponent<Slider>();

            if (sliderHp != null)
            {
                sliderHp.maxValue = maxLife;
                sliderHp.value = currentLife;
            }

            // Definir o nome do personagem antes de enviar
            publicName = PersonagemUtils.LoggedChar.nome;

            if (!string.IsNullOrEmpty(publicName))
            {
                photonView.RPC("RPC_SetPublicName", RpcTarget.OthersBuffered, publicName);
            }
        }

        public void ToggleMobNpcSliders()
        {
            // Não mostrar se não for mob/NPC
            if (!isMobOrNpc && publicName == PersonagemUtils.LoggedChar.nome) return;

            if (!PhotonNetwork.IsConnectedAndReady) return;

            var selectedPanel = GameObject.Find("HandleScene/Canvas/panel_selected_mob_npc");
            if (selectedPanel == null) return;

            selectedPanel.SetActive(true);

            if (currentMobNpcSlider != null)
                Destroy(currentMobNpcSlider);

            var sliderPrefab = Resources.Load<GameObject>("UI/SlidersStatus");

            if (sliderPrefab.transform.Find("name").TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI name))
            {
                name.text = publicName;
            }

            currentMobNpcSlider = Instantiate(sliderPrefab, selectedPanel.transform);
            currentMobNpcSlider.name = $"SlidersStatus_{publicName}";

            sliderHp = currentMobNpcSlider.transform.Find("slider_hp")?.GetComponent<Slider>();
            sliderRes = currentMobNpcSlider.transform.Find("slider_res")?.GetComponent<Slider>();
            sliderMana = currentMobNpcSlider.transform.Find("slider_mana")?.GetComponent<Slider>();

            if (sliderHp != null)
            {
                sliderHp.maxValue = maxLife;
                sliderHp.value = currentLife;
            }
        }

        #endregion

        #region Combat / Damage

        public void TakeDamage(float damage, string tag)
        {
            if (!PhotonNetwork.IsConnectedAndReady) return;

            if (tag == "Enemy")
                photonView.RPC("RPC_TakeDamageEnemy", RpcTarget.All, damage, PhotonNetwork.LocalPlayer.ActorNumber);
            else if (tag == "Player")
                photonView.RPC("RPC_TakeDamagePlayer", RpcTarget.All, damage);
        }

        [PunRPC]
        private async Task RPC_TakeDamageEnemy(float damage, int playerId)
        {
            ApplyDamageInfo(damage, playerId);

            if (currentLife <= 0)
            {
                await DistributeExp();
                SpawnChest();
                DestroyEnemy();
            }
        }

        private void SpawnChest()
        {
            GameObject chestPrefab = Resources.Load<GameObject>("GameObjects/ChestModel");

            if (chestPrefab is not null)
            {
                // Instancia o baú e guarda referência à instância
                GameObject chestInstance = Instantiate(chestPrefab, transform.position, Quaternion.identity);

                // Agora sim, pega o componente da instância
                if (chestInstance.TryGetComponent<Chest>(out Chest chest))
                {
                    chest.dropItems = enemy?.Drops?.Select(_ => _.DropItem).ToList();
                }
            }
        }


        private void ApplyDamageInfo(float damage, int playerId)
        {
            currentLife -= damage;

            if (playerId == 0) return;

            if (!damageDealt.ContainsKey(playerId))
                damageDealt[playerId] = 0;

            damageDealt[playerId] += damage;
        }

        [PunRPC]
        private void RPC_TakeDamagePlayer(float damage)
        {
            currentLife -= damage;

            if (sliderHp != null)
            {
                sliderHp.value = Mathf.Clamp(sliderHp.value - damage, 0, sliderHp.maxValue);
                if (sliderHp.value <= 0)
                {
                    HideSliders();
                    Die();
                }
            }
        }

        private void Die()
        {
            if (!photonView.IsMine) return;

            animator.SetTrigger("dying");
            animator.ResetTrigger("resurrecting");

            if (TryGetComponent<CharacterController>(out var controller))
                controller.height = 0.5f;

            if (TryGetComponent<Combat>(out var combat))
                combat.enabled = false;

            StartCoroutine(DisableMovementAfterDelay(0.4f));
            enabled = false;
        }

        private IEnumerator DisableMovementAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (TryGetComponent<Movement>(out var movement))
                movement.enabled = false;
        }

        #endregion

        #region Enemy Lifecycle

        private void DestroyEnemy()
        {
            if (currentMobNpcSlider != null && currentMobNpcSlider.name == $"SlidersStatus_{gameObject.name}")
            {
                Destroy(currentMobNpcSlider);
                currentMobNpcSlider = null;
                HideSliders();
            }

            if (photonView.IsMine || PhotonNetwork.IsMasterClient)
            {
                if (PhotonNetwork.IsConnectedAndReady)
                    PhotonNetwork.Destroy(gameObject);
                else
                    Destroy(gameObject);
            }
        }

        private async Task DistributeExp()
        {
            if (damageDealt.Count == 0) return;

            float totalDamage = damageDealt.Values.Sum();

            foreach (var (playerId, playerDamage) in damageDealt)
            {
                float expToGive = (playerDamage / totalDamage) * exp;
                var playerObj = PhotonNetwork.CurrentRoom.Players[playerId].TagObject as GameObject;

                if (playerObj != null)
                {
                    PersonagemUtils.IncreaseExp(expToGive);

                    if (PhotonNetwork.LocalPlayer.ActorNumber == playerId)
                        await AccountCharacters.EditCharacter(PersonagemUtils.LoggedChar);
                }
            }
        }

        #endregion

        #region Utility

        public void RegenerateRes()
        {
            if (sliderRes == null || sliderRes.value >= MaxResValue) return;

            sliderRes.value = Mathf.Min(sliderRes.value + ResRegenRate * Time.deltaTime, MaxResValue);
        }

        public GameObject FindLocalPlayer() => PhotonNetwork.LocalPlayer.TagObject as GameObject;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(currentLife);
            }
            else
            {
                currentLife = (float)stream.ReceiveNext();

                if (sliderHp != null)
                {
                    sliderHp.value = currentLife;
                }
            }
        }

        private void ShowDeathPanel()
        {
            if (!photonView.IsMine) return;

            FindObjectOfType<GameUIManager>()?.ShowDeathPanel();
        }

        #endregion
    }
}
