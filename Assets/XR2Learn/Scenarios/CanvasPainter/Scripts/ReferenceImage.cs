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


using UnityEngine;

namespace XR2Learn.Scenarios.CanvasPainter
{
    [CreateAssetMenu(fileName = "ReferenceImage", menuName = "ScriptableObjects/CanvasPainter/Reference Image", order = 1)]
    public class ReferenceImage : ScriptableObject
    {
        [SerializeField] private Texture2D image;
        [SerializeField] private int brushSize;
        [SerializeField] private int timeLimit;
        [SerializeField] private int foulLimit;
        [SerializeField] private int coverageTarget;

        public Texture2D Image => image;
        public int BrushSize => brushSize;
        public int TimeLimit => timeLimit;
        public int FoulLimit => foulLimit;
        public int CoverageTarget => coverageTarget;
    }
}