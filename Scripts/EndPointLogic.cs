using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPointLogic : MonoBehaviour
{
    // Backing field for the singleton instance
    private static EndPointLogic _instance;

    /// <summary>
    /// Global access point to the singleton.
    /// </summary>
    public static EndPointLogic Instance
    {
        get
        {
            if (_instance == null)
            {
                // Try to find an existing instance in the scene
                _instance = FindObjectOfType<EndPointLogic>();

                // If none exists, create a new one
                if (_instance == null)
                {
                    GameObject go = new GameObject(nameof(EndPointLogic));
                    _instance = go.AddComponent<EndPointLogic>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            // First time this runs, assign and optionally persist
            _instance = this;
            DontDestroyOnLoad(gameObject);  // Comment out if you don’t want persistence
        }
        else if (_instance != this)
        {
            // If another instance already exists, destroy this one
            Destroy(gameObject);
        }
    }

    
    public void DoSomething()
    {
        Debug.Log("Singleton instance is working!");
    }

    // You can still use Start/Update as usual:
    private void Start()
    {
        // …
    }

    private void Update()
    {
        // …
    }
}

