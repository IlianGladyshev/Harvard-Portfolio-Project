using UnityEngine;

public class LevelLoadingInitializer : MonoBehaviour
{
    public GameObject LevelLoading;
    public GameObject SceneIsReadyCheck;
    void Awake()
    {
        DontDestroyOnLoad(LevelLoading);
        DontDestroyOnLoad(SceneIsReadyCheck);
        Destroy(gameObject);
    }
    
}
