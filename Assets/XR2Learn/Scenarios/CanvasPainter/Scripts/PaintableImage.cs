/*********** Copyright © 2024 University of Applied Sciences of Southern Switzerland (SUPSI) ***********\
 
 Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
 associated documentation files (the "Software"), to deal in the Software without restriction,
 including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 subject to the following conditions:

 The above copyright notice and this permission notice shall be included in all copies or substantial
 portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT
 LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

\*******************************************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XR2Learn.Scenarios.CanvasPainter
{
    public class PaintableImage : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerMoveHandler
    {
        [SerializeField] private RawImage reference;
        [SerializeField] private RawImage target;
        [SerializeField] private int brushSize;

        [SerializeField] private Texture2D referenceTexture;
        private Texture2D _targetTexture;

        private bool[,] _referencePixels;
        private bool[,] _targetPixels;

        private bool _isActive;
        private bool _isDrawing;

        private Vector2 _textureScaleFactor;
        private Vector2 _lastPosition;

        private int _pixelsToCover;
        private int _missCount;
        private float _coverage;
        private float _overflow;

        public int MissCount => _missCount;
        public float Coverage => _coverage;
        public float Overflow => _overflow;

        public int BrushSize
        {
            get => brushSize;
            set => brushSize = value;
        }
        

        public void StartGame()
        {
            Vector2 transformSize = ((RectTransform)transform).rect.size;   
            _textureScaleFactor = new Vector2(referenceTexture.width / transformSize.x,
                referenceTexture.height / transformSize.y);
            _targetTexture =
                new Texture2D(referenceTexture.width, referenceTexture.height, TextureFormat.RGBA32, false);
            target.texture = _targetTexture;
            target.color = Color.white;
            PrepareArrays();
            WriteTargetTexture();

            _pixelsToCover = _referencePixels.Cast<bool>().Count(p => p);

            _isActive = true;
        }

        public void StopGame()
        {
            if (!_isActive) return;
            _isActive = false;
        }

        public void Cleanup()
        {
            if (_isActive) return;
            if(target.texture)
                Destroy(target.texture);
            target.texture = null;
            target.color = new Color(0, 0, 0, 0);
            _coverage = 0;
            _overflow = 0;
            _missCount = 0;
        }

        public void SetImage(ReferenceImage image)
        {
            referenceTexture = image.Image;
            reference.texture = referenceTexture;
            brushSize = image.BrushSize;
        }

        private void PrepareArrays()
        {
            _referencePixels = new bool[referenceTexture.height, referenceTexture.width];
            _targetPixels = new bool[referenceTexture.height, referenceTexture.width];

            bool[] t = referenceTexture.GetPixels(0).Select(c => c.grayscale).Select(v => v < .5f).ToArray();
            for (int r = 0; r < referenceTexture.height; r++)
            {
                for (int c = 0; c < referenceTexture.width; c++)
                {
                    _referencePixels[r, c] = t[c + r * referenceTexture.height];
                    _targetPixels[r, c] = false;
                }
            }
        }

        private void WriteTargetTexture()
        {
            Color[] targetColor = new Color[referenceTexture.width * referenceTexture.height];
            for (int r = 0; r < referenceTexture.height; r++)
            {
                int i0 = r * referenceTexture.width;
                for (int c = 0; c < referenceTexture.width; c++)
                {
                    int i1 = c + i0;
                    if (!_targetPixels[r, c]) targetColor[i1] = new Color(0, 0, 0, 0);
                    else
                        targetColor[i1] =
                            _referencePixels[r, c] && _targetPixels[r, c] ? Color.green : Color.red;
                }
            }

            _targetTexture.SetPixels(targetColor);
            _targetTexture.Apply(false);
        }

        private void DoBrushStroke(Vector2 from, Vector2 to, bool isMiss)
        {
            int fillCount = Mathf.FloorToInt(Vector2.Distance(from, to) / brushSize);

            IEnumerable<Vector2> pointsOnLine = SplitLine(from, to, fillCount);

            foreach (Vector2 point in pointsOnLine)
                PaintTargetArray(Mathf.RoundToInt(point.y), Mathf.RoundToInt(point.x), brushSize, isMiss);
        }

        private void PaintTargetArray(int row, int column, int radius, bool isMiss)
        {
            int rowMin = Math.Max(row - radius, 0);
            int rowMax = Math.Min(row + radius + 1, _targetPixels.GetLength(0));
            int colMin = Math.Max(column - radius, 0);
            int colMax = Math.Min(column + radius + 1, _targetPixels.GetLength(1));

            int limit = radius * radius;

            for (int r = rowMin; r < rowMax; r++)
            for (int c = colMin; c < colMax; c++)
            {
                int distance = (row - r) * (row - r) + (column - c) * (column - c);
                if (distance < limit && (_referencePixels[r, c] || isMiss))
                    _targetPixels[r, c] = true;
            }
        }

        private static IEnumerable<Vector2> SplitLine(Vector2 a, Vector2 b, int count)
        {
            count += 1;

            double d = Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y)) / count;
            double fi = Math.Atan2(b.y - a.y, b.x - a.x);

            List<Vector2> points = new List<Vector2>(count + 1);

            for (int i = 0; i <= count; ++i)
                points.Add(new Vector2((float)(a.x + i * d * Math.Cos(fi)), (float)(a.y + i * d * Math.Sin(fi))));

            return points;
        }

        private bool IsMiss(Vector2 lastPosition, Vector2 position)
        {
            return IsOutOfBounds(lastPosition) || IsOutOfBounds(position);
        }

        private bool IsOutOfBounds(Vector2 position)
        {
            return !_referencePixels[Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.x)];
        }

        private void CalculateScore()
        {
            int rows = _referencePixels.GetLength(0);
            int cols = _referencePixels.GetLength(1);

            int hits = 0;
            int overflows = 0;

            for (int r = rows - 1; r >= 0; r--)
            {
                for (int c = cols - 1; c >= 0; c--)
                {
                    if (!_targetPixels[r, c]) continue;
                    if (_referencePixels[r, c] & _targetPixels[r, c]) hits++;
                    if (_referencePixels[r, c] ^ _targetPixels[r, c]) overflows++;
                }
            }

            _coverage = 1.0f * hits / _pixelsToCover;
            _overflow = 1.0f * overflows / _pixelsToCover;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isActive) return;
            _isDrawing = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isActive) return;
            _lastPosition = new Vector2(
                eventData.position.x * _textureScaleFactor.x,
                eventData.position.y * _textureScaleFactor.y
            );
            _isDrawing = true;
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (!_isDrawing || !_isActive) return;

            Vector2 position = new Vector2(
                eventData.position.x * _textureScaleFactor.x,
                eventData.position.y * _textureScaleFactor.y
            );

            bool isMiss = IsMiss(_lastPosition, _lastPosition);

            DoBrushStroke(_lastPosition, position, isMiss);
            WriteTargetTexture();
            _lastPosition = position;

            CalculateScore();

            if (!isMiss) return;
            _missCount++;
            _isDrawing = false;
        }
    }
}