using UnityEngine;

public class SceneIsReadyCheck : MonoBehaviour
{
    public bool IsReady { get; set; } = false;
    private static SceneIsReadyCheck _sceneIsReadyCheck;

    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (_sceneIsReadyCheck == null)
            _sceneIsReadyCheck = this;
        else
            Destroy(gameObject);
    }
}
