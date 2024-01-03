using System.Collections;
using RettellingDrawing.Types;
using UnityEngine;

public class Line : MonoBehaviour
{
    public GameObject LineGameObject;
    public LineRenderer LineRenderer;
    public Material PenColor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Dispose() => StartCoroutine(DisposeCoroutine());
    public void DestroyLine() => Destroy(this.gameObject);

    public void ChangeColor(string color)
    {
        switch (color)
        {
            case "red":
                PenColor.color = Color.red;
                break;
            case "blue":
                PenColor.color = Color.blue;
                break;
            case "orange":
                PenColor.color = new Color(1f, 0.54f, 0.29f, 1f);
                break;
            case "yellow":
                PenColor.color = Color.yellow;
                break;
            case "green":
                PenColor.color = Color.green;
                break;
            case "purple":
                PenColor.color = new Color(0.5686f, 0.133f, 0.87843f, 1f);
                break;
            case "brown":
                PenColor.color = new Color(0.51764f, 0.28627f, 0.28627f, 255);
                break;
            case "black":
                PenColor.color = Color.black;
                break;
            case "rubber":
                PenColor.color = Color.white;
                break;
        }

        LineRenderer.startColor = PenColor.color;
        LineRenderer.endColor = PenColor.color;
    }

    public LinePixel SetPosition(Vector2 position)
    {
        if (!CanAppend(position))
            return null;
        LineRenderer.positionCount++;
        LineRenderer.SetPosition(LineRenderer.positionCount - 1, position);
        return new LinePixel(position, LineRenderer.startColor);
    }

    private bool CanAppend(Vector2 position)
    {
        if (LineRenderer.positionCount == 0)
            return true;
        return Vector2.Distance(LineRenderer
                   .GetPosition(LineRenderer.positionCount - 1), position) >
               DrawingManager.Resolution;
    }

    private IEnumerator DisposeCoroutine()
    {
        yield return new WaitForSeconds(0.7f);
        if (LineRenderer.positionCount <= 1)
            Destroy(this.gameObject);
    }
}
