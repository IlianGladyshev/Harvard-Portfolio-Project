using System;
using System.Collections.Generic;
using System.Linq;
using RettellingDrawing.Types;
using UnityEngine;
using UnityEngine.UI;

public class DrawingManager : MonoBehaviour
{
    public Stack<Line> LineStack;
    public GameObject Drawing;
    public Sprite ImageTexture;
    public GameObject ImageGameObject;
    public Button SaveImageButton;
    public List<GameObject> ColorButtons;
    public GameObject ReturnButton;
    public Camera Camera;
    public Line LinePrefab;
    public const float Resolution = 0.05f;
    private Line _previousLine;
    private List<LinePixel> _linePixels;
    private string _currentColor;
    private Line _currentLine;

    public void InitializeFields()
    {
        LineStack = new Stack<Line>();
        _linePixels = new List<LinePixel>();
        for (int i = 0; i < ColorButtons.Count; i++)
            AddButtonListener(ColorButtons[i].GetComponent<Button>());
        ReturnButton.GetComponent<Button>().onClick.AddListener(() => Return());
    }

    public void ClearDrawing()
    {
        List<Line> lines = LineStack.ToList();
        if (lines.Count != 0)
            for (int i = 0; i < lines.Count; i++)
                Destroy(lines[i].gameObject);
        LineStack = new Stack<Line>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Camera.ScreenToWorldPoint(Input.mousePosition);
        if (Physics2D.OverlapPoint(mousePosition))
        {
            if (Input.GetMouseButtonDown(0))
            {
                _currentLine = Instantiate(LinePrefab, mousePosition, Quaternion.identity);
                _currentLine.gameObject.transform.SetParent(Drawing.transform, false);
                _currentLine.ChangeColor(_currentColor);
                LineStack.Push(_currentLine);
                _currentLine.Dispose();
            }
            try
            {
                if (Input.GetMouseButton(0))
                {
                    LinePixel linePixel = _currentLine.SetPosition(mousePosition);
                    if (linePixel != null)
                        _linePixels.Add(linePixel);
                }
                else
                {
                    if (_currentLine.LineRenderer.positionCount >= 50)
                        _currentLine.LineRenderer.Simplify(0.027f);
                    else
                        _currentLine.LineRenderer.Simplify(0.05f);
                }
            }
            catch (Exception) { }
        }

        /*Debug.Log(_linePixels.Count);*/
    }

    private void SaveImage()
    {
        for (int i = 0; i < _linePixels.Count; i++)
        {
            Vector2 coordinates = WorldToPixelCoordinates(_linePixels[i].Position);
            ImageTexture.texture.SetPixel(Mathf.RoundToInt(coordinates.x), Mathf.RoundToInt(coordinates.y), _linePixels[i].Color);
        }
        ImageTexture.texture.Apply();
    }
    
    /*private void SaveImage()
    {
        System.Random random = new System.Random();
        for (int i = 0; i < 200; i++)
        {
            /*Debug.Log($"Coordinates = {Convert.ToInt32(_linePixels[i].Position.x) * ImageTexture.texture.width / Screen.width} {_linePixels[i].Position.y * ImageTexture.texture.height / Screen.height}, Color = {_linePixels[0].Color}");
            ImageTexture.texture.SetPixel(
                Convert.ToInt32(Convert.ToInt32(_linePixels[i].Position.x) * ImageTexture.texture.width / Screen.width),
                Convert.ToInt32(_linePixels[i].Position.y * ImageTexture.texture.height / Screen.height),
                _linePixels[i].Color);#1#
            int x = random.Next(0, 8192);
            int y = random.Next(0, 8192);
            ImageTexture.texture.SetPixel(x, y, Color.black);
        }
        ImageTexture.texture.Apply();
    }*/
    
    private void Return()
    {
        if (LineStack.Count != 0)
        {
            LineStack.Peek().DestroyLine();
            LineStack.Pop();
        }
    }

    private void AddButtonListener(Button button) =>
        button.onClick.AddListener(() => _currentColor = button.name.ToLower());
    
    private Vector2 WorldToPixelCoordinates(Vector2 position)
    {
        const int mouseOffset = 191;
        const int imageResolution = 8192;
        Vector3 localPosition = ImageGameObject.transform.InverseTransformPoint(position);
        float imageWidth = ImageGameObject.GetComponent<RectTransform>().rect.width;
        float imageHeight = ImageGameObject.GetComponent<RectTransform>().rect.height;
        float pixelWidth = ImageGameObject.GetComponent<Image>().sprite.texture.width;
        float pixelHeight = ImageGameObject.GetComponent<Image>().sprite.texture.height;
        float coefficientX = imageWidth / pixelWidth;
        float coefficientY = imageHeight / pixelHeight;
        float centeredX = ((localPosition.x * coefficientX + mouseOffset) + (imageWidth / 2)) * imageResolution / imageWidth;
        float centeredY = ((localPosition.y * coefficientY + mouseOffset) + (imageHeight / 2)) * imageResolution / imageHeight;
        return new Vector2(centeredX, centeredY);
    }
    
}
