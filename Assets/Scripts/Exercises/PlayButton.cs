using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    public GameObject LetterCageContainer;
    public GameObject LetterBoxContainer;
    void Awake()
    {
        Button letterButton = this.gameObject.GetComponent<Button>();
        letterButton.onClick.AddListener(() => ButtonClick());
    }

    // Update is called once per frame
    private void ButtonClick()
    {
        if (LetterCageContainer != null)
            LetterCageContainer.GetComponent<LetterCageManager>().StartCoroutine();
        else
            LetterBoxContainer.GetComponent<LetterBoxManager>().StartCoroutine();
    }
}
