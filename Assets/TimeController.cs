using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeController : MonoBehaviour
{
    [SerializeField]
    private float timeMultiplier;

    /* Starting hour*/
    [SerializeField]
    private float startHour;

    [SerializeField]
    private TextMeshProUGUI timeText;

    private DateTime currentTime;

    [SerializeField]
    private Light sunLight;

    [SerializeField]
    private float sunriseHour;

    [SerializeField]
    private float sunsetHour;   

    private TimeSpan sunriseTime;

    private TimeSpan sunsetTime;

    [SerializeField]
    private Color dayAmbientLight;

    [SerializeField]
    private Color nightAmbientLight;

    [SerializeField]
    private AnimationCurve lightChangeCurve;

    [SerializeField]
    private float maxSunLightIntensity;

    [SerializeField]
    private Light moonLight;

    [SerializeField]
    private float maxMoonLightIntensity;

    // Start is called before the first frame update
    void Start()
    {
        // Setting current time
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);

        // Setting sunrise and sunset time
        sunriseTime = TimeSpan.FromHours(sunriseHour);
        sunsetTime = TimeSpan.FromHours(sunsetHour);
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimeOfDay();
        RotateSun();
        UpdateLightSettings();
    }

    private void UpdateTimeOfDay() {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);

        // If time is not null then set text to current time
        if(timeText != null){
            timeText.text = currentTime.ToString("HH:mm");
        }
    }

    private void RotateSun() {
        // Based on the current time, the directional light will be at a certain angle

        float sunLightRotation;

        if (currentTime.TimeOfDay.TotalMinutes > sunriseTime.TotalMinutes && currentTime.TimeOfDay.TotalMinutes < sunsetTime.TotalMinutes) {
            TimeSpan sunriseToSunsetDuration = CalculateTimeDifference(sunriseTime, sunsetTime);
            TimeSpan timeSinceSunrise = CalculateTimeDifference(sunriseTime, currentTime.TimeOfDay);

            // Percentage of the day thats passed
            double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;

            // Sun rotation will change based on percentage
            sunLightRotation = Mathf.Lerp(0, 180, (float)percentage);
        }
        else {
            TimeSpan sunsetToSunriseDuration = CalculateTimeDifference(sunsetTime, sunriseTime);
            TimeSpan timeSinceSunset = CalculateTimeDifference(sunsetTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunset.TotalMinutes / sunsetToSunriseDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(180, 360, (float)percentage);
        }

        sunLight.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right);
    }

    private void UpdateLightSettings(){
        // Gives a value of -1 or 1
        float dotProduct = Vector3.Dot(sunLight.transform.forward, Vector3.down);
        sunLight.intensity = Mathf.Lerp(0, maxSunLightIntensity, lightChangeCurve.Evaluate(dotProduct));
        moonLight.intensity = Mathf.Lerp(maxMoonLightIntensity, 0, lightChangeCurve.Evaluate(dotProduct));
        RenderSettings.ambientLight = Color.Lerp(nightAmbientLight, dayAmbientLight, lightChangeCurve.Evaluate(dotProduct));
    }

    private TimeSpan CalculateTimeDifference(TimeSpan fromTime, TimeSpan toTime) {
        TimeSpan difference = toTime - fromTime;

        // If the difference is negative, add 24 hours to get in the correct range
        if (difference.TotalMinutes < 0){
            difference += TimeSpan.FromHours(24);
        }

        return difference;
    }
}
