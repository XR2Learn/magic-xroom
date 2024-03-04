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


using System.Text;

namespace XR2Learn.DataCollection.Data
{
    public class LipData
    {

        private readonly long _timestamp;
        private readonly ViveSR.anipal.Lip.LipData _lips;

        public LipData(long timestamp, ViveSR.anipal.Lip.LipData lips)
        {
            _timestamp = timestamp;
            _lips = lips;
        }

        public override string ToString()
        {
            string comma = ",";

            StringBuilder sb = new StringBuilder();
            sb.Append(_timestamp);
            sb.Append(comma);
            sb.Append(_lips.time);
            sb.Append(comma);
            for (int i = 0; i < (int)ViveSR.anipal.Lip.LipShape.Max - 1; i++)
            {
                sb.Append(_lips.prediction_data.blend_shape_weight[i]);
                sb.Append(comma);
            }
            sb.Append(_lips.prediction_data.blend_shape_weight[(int)ViveSR.anipal.Lip.LipShape.Max - 1]);

            return sb.ToString();
        }
    }
}
