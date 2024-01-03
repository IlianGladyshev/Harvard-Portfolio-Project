using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AudioTexts;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class TextManager : MonoBehaviour
{
    public GameObject[] StoryLists = new GameObject[2];
    public GameObject[] ControlButtons = new GameObject[2];
    public GameObject[] MenuControlButtons = new GameObject[2];
    public GameObject[] ModesControlButtons = new GameObject[3];
    public GameObject[] ContentControlButtons = new GameObject[2];
    public Sprite[] ContentControlButtonsSprites = new Sprite[4];
    public GameObject Drawing;
    public GameObject DrawingContainer;
    public GameObject DrawingField;
    public GameObject ImageContainer;
    public GameObject StoryTitlesContainer;
    public GameObject MenuModeContainer;
    public GameObject StoryTextContainer;
    public GameObject StoryTitleContainerPrefab;
    public GameObject ReadingModeContainer;
    public GameObject DrawingModeContainer;
    public GameObject CheckModeContainer;
    public GameObject ReturnButton;
    public int StoryTitlesPerList;
    public float StoryTitleMargin;
    public int LinesOnPage;
    public int SymbolsOnLine;
    private List<Story> _stories;
    private List<GameObject> _storyTitles;
    private float _storyTitlesListSize;
    private int _currentMenuPage;
    private int _currentPage;
    private int _storyTitlesPerPage;
    private int _menuPagesCount;
    private List<string> _currentTextPages;
    private bool _drawingModeInitialized = false;

    void Awake()
    {
        BetterStreamingAssets.Initialize();
        _stories = new List<Story>();
        _storyTitles = new List<GameObject>();
        _storyTitlesListSize = StoryLists[0].GetComponent<RectTransform>().rect.height;
        SetUpTextTitles();
        if (_storyTitles.Count <= _storyTitlesPerPage)
            MenuControlButtons[0].SetActive(false);
        else
            MenuControlButtons[0].SetActive(true);
        MenuControlButtons[1].SetActive(false);
        SetUpControlButtons();
        SetUpMenu();
        SwitchStates(ref ReadingModeContainer, ref MenuModeContainer);
        SwitchContentState(0, false);
        GameObject.Find("SceneIsReady").GetComponent<SceneIsReadyCheck>().IsReady = true;
    }

    private void SetUpControlButtons()
    {
        ReturnButton.GetComponent<Button>().onClick
            .AddListener(() => SwitchStates(ref ReadingModeContainer, ref MenuModeContainer));
        MenuControlButtons[0].GetComponent<Button>().onClick
            .AddListener(() => SwitchMenuPage(true));
        MenuControlButtons[1].GetComponent<Button>().onClick
            .AddListener(() => SwitchMenuPage(false));
        ControlButtons[0].GetComponent<Button>().onClick
            .AddListener(() => TurnPage(false));
        ControlButtons[1].GetComponent<Button>().onClick
            .AddListener(() => TurnPage(true));
        ModesControlButtons[0].GetComponent<Button>().onClick
            .AddListener(() => TransferDrawing(false));
        ModesControlButtons[1].GetComponent<Button>().onClick
            .AddListener(() => TransferDrawing(true));
        ModesControlButtons[2].GetComponent<Button>().onClick
            .AddListener(() => SwitchStates(ref CheckModeContainer, ref MenuModeContainer));
        ContentControlButtons[0].GetComponent<Button>().onClick
            .AddListener(() => SwitchContentState(0, false));
        ContentControlButtons[1].GetComponent<Button>().onClick
            .AddListener(() => SwitchContentState(1, true));
    }

    private void TransferDrawing(bool isCheck)
    {
        if (isCheck)
        {
            Drawing.transform.SetParent(DrawingContainer.transform, false);
            SwitchStates(ref DrawingModeContainer, ref CheckModeContainer);
            Drawing.SetActive(true);
        }
        else
        {
            Drawing.transform.SetParent(DrawingField.transform, false);
            SwitchStates(ref ReadingModeContainer, ref DrawingModeContainer);
            if (!_drawingModeInitialized)
            {
                DrawingField.GetComponent<DrawingManager>().InitializeFields();
                _drawingModeInitialized = true;
            }
            DrawingField.GetComponent<DrawingManager>().ClearDrawing();
            Drawing.SetActive(true);
        }
    }
    

    private void SwitchContentState(int buttonIndex, bool setContentActive)
    {
        Drawing.SetActive(!Convert.ToBoolean(buttonIndex));
        ContentControlButtons[buttonIndex].GetComponent<Image>().sprite =
            ContentControlButtonsSprites[1 + buttonIndex * 2];
        ContentControlButtons[Math.Abs(1 - buttonIndex)].GetComponent<Image>().sprite =
            ContentControlButtonsSprites[Math.Abs(1 - buttonIndex) + Math.Abs(buttonIndex - 1)];
        ImageContainer.SetActive(setContentActive);
    }

    private void SetUpTextTitles()
    {
        string storiesPath = "/database/retelling_texts/";
        string storyPath = "";
        _storyTitlesPerPage = StoryTitlesPerList * StoryLists.Length;
        int fileIndex = 1;
        while (true)
        {
            try
            {
                storyPath = $"{storiesPath}{fileIndex}.txt";
                string[] rawStory = BetterStreamingAssets.ReadAllLines(storyPath);
                _stories.Add(new Story(rawStory[0],
                    rawStory.Skip(2).Take(rawStory.Length - 2).ToArray(),
                    rawStory[1]));
                fileIndex++;
            }
            catch (Exception)
            {
                break;
            }
        }
        for (int i = 0; i < _stories.Count; i++)
        {
            GameObject storyTitlePlaceholder = Instantiate(StoryTitleContainerPrefab,
                StoryTextContainer.transform.position, Quaternion.identity);
            storyTitlePlaceholder.name = $"{_stories[i].Name}";
            storyTitlePlaceholder.GetComponentInChildren<Text>().text = storyTitlePlaceholder.name;
            storyTitlePlaceholder.transform.SetParent(StoryTitlesContainer.transform,
                false);
            storyTitlePlaceholder.GetComponent<Button>().onClick
                .AddListener(() => SetUpStory(storyTitlePlaceholder.name));
            _storyTitles.Add(storyTitlePlaceholder);
        }
    }

    private void CleanStoryLists()
    {
        List<GameObject> storiesToRemove = new List<GameObject>();
        storiesToRemove = _storyTitles.Skip(_currentMenuPage * _storyTitlesPerPage)
            .Take(_storyTitlesPerPage).ToList();
        for (int i = 0; i < storiesToRemove.Count; i++)
        {
            if (storiesToRemove[i].transform.position != StoryTitlesContainer.transform.position)
            {
                storiesToRemove[i].transform.SetParent(StoryTitlesContainer.transform, false);
                storiesToRemove[i].transform.position = StoryTitlesContainer.transform.position;
            }
        }
    }

    private void SetUpMenu()
    {
        int k = 0;
        bool listIsEmpty = false;
        List<GameObject> stories = _storyTitles.Skip(_currentMenuPage * _storyTitlesPerPage).Take(_storyTitlesPerPage)
            .ToList();
        for (int i = 0; i < StoryLists.Length; i++)
        {
            if (stories.Count - k < StoryTitlesPerList)
            {
                float size = (StoryTitleContainerPrefab.GetComponent<RectTransform>().rect.height + StoryTitleMargin) *
                             (_storyTitles.Count - k);
                StoryLists[i].GetComponent<RectTransform>()
                    .SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
                StoryLists[i].GetComponent<RectTransform>().anchoredPosition = MenuModeContainer.transform.position;
            }
            else
                StoryLists[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                    _storyTitlesListSize);

            for (int j = 0; j < StoryTitlesPerList; j++)
            {
                try
                {
                    stories[k].transform.SetParent(StoryLists[i].transform, false);
                    stories[k].transform.position = StoryLists[i].transform.position;
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

    private void SetUpStory(string storyName)
    {
        Story story = _stories.FirstOrDefault(s => s.Name == storyName);
        SplitTextIntoPages(story);
        SwitchStates(ref MenuModeContainer, ref ReadingModeContainer);
        ControlButtons[0].SetActive(false);
        if (_currentTextPages.Count > 1)
            ControlButtons[1].SetActive(true);
        else
            ControlButtons[1].SetActive(false);
        ShowText();
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

    private void SwitchStates(ref GameObject deactivate, ref GameObject activate)
    {
        deactivate.SetActive(false);
        activate.SetActive(true);
    }

    private void TurnPage(bool nextPage)
    {
        if (nextPage && _currentTextPages.Count - 1 > _currentPage)
        {
            ControlButtons[0].SetActive(true);
            _currentPage++;
            if (_currentTextPages.Count - _currentPage == 1)
                ControlButtons[1].SetActive(false);
        }

        if (!nextPage && _currentPage > 0)
        {
            ControlButtons[1].SetActive(true);
            _currentPage--;
            if (_currentPage == 0)
                ControlButtons[0].SetActive(false);
        }

        ShowText();
    }

    private void ShowText() => StoryTextContainer.GetComponent<Text>().text =
        _currentTextPages[_currentPage];

    private void ClearStoryPages()
    {
        _currentTextPages = new List<string>();
        _currentPage = 0;
    }

    private void SplitTextIntoPages(Story story)
    {
        ClearStoryPages();
        List<string> lines = story.Content.ToList();
        List<string> linesForPage = new List<string>();
        int numberOfOriginalLines;
        string transfer = "";
        while (lines.Count != 0)
        {
            numberOfOriginalLines = 0;
            linesForPage.RemoveRange(0, linesForPage.Count);
            linesForPage = new List<string>();
            while (linesForPage.Count != LinesOnPage)
            {
                if (numberOfOriginalLines >= lines.Count)
                    break;
                if (lines[numberOfOriginalLines].Length > SymbolsOnLine)
                {
                    List<char> lineText = lines[numberOfOriginalLines]
                        .Trim(' ').ToCharArray().ToList();
                    while (lineText.Count != 0)
                    {
                        string text1 = "";
                        transfer = "";
                        int i = SymbolsOnLine;
                        text1 = new string(lineText.Take(SymbolsOnLine).ToArray()).Trim();
                        if (lineText.Count > SymbolsOnLine)
                        {
                            if (Char.IsLetter(lineText[SymbolsOnLine])
                                || Char.IsPunctuation(lineText[SymbolsOnLine]))
                            {
                                while (Char.IsLetter(lineText[i - 1]) || Char.IsSurrogate(lineText[i - 1]))
                                    i--;
                                i--;
                                text1 = new string(lineText.Take(i + 1).ToArray());
                            }
                        }

                        linesForPage.Add(text1);
                        if (lineText.Count > SymbolsOnLine)
                            lineText.RemoveRange(0, i + 1);
                        else
                            lineText.RemoveRange(0, lineText.Count);
                        if (linesForPage.Count == LinesOnPage)
                        {
                            transfer = new string(lineText.ToArray());
                            break;
                        }
                    }

                    if (linesForPage.Count == LinesOnPage)
                        break;
                    numberOfOriginalLines++;
                }
                else
                {
                    linesForPage.Add(lines[numberOfOriginalLines].Trim(' '));
                    numberOfOriginalLines++;
                }
            }

            string text = "";
            for (int i = 0; i < linesForPage.Count; i++)
            {
                text += $"{linesForPage[i]}\n";
            }

            _currentTextPages.Add(text.Trim('\n'));
            lines.RemoveRange(0, numberOfOriginalLines);
            if (lines.Count != 0)
            {
                if (transfer != "")
                    lines[0] = transfer;
                if (lines[0].Contains(linesForPage[linesForPage.Count - 1]))
                    lines.RemoveAt(0);
            }
        }
    }
}