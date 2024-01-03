using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = System.Random;

public class LetterNameManager : MonoBehaviour
{
    public AssetLabelReference AudioAssetLabelReference;
    public GameObject LetterContainer;
    public GameObject PlayAudioButton;
    public GameObject NextLetterButton;
    public Button ExitButton;
    private List<string> _letters;
    private AsyncOperationHandle<IList<AudioClip>> _letterClips;
    private string _currentLetter;
    private Random _random = new Random();
    // Start is called before the first frame update
    void Awake()
    {
        BetterStreamingAssets.Initialize();
        _letters = new List<string>();
        _letters = BetterStreamingAssets.ReadAllText("/database/letters.txt").Split(' ').ToList();
        _letterClips = Addressables
            .LoadAssetsAsync<AudioClip>(AudioAssetLabelReference, null);
        SetUpControlButtons();
        NextLetter();
        StartCoroutine(StartLevel());
    }
    
    private IEnumerator StartLevel()
    {
        while (_letterClips.Result == null)
            yield return null;
        GameObject.Find("SceneIsReady").GetComponent<SceneIsReadyCheck>().IsReady = true;
    }

    private void SetUpControlButtons()
    {
        PlayAudioButton.GetComponent<Button>()
            .onClick.AddListener(() => PlayAudio());
        NextLetterButton.GetComponent<Button>()
            .onClick.AddListener(() => NextLetter());
        ExitButton.onClick.AddListener(() => ReleaseMemory());
    }

    private void PlayAudio()
    {
        PlayAudioButton.GetComponent<AudioSource>().clip = _letterClips.Result.FirstOrDefault(l => l.name == _currentLetter.ToLower());
            PlayAudioButton.GetComponent<AudioSource>().Play();
    }

    private void NextLetter()
    {
        _currentLetter = _letters[_random.Next(0, _letters.Count)];
        LetterContainer.GetComponentInChildren<Text>().text = _currentLetter;
    }
    
    private void ReleaseMemory() => Addressables.Release(_letterClips);
}
