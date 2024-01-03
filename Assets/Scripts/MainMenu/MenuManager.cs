using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject FirstButtonListPlaceholder;
    public GameObject SecondButtonListPlaceholder;
    public GameObject ExerciseButton;
    public GameObject ReturnButton;
    // Start is called before the first frame update
    private void Awake()
    {
        SetUpControlButtons();
        GameObject.Find("SceneIsReady").GetComponent<SceneIsReadyCheck>().IsReady = true;
    }

    private void SetUpControlButtons()
    {
        ExerciseButton.GetComponent<Button>().onClick
            .AddListener(() => ChangeStates(ref FirstButtonListPlaceholder, ref SecondButtonListPlaceholder));
        ReturnButton.GetComponent<Button>().onClick
            .AddListener(() => ChangeStates(ref SecondButtonListPlaceholder, ref FirstButtonListPlaceholder));
    }

    private void ChangeStates(ref GameObject disactivate, ref GameObject activate)
    {
        disactivate.transform.position = new Vector3(-3000, -3000, disactivate.transform.position.z);
        activate.transform.position = new Vector3(0, 0, activate.transform.position.z);
    }
}
