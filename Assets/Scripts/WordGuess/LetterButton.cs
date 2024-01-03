using UnityEngine;
using UnityEngine.UI;

public class LetterButton : MonoBehaviour
{
    public GameObject WordGuess { get; set; }
    public bool IsHighlighted { get; set; } = false;
    public bool IsClicked { get; set; }
    public Sprite NormalSprite;
    public Sprite HighlightedSprite;

    private void Start()
    {
        Button letterButton = this.gameObject.GetComponent<Button>();
        letterButton.onClick.AddListener(() => ButtonClick());
    }

    private void ButtonClick()
    {
        if (this.gameObject.transform.GetChild(0).gameObject.activeSelf && this.gameObject.GetComponentInChildren<Text>().text != "")
            WordGuess.GetComponent<WordGuessManager>().DisposeLetterButton(this.gameObject);
    }
}
