using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DictionaryManager : MonoBehaviour
{
    public GameObject[] WordLists = new GameObject[3];
    public GameObject[] MenuControlButtons = new GameObject[2];
    public GameObject PanelImage;
    public GameObject ReturnButton;
    public GameObject WordTitlesContainer;
    public GameObject MenuModeContainer;
    public GameObject WordContainerPrefab;
    public GameObject WordMeaningModeContainer;
    public GameObject Word;
    public GameObject Description;
    public GameObject Example;
    public int WordsPerList;
    public float WordMargin;
    private List<DictionaryWord> _words { get; set; }
    private List<GameObject> _wordTitles;
    private float _wordsListSize;
    private int _currentMenuPage;
    private int _currentPage;
    private int _wordsPerPage;
    private int _menuPagesCount;
    private List<string> _currentTextPages;
    private void Awake()
    {
        BetterStreamingAssets.Initialize();
        _words = new List<DictionaryWord>();
        _wordTitles = new List<GameObject>();
        _wordsListSize = WordLists[0].GetComponent<RectTransform>().rect.height;
        SetUpControlButtons();
        MenuControlButtons[1].SetActive(false);
        SetUpWordTitles();
        SetUpMenu();
        GameObject.Find("SceneIsReady").GetComponent<SceneIsReadyCheck>().IsReady = true;
    }
    
    private void SetUpWordTitles()
    {
        _wordsPerPage = WordsPerList * WordLists.Length;
        string[] rawWords = BetterStreamingAssets.ReadAllText("/database/dictionary/words.txt")
            .Trim().Split('\n').Select(w => w.Trim()).ToArray();
        _menuPagesCount = Convert.ToInt32(Math.Ceiling(rawWords.Length / Convert.ToDecimal(_wordsPerPage)));
        for (int i = 0; i < rawWords.Length; i++)
        {
            string[] entity = rawWords[i].Split('—');
            _words.Add(new DictionaryWord(entity[0].Trim(), entity[1].Trim(), entity[2].Trim()));
        }
        _words = _words.OrderBy(w => w.Name).ToList();
        for (int i = 0; i < _words.Count; i++)
        {
            GameObject wordTitlePlaceholder = Instantiate(WordContainerPrefab, 
                WordMeaningModeContainer.transform.position, Quaternion.identity);
            wordTitlePlaceholder.name = $"{_words[i].Name}";
            wordTitlePlaceholder.GetComponentInChildren<Text>().text = wordTitlePlaceholder.name;
            wordTitlePlaceholder.transform.SetParent(WordTitlesContainer.transform, 
                false);
            SetWordTitleButton(wordTitlePlaceholder, _words[i]);
            _wordTitles.Add(wordTitlePlaceholder);
        }
    }

    private void SetWordTitleButton(GameObject wordTitle, DictionaryWord word) => wordTitle.GetComponent<Button>().onClick
        .AddListener(() => SetUpWordMeaningMode(word));
    private void SetUpControlButtons()
    {
        ReturnButton.GetComponent<Button>().onClick.AddListener(() => ReturnMenuMode());
        MenuControlButtons[0].GetComponent<Button>().onClick
            .AddListener(() => SwitchMenuPage(true));
        MenuControlButtons[1].GetComponent<Button>().onClick
            .AddListener(() => SwitchMenuPage(false));
    }

    private void ReturnMenuMode()
    {
        WordMeaningModeContainer.SetActive(false);
        MenuModeContainer.SetActive(true);
    }
    
    private void CleanStoryLists()
    {
        List<GameObject> wordsToRemove = new List<GameObject>();
        wordsToRemove = _wordTitles.Skip(_currentMenuPage * _wordsPerPage)
            .Take(_wordsPerPage).ToList();
        for (int i = 0; i < wordsToRemove.Count; i++)
        {
            if (wordsToRemove[i].transform.position != WordTitlesContainer.transform.position)
            {
                wordsToRemove[i].transform.SetParent(WordTitlesContainer.transform, false);
                wordsToRemove[i].transform.position = WordTitlesContainer.transform.position;
            }
        }
    }

    private void SetUpWordMeaningMode(DictionaryWord word)
    {
        PanelImage.GetComponent<Image>().sprite = Resources.Load<Sprite>($"UI/Images/Dictionary/{word.Name}");
        Word.GetComponent<Text>().text = word.Name.ToUpper();
        Description.GetComponent<Text>().text = word.Meaning;
        Example.GetComponent<Text>().text = word.Example;
        MenuModeContainer.SetActive(false);
        WordMeaningModeContainer.SetActive(true);
    }
    
    private void SwitchMenuPage(bool next)
    {
        if (next && _currentMenuPage + 1 != _menuPagesCount)
        {
            CleanStoryLists();
            _currentMenuPage++;
            SetUpMenu();
            if (_currentMenuPage + 1 == _menuPagesCount)
                MenuControlButtons[0].SetActive(false);
            if (MenuControlButtons[1].activeSelf == false)
                MenuControlButtons[1].SetActive(true);
        }
        if (!next && _currentMenuPage != 0)
        {
            CleanStoryLists();
            _currentMenuPage--;
            SetUpMenu();
            if (_currentMenuPage == 0)
                MenuControlButtons[1].SetActive(false);
            if (MenuControlButtons[0].activeSelf == false)
                MenuControlButtons[0].SetActive(true);
        }
    }

    private void SetUpMenu()
    {
        int k = 0;
        bool listIsEmpty = false;
        List<GameObject> words = _wordTitles.Skip(_currentMenuPage * _wordsPerPage).Take(_wordsPerPage).ToList();
        for (int i = 0; i < WordLists.Length; i++)
        {
            if (words.Count - k < WordsPerList)
            {
                float size = (WordContainerPrefab.GetComponent<RectTransform>().rect.height) *
                             (words.Count - k) + WordMargin * (words.Count - k);
                WordLists[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
                WordLists[i].GetComponent<RectTransform>().anchoredPosition = MenuModeContainer.transform.position;
            }
            else
                WordLists[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                    _wordsListSize);
            for (int j = 0; j < WordsPerList; j++)
            {
                try
                {
                    words[k].transform.SetParent(WordLists[i].transform, false);
                    words[k].transform.position = WordLists[i].transform.position;
                    k++;
                }
                catch (Exception)
                {
                    listIsEmpty = true;
                    break;
                }
            }

            if (listIsEmpty)
                break;
        }
    }
}
