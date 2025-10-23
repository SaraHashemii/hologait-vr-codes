using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicates
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scene loads
    }

    public float PillarDistance { get; set; } = 3f;

    //In CM
    public float PathwayDistance { get; set; } = 65f;

    public float PillarDiameter { get; set; } = 1;
}
