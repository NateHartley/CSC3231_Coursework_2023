using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    float fps;
    [SerializeField] float updateFrequency = 0.2f;
    float updateTimer;

    // SerializeField Links to the Unity scene
    [SerializeField] TextMeshProUGUI fpsTitle;

    private void UpdatefpsDisplay() {
        updateTimer -= Time.deltaTime;
        if (updateTimer <= 0f) {
            fps = 1f / Time.unscaledDeltaTime;
            fpsTitle.text = Mathf.Round(fps) + " fps";
            updateTimer = updateFrequency;
        }
    }

    void Update() {
        UpdatefpsDisplay();
    }

    void Start() {
        updateTimer = updateFrequency;
    }
}
