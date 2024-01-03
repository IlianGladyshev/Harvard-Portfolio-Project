using UnityEngine;
using UnityEngine.UI;

public class Clue : MonoBehaviour
{
    public GameObject WordGuess { get; set; }
    public bool IsClicked { get; set; } = false;
    void Start()
    {
        Button clueButton = this.gameObject.GetComponent<Button>();
        WordGuess = GameObject.Find("LevelCanvas");
        clueButton.onClick.AddListener(() => ButtonClick());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ButtonClick()
    {
        if (IsClicked == false)
        {
            IsClicked = true;
            WordGuess.GetComponent<WordGuessManager>().HighlightLetters();
        }
    }
}
