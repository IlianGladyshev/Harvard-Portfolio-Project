using System.Collections;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class LevelSwitchButton : MonoBehaviour
{
    private Button _button;
    public void Awake()
    {
        _button = GetComponent<Button>();
        _button.interactable = true;
        StartCoroutine(FindLevelSwitcher());
    }

    private IEnumerator FindLevelSwitcher()
    {
        SceneIsReadyCheck sceneIsReady = GameObject.Find("SceneIsReady")
            .GetComponent<SceneIsReadyCheck>();
        while (sceneIsReady.IsReady == false)
            yield return null;
        GameObject levelSwitch = GameObject.Find("LevelLoading");
        _button.onClick.AddListener(() => 
            levelSwitch.GetComponent<SwitchLevel>()
                .Switch(this.gameObject.tag
                    .Split("Button")[0], _button));
    }
}
