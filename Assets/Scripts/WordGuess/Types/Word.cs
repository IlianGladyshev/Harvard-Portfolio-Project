using UnityEngine;

namespace WordGuess.Types
{
    public class Word
    {
        public string Name { get; set; }
        public Sprite Sprite { get; set; }

        public Word(string name, Sprite sprite)
        {
            Name = name;
            Sprite = sprite;
        }
    }
}