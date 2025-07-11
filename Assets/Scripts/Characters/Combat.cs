using Assets.Enums;
using Assets.Scripts.Core.Managers;
using Assets.Scripts.Manager;
using Photon.Pun;
using UnityEngine;

public class Combat : MonoBehaviourPun
{
    private Animator animator;
    private Mob_NPC_CharacterInfoManager characterInfoManager;
    private GameObject currentTarget;


    [SerializeField] private AudioSource combatAudioSource;
    [SerializeField] private AudioClip[] combatAudioClips;

    private void Start()
    {
        InitializeComponents();

        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }
    }

    void Update()
    {
        InputManager.UpdateInputAttack();
    }

    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        characterInfoManager = GetComponent<Mob_NPC_CharacterInfoManager>();
    }

    private void OnEnable()
    {
        // Registrar eventos de input apenas se for o dono do objeto
        if (photonView.IsMine)
        {
            InputManager.OnMouseClick += OnMouseClick;
            InputManager.OnKeyAlpha1 += () => TryAttack(TypeCombat.WeaponMelee, "transitionWeaponCombat", 1);
            InputManager.OnKeyAlpha2 += () => TryAttack(TypeCombat.WeaponMelee, "transitionWeaponCombat", 2);
            InputManager.OnKeyAlpha3 += () => TryAttack(TypeCombat.WeaponMelee, "transitionWeaponCombat", 3);
        }
    }

    private void OnDisable()
    {
        if (photonView.IsMine)
        {
            InputManager.OnMouseClick -= OnMouseClick;
            InputManager.OnKeyAlpha1 -= () => TryAttack(TypeCombat.WeaponMelee, "transitionWeaponCombat", 1);
            InputManager.OnKeyAlpha2 -= () => TryAttack(TypeCombat.WeaponMelee, "transitionWeaponCombat", 2);
            InputManager.OnKeyAlpha3 -= () => TryAttack(TypeCombat.WeaponMelee, "transitionWeaponCombat", 3);
        }
    }

    private void OnMouseClick()
    {
        TryAttack(TypeCombat.Melee, "transitionCombat");
    }

    private void TryAttack(TypeCombat type, string transitionName, int attack = 0)
    {
        if ((characterInfoManager.sliderRes?.value ?? 1) <= 0) return;
        if (animator.GetBool("inCombat") || animator.GetInteger("transitionMovement") > 0) return;

        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, 2f))
        {
            GameObject target = hit.collider.gameObject;
            if (target.CompareTag("Enemy") || target.CompareTag("Player"))
            {
                currentTarget = target;
                int attackIndex = attack > 0 ? attack : Random.Range(1, 4);

                animator.SetInteger(transitionName, attackIndex);
                animator.SetInteger("typeCombat", (int)type);
                animator.SetBool("inCombat", true);
            }
        }
    }

    public void ApplyDamage()
    {
        if (currentTarget != null && photonView.IsMine)
        {
            characterInfoManager.sliderRes.value = Mathf.Clamp(
                characterInfoManager.sliderRes.value - 0.05f,
                0,
                characterInfoManager.sliderRes.maxValue);

            if (currentTarget.TryGetComponent(out Mob_NPC_CharacterInfoManager infoManager))
            {
                infoManager.TakeDamage(100f, currentTarget.tag);
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

    private void SoundCombat()
    {
        combatAudioSource.PlayOneShot(combatAudioClips[UnityEngine.Random.Range(0, combatAudioClips.Length)]);
    }
}
