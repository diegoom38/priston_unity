using Assets.Enums;
using Assets.Scripts;
using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class MeleeCombat : MonoBehaviour
{
    private Animator animator;
    private Slider sliderRes;
    private Slider sliderMana;
    private Slider sliderHp;
    private float resRegenRate = 0.05f; // Taxa de regeneração por segundo
    private float maxResValue = 1.0f;  // Valor máximo do sliderRes

    private void Start()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        InitializeSliders();
    }

    void Update()
    {
        HandleInput();
        RegenerateRes();
    }

    void InitializeSliders()
    {
        sliderRes = FindSliderByTag("SliderRes");
        sliderMana = FindSliderByTag("SliderMana");
        sliderHp = FindSliderByTag("SliderHp");
    }

    Slider FindSliderByTag(string tag)
    {
        GameObject gameObject = GameObject.FindGameObjectWithTag(tag);
        if (gameObject != null && gameObject.TryGetComponent(out Slider slider))
            return slider;

        return null;
    }

    void HandleInput()
    {
        animator.SetInteger("transitionCombat", (int)CombatMeleeList.Idle);

        if (
            Input.GetKeyUp(KeyCode.Mouse0) &&
            (sliderRes?.value ?? 1) > 0
        )
            BasicAttack();

    }

    void BasicAttack()
    {
        if (animator.GetBool("inCombat")) return;

        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 direction = transform.forward;

        // Inicia a coroutine para lidar com a animação e a lógica de ataque
        StartCoroutine(HandleAttack(origin, direction));
    }

    private IEnumerator HandleAttack(Vector3 origin, Vector3 direction)
    {
        // Realiza o ataque se houver um inimigo na linha de ataque
        if (Physics.Raycast(origin, direction, out RaycastHit hit, 2f))
        {
            GameObject target = hit.collider.gameObject;

            if (target.CompareTag("Enemy"))
            {
                System.Random randNum = new();
                int randomBasicAttack = randNum.Next(1, 4);

                animator.SetInteger("transitionCombat", randomBasicAttack);
                animator.SetInteger("typeCombat", (int)TypeCombat.Melee);
                animator.SetBool("inCombat", true);

                // Atualiza o estado da animação após a transição
                yield return new WaitForEndOfFrame(); // Espera 1 frame para garantir que a nova animação carregue

                // Obtém o estado atual da Layer 1 (Ataque Melee)
                AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(1);
                float timeRemaining = (1 - currentState.normalizedTime) * currentState.length;

                yield return new WaitForSeconds(timeRemaining);

                // Reduz resistência apenas se a animação final não for "FightIdle"
                if (!currentState.IsName("FightIdle"))
                {
                    sliderRes.value = Mathf.Clamp(sliderRes.value - 0.05f, 0, sliderRes.maxValue);
                    target.GetComponent<Enemy>().life -= 40;
                }
            }
        }

        animator.SetInteger("typeCombat", (int)CombatMeleeList.Idle);
        animator.SetBool("inCombat", false);
    }

    void RegenerateRes()
    {
        if (sliderRes != null && sliderRes.value < maxResValue)
        {
            sliderRes.value = Mathf.Min(sliderRes.value + resRegenRate * Time.deltaTime, maxResValue);
        }
    }
}
