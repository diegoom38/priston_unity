using System;
using UnityEngine;
using TMPro;

public class DayNightManager : MonoBehaviour
{
    // Reference to the directional light in the scene, representing the sun
    [SerializeField] private Transform directionalLight;

    // Duration of a full day in seconds (set in the Unity Inspector)
    [SerializeField, Tooltip("Duration of the day in seconds")] private int durationDay = 3600;

    // Reference to the TextMeshProUGUI component to display the current time
    [SerializeField] private TextMeshProUGUI textHour;

    // Tracks the number of seconds that have passed in the current day cycle
    private float seconds;

    // Minimum and maximum exposure values for the skybox
    [SerializeField, Tooltip("Minimum skybox exposure at night")] private float minExposure = 0.02f;
    [SerializeField, Tooltip("Maximum skybox exposure during the day")] private float maxExposure = 1f;

    // Multiplier to speed up or slow down time based on the duration of the day
    private float multiplier;

    void Start()
    {
        // Get the current time from the system
        DateTime currentTime = DateTime.Now;

        // Convert the current time to seconds since midnight
        seconds = (currentTime.Hour * 3600) + (currentTime.Minute * 60) + currentTime.Second;

        // Calculate the time multiplier based on the desired day duration
        multiplier = (float)Seconds.AllDay / durationDay;
    }

    void Update()
    {
        // Increment the number of seconds based on the delta time and multiplier
        // The modulus operator ensures that 'seconds' loops back to 0 after a full day
        seconds = (seconds + Time.deltaTime * multiplier) % Seconds.AllDay;

        // Update the rotation of the directional light to simulate the sun's movement
        UpdateLighting();

        // Update the on-screen display to show the current time
        UpdateTimeDisplay();

        UpdateSkyboxExposure();
    }

    private void UpdateSkyboxExposure()
    {
        // Normalize time to a range of 0 to 1 (0 = midnight, 0.5 = noon, 1 = next midnight)
        float normalizedTime = seconds / Seconds.AllDay;

        // Calculate the exposure value based on the time of day
        float exposure = normalizedTime < 0.5f
            ? Mathf.Lerp(minExposure, maxExposure, Mathf.InverseLerp(0.25f, 0.5f, normalizedTime)) // Dawn
            : Mathf.Lerp(maxExposure, minExposure, Mathf.InverseLerp(0.5f, 0.75f, normalizedTime)); // Dusk

        // Clamp exposure between min and max values
        exposure = Mathf.Clamp(exposure, minExposure, maxExposure);

        // Apply the exposure value to the skybox
        RenderSettings.skybox.SetFloat("_Exposure", exposure);
    }

    private void UpdateLighting()
    {
        // Calculate the rotation angle for the light based on the time of day
        float rotationX = Mathf.Lerp(-90f, 270f, seconds / Seconds.AllDay);

        // Apply the calculated rotation to the directional light
        directionalLight.rotation = Quaternion.Euler(rotationX, 0f, 0f);

        // Rotate the skybox
        float skyboxRotation = (seconds / Seconds.AllDay) * 360f; // Full rotation during the day
        RenderSettings.skybox.SetFloat("_Rotation", skyboxRotation);
    }

    private void UpdateTimeDisplay()
    {
        if (textHour != null)
        {
            // Convert the elapsed seconds into hours and minutes, then display them on the screen
            textHour.text = TimeSpan.FromSeconds(seconds).ToString(@"hh\:mm");
        }
    }
}

// Static class containing constants for time calculations
public static class Seconds
{
    // Constant for the total number of seconds in a full day (24 hours)
    public const int AllDay = 86400;
}