using Assets.Enums;
using Assets.Scripts.Manager;
using Photon.Pun;
using UnityEngine;

public class Combat : MonoBehaviourPun
{
    private Animator animator;
    private Mob_NPC_CharacterInfoManager characterInfoManager;
    private GameObject currentTarget; // Armazena o alvo do ataque

    private void Start()
    {
        InitializeComponents();

        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }
    }

    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        characterInfoManager = GetComponent<Mob_NPC_CharacterInfoManager>();
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        HandleInput();
    }

    void HandleInput()
    {
        if ((characterInfoManager.sliderRes?.value ?? 1) > 0)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
                BasicAttack(TypeCombat.Melee, "transitionCombat");

            if (Input.GetKeyUp(KeyCode.Alpha1))
                BasicAttack(TypeCombat.WeaponMelee, "transitionWeaponCombat", 1);

            if (Input.GetKeyUp(KeyCode.Alpha2))
                BasicAttack(TypeCombat.WeaponMelee, "transitionWeaponCombat", 2);

            if (Input.GetKeyUp(KeyCode.Alpha3))
                BasicAttack(TypeCombat.WeaponMelee, "transitionWeaponCombat", 3);
        }
    }

    void BasicAttack(TypeCombat type, string integerCombat, int attack = 0)
    {
        if (animator.GetBool("inCombat") || animator.GetInteger("transitionMovement") > 0) return;

        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 direction = transform.forward;

        // Verifica se há um inimigo na linha de ataque
        if (Physics.Raycast(origin, direction, out RaycastHit hit, 2f))
        {
            GameObject target = hit.collider.gameObject;
            if (target.CompareTag("Enemy") || target.CompareTag("Player"))
            {
                currentTarget = target; // Armazena o inimigo atual
                int attackIndex = attack > 0 ? attack : Random.Range(1, 4);

                // Configura a animação de ataque
                animator.SetInteger(integerCombat, attackIndex);
                animator.SetInteger("typeCombat", (int)type);
                animator.SetBool("inCombat", true);
            }
        }
    }

    /// <summary>
    /// Método chamado via evento de animação para aplicar dano.
    /// </summary>
    public void ApplyDamage()
    {
        if (currentTarget != null && photonView.IsMine)
        {
            characterInfoManager.sliderRes.value = Mathf.Clamp(characterInfoManager.sliderRes.value - 0.05f, 0, characterInfoManager.sliderRes.maxValue);

            if (currentTarget.TryGetComponent<Mob_NPC_CharacterInfoManager>(out Mob_NPC_CharacterInfoManager infoManager))
            {
                infoManager.TakeDamage(50f, currentTarget.tag);
            }
        }
    }

    public void ResetCombatState()
    {
        if (photonView.IsMine)
        {
            animator.SetInteger("transitionCombat", 0);
            animator.SetInteger("transitionWeaponCombat", 0);
            animator.SetInteger("typeCombat", 0);
            animator.SetBool("inCombat", false);
        }
    }
}