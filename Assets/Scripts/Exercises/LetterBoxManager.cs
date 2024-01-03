using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class LetterBoxManager : MonoBehaviour
{
    public float LetterCageMargin = 50f;
    public GameObject LetterBoxPrefab;
    public GameObject OpenButton;
    public GameObject NextWordButton;
    public Sprite OpenBoxSprite;
    public Sprite ClosedBoxSprite;
    public int CurrentOpenButton;
    private List<GameObject> _letterBoxes;
    private List<string> _words;
    private string _generatedWord;
    
    // Start is called before the first frame update
    
    private void GenerateWord()
    {
        Random random = new Random();
        _generatedWord = _words[random.Next(_words.Count - 1)].Trim();
    }
    private void Awake()
    {
        BetterStreamingAssets.Initialize();
        _words = BetterStreamingAssets.ReadAllText("/database/words.txt").Split("\n").ToList();
        SetUpScene();
        GameObject.Find("SceneIsReady").GetComponent<SceneIsReadyCheck>().IsReady = true;
    }

    private void SetUpScene()
    {
        GenerateWord(); 
        SetSize(this.gameObject);
        _letterBoxes = new List<GameObject>();
        OpenButton.SetActive(true);
        NextWordButton.SetActive(false);
        for (int i = 0; i < _generatedWord.Length; i++)
        {
            GameObject letterBox = Instantiate(LetterBoxPrefab, this.gameObject.transform.position, Quaternion.identity);
            letterBox.name = $"{i}";
            letterBox.transform.SetParent(this.gameObject.transform, false);
            letterBox.GetComponentInChildren<Text>().text = "";
            letterBox.GetComponent<Image>().sprite = ClosedBoxSprite;
            letterBox.GetComponent<Button>().enabled = false;
            letterBox.GetComponent<Button>().onClick.AddListener(() => BoxClick(letterBox));
            _letterBoxes.Add(letterBox);
        }
    }
    

    private void SetSize(GameObject gameObject)
    {
        gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 
            LetterBoxPrefab.GetComponent<RectTransform>().rect.width * _generatedWord.Length +
            LetterCageMargin * (_generatedWord.Length - 1));
        gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 
            LetterBoxPrefab.GetComponent<RectTransform>().rect.height);
    }

    public void StartCoroutine()
    {
        OpenButton.SetActive(false);
        StartCoroutine(OpenBoxes());
    }

    public void NextWord()
    {
        _words.Remove(_words.FirstOrDefault(w => w.Contains(_generatedWord)));
        for (int i = 0; i < _letterBoxes.Count; i++)
        {
            Destroy(_letterBoxes[i]);
        }
        SetUpScene();
    }

    public void BoxClick(GameObject letterBox)
    {
        if (letterBox.name == CurrentOpenButton.ToString())
        {
            letterBox.GetComponent<Image>().sprite = OpenBoxSprite;
            letterBox.GetComponentInChildren<Text>().text = _generatedWord[CurrentOpenButton].ToString().ToUpper();
            letterBox.GetComponent<Button>().enabled = false;
            CurrentOpenButton++;
        }
    }
    private IEnumerator OpenBoxes()
    {
        for (int i = 0; i < _generatedWord.Length; i++)
        {
            yield return new WaitForSeconds(1f);
            _letterBoxes[i].GetComponentInChildren<Text>().text = _generatedWord[i].ToString().ToUpper();
            _letterBoxes[i].GetComponent<Image>().sprite = OpenBoxSprite;
        }
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < _generatedWord.Length; i++)
        {
            _letterBoxes[i].GetComponent<Button>().enabled = true;
            _letterBoxes[i].GetComponentInChildren<Text>().text = "";
            _letterBoxes[i].GetComponent<Image>().sprite = ClosedBoxSprite;
        }
        CurrentOpenButton = 0;
        NextWordButton.SetActive(true);
    }
}
