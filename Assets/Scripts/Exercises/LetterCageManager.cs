using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class LetterCageManager : MonoBehaviour
{
    public int Padding;
    public GameObject LetterCagePrefab;
    public GameObject Curtain;
    public GameObject HandleSlide;
    public GameObject SlideButton;
    public GameObject NextWordButton;
    private List<GameObject> _letterCages;
    private List<string> _words;
    private Slider _curtainSlider;
    private string _generatedWord;

    private void GenerateWord()
    {
        Random random = new Random();
        _generatedWord = _words[random.Next(_words.Count - 1)].Trim();
    }
    private void Awake()
    {
        BetterStreamingAssets.Initialize();
        if (BetterStreamingAssets.FileExists("/database/words.txt"))
            _words = BetterStreamingAssets.ReadAllText("/database/words.txt").Split("\n").ToList();
        else
            Debug.Log("Could not find the file words.txt");
        SetUpScene();
        GameObject.Find("SceneIsReady").GetComponent<SceneIsReadyCheck>().IsReady = true;
    }

    private void SetUpScene()
    {
        GenerateWord();
        float newSize = _generatedWord.Length * (LetterCagePrefab.GetComponent<RectTransform>().rect.width + Padding);
        this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSize);
        Curtain.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSize);
        HandleSlide.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSize);
        _letterCages = new List<GameObject>();
        SlideButton.SetActive(true);
        NextWordButton.SetActive(false);
        for (int i = 0; i < _generatedWord.Length; i++)
        {
            GameObject letterCage = Instantiate(LetterCagePrefab, this.gameObject.transform.position, Quaternion.identity);
            letterCage.name = $"LetterCage ({i})";
            letterCage.transform.SetParent(this.gameObject.transform, false);
            letterCage.GetComponentInChildren<Text>().text = _generatedWord[i].ToString().ToUpper();
            _letterCages.Add(letterCage);
        }
        SetUpCurtain();
    }

    private void SetUpCurtain()
    {
        _curtainSlider = Curtain.GetComponent<Slider>();
        _curtainSlider.value = 1;
        _curtainSlider.handleRect.gameObject.SetActive(false);
        _curtainSlider.interactable = false;
    }

    public void StartCoroutine()
    {
        SlideButton.SetActive(false);
        StartCoroutine(SlideBack());
    }

    public void NextWord()
    {
        _words.Remove(_words.FirstOrDefault(w => w.Contains(_generatedWord)));
        for (int i = 0; i < _letterCages.Count; i++)
        {
            Destroy(_letterCages[i]);
        }
        SetUpScene();
    }
    private IEnumerator SlideBack()
    {
        while (_curtainSlider.value > 0)
        {
            _curtainSlider.value -= 0.01f;
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(2f);
        _curtainSlider.value = 1;
        _curtainSlider.interactable = true;
        _curtainSlider.handleRect.gameObject.SetActive(true);
        NextWordButton.SetActive(true);
    }
}
