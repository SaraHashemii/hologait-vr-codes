using UnityEngine;
using TMPro;

public class FPSCounterUI : MonoBehaviour
{
    public TextMeshProUGUI fpsTextUI;  
    public float updateInterval = 0.5f;

    private float accumulatedTime = 0f;
    private int frames = 0;
    private float fps = 0f;
    private float timeLeft;

    void Start()
    {
        timeLeft = updateInterval;

        if (!fpsTextUI)
        {
            Debug.LogWarning("[FPSCounterUI] No TextMeshProUGUI assigned.");
        }
    }

    void Update()
    {
        timeLeft -= Time.unscaledDeltaTime;
        accumulatedTime += Time.unscaledDeltaTime;
        frames++;

        if (timeLeft <= 0f)
        {
            fps = frames / accumulatedTime;
            timeLeft = updateInterval;
            accumulatedTime = 0f;
            frames = 0;

            if (fpsTextUI)
                fpsTextUI.text = $"FPS: {fps:F1}";
        }
    }
}