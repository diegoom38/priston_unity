using Assets.Enums;
using Assets.Scripts;
using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class Combat : MonoBehaviour
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
        animator.SetInteger("transitionCombat", (int)CombatList.Idle);

        if (
            Input.GetKeyUp(KeyCode.Mouse0) &&
            (sliderRes?.value ?? 1) > 0
        )
            BasicAttack();

    }

    void BasicAttack()
    {
        if (animator.GetBool("isAttacking")) return;

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
                animator.SetBool("isAttacking", true);

                float maxSecondsAttack = 0.65f;

                // Obtém o estado atual da animação
                AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);

                // Aguarda a animação terminar ou o tempo máximo
                float waitTime = Mathf.Min(currentState.length, maxSecondsAttack);
                yield return new WaitForSeconds(waitTime);

                // Diminui o valor do slider de resistência
                sliderRes.value = Mathf.Clamp(sliderRes.value - 0.05f, 0, sliderRes.maxValue);

                // Destroi o alvo se for um inimigo

                target.GetComponent<Enemy>().life -= 40;
            }
        }

        animator.SetBool("isAttacking", false);
    }

    void RegenerateRes()
    {
        if (sliderRes != null && sliderRes.value < maxResValue)
        {
            sliderRes.value = Mathf.Min(sliderRes.value + resRegenRate * Time.deltaTime, maxResValue);
        }
    }
}
