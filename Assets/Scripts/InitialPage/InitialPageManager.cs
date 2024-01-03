using UnityEngine;

public class InitialPageManager : MonoBehaviour
{
    void Awake()
    {
        GameObject.Find("SceneIsReady").GetComponent<SceneIsReadyCheck>().IsReady = true;
    }

}
