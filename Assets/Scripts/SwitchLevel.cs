using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class SwitchLevel : MonoBehaviour
{
    public Animator Animator;
    public Slider LoadingBar;
    private static SwitchLevel _switchLevel;
    private static readonly int Transition = Animator.StringToHash("Transition");
    private static readonly int Default = Animator.StringToHash("Default");
    private static readonly int Begin = Animator.StringToHash("Begin");

    public void Awake()
    {
        DontDestroyOnLoad(this);
        if (_switchLevel == null)
            _switchLevel = this;
        else
            Destroy(gameObject);
    }
    

    private void ResetScene()
    {
        LoadingBar.value = 0;
        Animator.SetTrigger(Default);
    }
    public void Switch(string levelName, Button button) => StartCoroutine(LevelSwitch(levelName, button));
    private IEnumerator LevelSwitch(string levelName, Button button)
    {
        ResetScene();
        button.interactable = false;
        Animator.SetTrigger(Begin);
        while (Animator.GetCurrentAnimatorStateInfo(Animator.GetLayerIndex("Base Layer")).normalizedTime > 1.0f)
            yield return null;
        while (Animator.GetCurrentAnimatorStateInfo(Animator.GetLayerIndex("Base Layer")).normalizedTime < 1.0f)
            yield return null;
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelName);
        operation.allowSceneActivation = false;
        for (int i = 0; i < 5; i++)
        {
            LoadingBar.value += 0.1f;
            yield return null;
        }
        operation.allowSceneActivation = true;
        SceneIsReadyCheck sceneIsReady = GameObject.Find("SceneIsReady").GetComponent<SceneIsReadyCheck>();
        while (!sceneIsReady.IsReady)
            yield return null;
        for (int i = 0; i < 4; i++)
        {
            LoadingBar.value += 0.1f;
            yield return null;
        }
        LoadingBar.value += LoadingBar.maxValue - LoadingBar.value;
        Animator.SetTrigger(Transition);
        while (Animator.GetCurrentAnimatorStateInfo(Animator.GetLayerIndex("Base Layer")).normalizedTime > 1.0f)
            yield return null;
        while (Animator.GetCurrentAnimatorStateInfo(Animator.GetLayerIndex("Base Layer")).normalizedTime < 1.0f)
            yield return new WaitForSeconds(0.01f);
        sceneIsReady.IsReady = false;
    }
}
