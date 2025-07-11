using System;
using UnityEngine;
using TMPro;

public class DayNightManager : MonoBehaviour
{
    [SerializeField, HideInInspector] private Transform directionalLight;
    [SerializeField, Tooltip("Duration of the day in seconds")] private int durationDay = Seconds.AllDay;
    [SerializeField] private TextMeshProUGUI textHour;

    private float seconds;

    // Ajustei os valores de exposição para ficar menos escuro à noite
    [SerializeField, Tooltip("Minimum skybox exposure at night")] private float minExposure = 0.4f;
    [SerializeField, Tooltip("Maximum skybox exposure during the day")] private float maxExposure = 1.2f;

    // Adicionei uma curva para controlar melhor as transições
    [SerializeField]
    private AnimationCurve exposureCurve = new AnimationCurve(
        new Keyframe(0f, 0.4f),
        new Keyframe(0.25f, 0.5f),
        new Keyframe(0.5f, 1.2f),
        new Keyframe(0.75f, 0.5f),
        new Keyframe(1f, 0.4f)
    );

    private float multiplier;

    void Start()
    {
        directionalLight = GameObject.FindGameObjectWithTag("DirectionalLight").transform;
        DateTime currentTime = DateTime.Now;
        seconds = (currentTime.Hour * 3600) + (currentTime.Minute * 60) + currentTime.Second;
        multiplier = (float)Seconds.AllDay / durationDay;
    }

    void Update()
    {
        seconds = (seconds + Time.deltaTime * multiplier) % Seconds.AllDay;
        UpdateLighting();
        UpdateTimeDisplay();
        UpdateSkyboxExposure();
    }

    private void UpdateSkyboxExposure()
    {
        float normalizedTime = seconds / Seconds.AllDay;

        // Usando a curva para controlar a exposição
        float exposure = exposureCurve.Evaluate(normalizedTime);

        // Aplicando um pequeno ajuste para evitar valores extremos
        exposure = Mathf.Clamp(exposure, minExposure, maxExposure);
        RenderSettings.skybox.SetFloat("_Exposure", exposure);
    }

    private void UpdateLighting()
    {
        float rotationX = Mathf.Lerp(-90f, 270f, seconds / Seconds.AllDay);
        directionalLight.rotation = Quaternion.Euler(rotationX, 0f, 0f);
        float skyboxRotation = (seconds / Seconds.AllDay) * 360f;
        RenderSettings.skybox.SetFloat("_Rotation", skyboxRotation);
    }

    private void UpdateTimeDisplay()
    {
        if (textHour != null)
        {
            textHour.text = TimeSpan.FromSeconds(seconds).ToString(@"hh\:mm");
        }
    }
}

public static class Seconds
{
    public const int AllDay = 86400;
}