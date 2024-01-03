using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using WordGuess.Types;
using Random = System.Random;


public class WordGuessManager : MonoBehaviour
{
    public AssetLabelReference AudioAssetLabelReference;
    public AudioSource VoiceAudioSource;
    public AudioSource EffectAudioSource;
    public GameObject ImagePlaceholder;
    public GameObject LetterPlaceholderPrefab;
    public GameObject LetterPlaceholderContainer { get; set; }
    public GameObject[] LetterButtons { get; set; }
    public List<GameObject> LetterPlaceholders;
    public List<Sprite> LetterPlaceholderSprites;
    public Button ExitButton;
    public int CurrentLetter { get; set; } = 0;
    public int CurrentHighlightedLetter = 0;
    
    private List<Word> Words { get; set; }
    private string[] _letters { get; set; }
    private Word GeneratedDictionaryWord { get; set; }
    private string _word { get; set; }
    private int _numberOfLetterButtons { get; set; } = 9;
    private int _numberOfLetterPlaceholders { get; set; }
    private float _letterPlaceholderContainerMargin = 50f;
    private GameObject _clueButton;
    private GameObject _star;
    private Random _random = new Random();
    private RectTransform _letterPlaceholderContainerRectTransform;
    private RectTransform _letterPlaceholderPrefabRectTransform;
    private Transform _letterPlaceholderContainerTransform;
    private Transform _letterPlaceholderPrefabTransform;
    private List<Text> _letterPlaceholdersTexts;
    private AsyncOperationHandle<IList<AudioClip>> _soundEffectClips;

    private void Awake()
    {
        BetterStreamingAssets.Initialize();
        _clueButton = GameObject.Find("Clue");
        _star = GameObject.Find("Star");
        LetterButtons = new GameObject[_numberOfLetterButtons];
        _soundEffectClips = Addressables
            .LoadAssetsAsync<AudioClip>(AudioAssetLabelReference, null);
        ReadFiles();
        GenerateNewWord();
        GenerateLetterPlaceholders();
        InitializeLetterButtons();
        GenerateLetterButtons();
        SetUpExitButton();
        StartCoroutine(StartLevel());
    }

    private void SetUpExitButton()
    {
        ExitButton.onClick.AddListener(() => ReleaseMemory());
    }
    
    private IEnumerator StartLevel()
    {
        while (_soundEffectClips.Result == null)
            yield return null;
        EffectAudioSource.clip = _soundEffectClips.Result.FirstOrDefault(sc => sc.name == "correct");
        GameObject.Find("SceneIsReady").GetComponent<SceneIsReadyCheck>().IsReady = true;
    }
    
    private void ReleaseMemory() => Addressables.Release(_soundEffectClips);

    private void PlayAudio(bool correct)
    {
        int correctChance = Math.Abs(_random.Next(1, 1000000) % 3);
        int wrongChance = Math.Abs(_random.Next(1, 1000000) % 4);
        switch (correct)
        {
            case true:
                VoiceAudioSource.clip =
                    _soundEffectClips.Result.FirstOrDefault(sc => sc.name == $"correct{correctChance}");
                    VoiceAudioSource.Play();
                break;
            case false:
                VoiceAudioSource.clip = 
                    _soundEffectClips.Result.FirstOrDefault(sc => sc.name == $"wrong{wrongChance}");
                VoiceAudioSource.Play();
                break;
        }
    }

    private void SetUpLetterPlaceholderData()
    {
        LetterPlaceholderContainer = GameObject.Find("LetterPlaceholderContainer");
        _letterPlaceholderContainerRectTransform = LetterPlaceholderContainer.GetComponent<RectTransform>();
        _letterPlaceholderContainerTransform = LetterPlaceholderContainer.transform;
        _letterPlaceholderPrefabRectTransform = LetterPlaceholderPrefab.GetComponent<RectTransform>();
        _letterPlaceholdersTexts = new List<Text>();
    }

