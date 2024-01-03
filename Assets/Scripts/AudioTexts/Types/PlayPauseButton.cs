using UnityEngine;
using UnityEngine.UI;

namespace AudioTexts.Types
{
    public class PlayPauseButton : MonoBehaviour
    {
        public Sprite Play;
        public Sprite Pause;


        public void ChangeSprite(bool play)
        {
            if (play)
                gameObject.GetComponent<Image>().sprite = Play;
            else
                gameObject.GetComponent<Image>().sprite = Pause;
        }
    }
}