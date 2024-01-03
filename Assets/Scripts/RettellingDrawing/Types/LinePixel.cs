using UnityEngine;

namespace RettellingDrawing.Types
{
    public class LinePixel
    {
        public Vector2 Position { get; set; }
        public Color Color { get; set; }

        public LinePixel(Vector2 position, Color color)
        {
            Position = position;
            Color = color;
        }
    }
}