    private void GenerateLetterPlaceholders()
    {
        SetUpLetterPlaceholderData();
        _numberOfLetterPlaceholders = GeneratedDictionaryWord.Name.Length;
        LetterPlaceholders = new List<GameObject>();
        
        _letterPlaceholderContainerRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 
            _letterPlaceholderPrefabRectTransform.rect.width * _numberOfLetterPlaceholders +
            _letterPlaceholderContainerMargin * _numberOfLetterPlaceholders);
        _letterPlaceholderContainerRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 
            _letterPlaceholderPrefabRectTransform.rect.height);
        for (int i = 0; i < _numberOfLetterPlaceholders; i++)
        {
            GameObject letterPlaceholder = Instantiate(LetterPlaceholderPrefab, _letterPlaceholderContainerTransform.position, Quaternion.identity);
            letterPlaceholder.name = $"LetterPlaceholder ({i})";
            letterPlaceholder.transform.SetParent(LetterPlaceholderContainer.transform, false);
            LetterPlaceholders.Add(letterPlaceholder);
            _letterPlaceholdersTexts.Add(letterPlaceholder.GetComponentInChildren<Text>());
        }
    }

    private void ResetLetterPlaceholders()
    {
        for (int i = 0; i < _numberOfLetterPlaceholders; i++)
            Destroy(LetterPlaceholders[i]);
        GenerateLetterPlaceholders();
    }

    private void InitializeLetterButtons()
    {
        for (int i = 0; i != _numberOfLetterButtons; i++)
        {
            LetterButtons[i] = GameObject.Find($"LetterButton ({i})");
            LetterButtons[i].GetComponent<LetterButton>().WordGuess = this.gameObject;
        }
    }
    private void GenerateLetterButtons()
    {
        for (int i = 0; i != _numberOfLetterButtons; i++)
        {
            LetterButtons[i].GetComponent<LetterButton>().IsClicked = false;
            LetterButtons[i].GetComponent<Image>().sprite = LetterButtons[i].GetComponent<LetterButton>().NormalSprite;
            LetterButtons[i].GetComponent<LetterButton>().IsHighlighted = false;
            LetterButtons[i].GetComponentInChildren<Text>().text =
                "";
        }
        SetUpLetterButtons();
    }

    private void GenerateNewWord()
    {
        GeneratedDictionaryWord = Words[_random.Next(Words.Count)];
        GeneratedDictionaryWord.Name.Trim();
    }
    private void SetUpLetterButtons()
    {
        ImagePlaceholder.GetComponent<Image>().sprite = GeneratedDictionaryWord.Sprite;
        ImagePlaceholder.GetComponent<Image>().SetNativeSize();
        List<char> wordLetters = GeneratedDictionaryWord.Name.ToList();
        List<int> numbers = new List<int>();
        for (int i = 0; i < _numberOfLetterButtons; i++)
            numbers.Add(i);
        int randomWordLetterIndex = 0;
        int randomNumberOfLetterIndex = 0;
        for (int i = 0; i < GeneratedDictionaryWord.Name.Length; i++)
        {
            randomWordLetterIndex = _random.Next(wordLetters.Count);
            randomNumberOfLetterIndex = numbers[_random.Next(numbers.Count)];
            LetterButtons[randomNumberOfLetterIndex]
                .GetComponentInChildren<Text>().text = 
                Convert.ToString(wordLetters[randomWordLetterIndex]);
            numbers.Remove(randomNumberOfLetterIndex);
            wordLetters.RemoveAt(randomWordLetterIndex);
        }
    }
    private void ReadFiles()
    {
        if (BetterStreamingAssets.FileExists("/database/letters.txt"))
            _letters = BetterStreamingAssets.ReadAllText("/database/letters.txt").Split(' ');
        else
            Debug.Log("Could not find the file letters.txt");
        CreateWordList(BetterStreamingAssets.ReadAllText("/database/words.txt")
            .Split("\n")
            .Select(w => w.ToUpper())
            .Select(w => w.Trim()).ToArray());
    }

    private void CreateWordList(string[] wordNames)
    {
        Words = new List<Word>();
        for (int i = 0; i < wordNames.Length; i++)
            Words.Add(new Word(wordNames[i], Resources.Load<Sprite>($"UI/Images/WordGuess/{wordNames[i][0]}{String.Join("", wordNames[i].Skip(1).Take(wordNames[i].Length - 1).ToArray()).ToLower()}")));
    }

    public void DisposeLetterButton(GameObject letterButton)
    {
        bool highlightedLetterButtonExists =
            LetterButtons.FirstOrDefault(o => o.GetComponent<LetterButton>().IsHighlighted);
        if (highlightedLetterButtonExists == false 
            || (highlightedLetterButtonExists && letterButton.GetComponent<LetterButton>().IsHighlighted))
        {
            letterButton.GetComponent<LetterButton>().IsClicked = true;
            string letterButtonText = letterButton.GetComponentInChildren<Text>().text;
            LetterPlaceholders[CurrentLetter]
                .GetComponentInChildren<Text>().text = letterButtonText;
            letterButton.GetComponent<Image>().sprite = letterButton.GetComponent<LetterButton>().NormalSprite;
            _word += letterButton.GetComponentInChildren<Text>().text;
            letterButton.transform.GetChild(0).gameObject.SetActive(false);
            letterButton.GetComponent<LetterButton>().IsHighlighted = false;
            CurrentHighlightedLetter++;
            CurrentLetter++;
            _clueButton.GetComponent<Clue>().IsClicked = false;
        }
        if (CurrentLetter == _numberOfLetterPlaceholders)
            StartCoroutine(ClearLetterPlaceholders());
    }


    public void HighlightLetters()
    {
        if (CurrentHighlightedLetter <= GeneratedDictionaryWord.Name.Length)
        {
            GameObject letterButton = LetterButtons.FirstOrDefault(o => o.GetComponent<LetterButton>().IsClicked == false 
                                                                        && o.GetComponentInChildren<Text>().text == GeneratedDictionaryWord.Name[CurrentHighlightedLetter].ToString());
            letterButton.GetComponent<Image>().sprite = letterButton.GetComponent<LetterButton>().HighlightedSprite;
            letterButton.GetComponent<LetterButton>().IsHighlighted = true;
        }
    }

    private void IncreaseScore() => _star.GetComponentInChildren<Text>().text =
        Convert.ToString(Convert.ToInt32(_star.GetComponentInChildren<Text>().text) + GeneratedDictionaryWord.Name.Length);

    public IEnumerator ClearLetterPlaceholders()
    {
        if (_word == GeneratedDictionaryWord.Name)
        {
            EffectAudioSource.Play();
            for (int i = 0; i < LetterPlaceholders.Count; i++)
                LetterPlaceholders[i].GetComponent<Image>().sprite = LetterPlaceholderSprites[2];
            while (EffectAudioSource.isPlaying)
                yield return null;
            int randomNumber = _random.Next(1, 100)%5;
            if (randomNumber == 2 || randomNumber == 1)
                PlayAudio(true);
        }
        else
        {
            for (int i = 0; i < LetterPlaceholders.Count; i++)
                LetterPlaceholders[i].GetComponent<Image>().sprite = LetterPlaceholderSprites[1];
            PlayAudio(false);
        }
        while (VoiceAudioSource.isPlaying)
            yield return null;
        yield return new WaitForSeconds(0.7f);
        for (int i = 0; i < LetterPlaceholders.Count; i++)
            _letterPlaceholdersTexts[i].text = "";
        for (int i = 0; i < LetterButtons.Length; i++)
        {
            LetterButtons[i].transform.GetChild(0).gameObject.SetActive(true);
            LetterButtons[i].GetComponent<LetterButton>().IsClicked = false;
        }
        for (int i = 0; i < LetterPlaceholders.Count; i++)
            LetterPlaceholders[i].GetComponent<Image>().sprite = LetterPlaceholderSprites[0];
        CurrentLetter = 0;
        CurrentHighlightedLetter = 0;
        if (_word == GeneratedDictionaryWord.Name)
        {
            IncreaseScore();
            GenerateNewWord();
            ResetLetterPlaceholders();
            GenerateLetterButtons();
        }
        _word = "";
    }
}
