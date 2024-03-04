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
    public class EyeData
    {
        private readonly long _timestamp;
        private readonly ViveSR.anipal.Eye.EyeData _eyes;

        public EyeData(long timestamp, ViveSR.anipal.Eye.EyeData eyes)
        {
            _timestamp = timestamp;
            _eyes = eyes;
        }

        public override string ToString()
        {
            string comma = ",";

            StringBuilder sb = new StringBuilder();
            sb.Append(_timestamp);
            sb.Append(comma);
            sb.Append(_eyes.timestamp);
            sb.Append(comma);
            sb.Append(_eyes.verbose_data.left.gaze_origin_mm.x);
            sb.Append(comma);
            sb.Append(_eyes.verbose_data.left.gaze_origin_mm.y);
            sb.Append(comma);
            sb.Append(_eyes.verbose_data.left.gaze_origin_mm.z);
            sb.Append(comma);
            sb.Append(_eyes.verbose_data.left.gaze_direction_normalized.x);
            sb.Append(comma);
            sb.Append(_eyes.verbose_data.left.gaze_direction_normalized.y);
            sb.Append(comma);
            sb.Append(_eyes.verbose_data.left.gaze_direction_normalized.z);
            sb.Append(comma);
            sb.Append(_eyes.verbose_data.left.pupil_diameter_mm);
            sb.Append(comma);
            sb.Append(_eyes.verbose_data.left.eye_openness);
            sb.Append(comma);
            sb.Append(_eyes.verbose_data.left.pupil_position_in_sensor_area.x);
            sb.Append(comma);
            sb.Append(_eyes.verbose_data.left.pupil_position_in_sensor_area.y);
            sb.Append(comma);
            sb.Append(_eyes.verbose_data.right.gaze_origin_mm.x);
            sb.Append(comma);
            sb.Append(_eyes.verbose_data.right.gaze_origin_mm.y);
            sb.Append(comma);
            sb.Append(_eyes.verbose_data.right.gaze_origin_mm.z);
            sb.Append(comma);
            sb.Append(_eyes.verbose_data.right.gaze_direction_normalized.x);
            sb.Append(comma);
            sb.Append(_eyes.verbose_data.right.gaze_direction_normalized.y);
            sb.Append(comma);
            sb.Append(_eyes.verbose_data.right.gaze_direction_normalized.z);
            sb.Append(comma);
            sb.Append(_eyes.verbose_data.right.pupil_diameter_mm);
            sb.Append(comma);
            sb.Append(_eyes.verbose_data.right.eye_openness);
            sb.Append(comma);
            sb.Append(_eyes.verbose_data.right.pupil_position_in_sensor_area.x);
            sb.Append(comma);
            sb.Append(_eyes.verbose_data.right.pupil_position_in_sensor_area.y);

            return sb.ToString();
        }
    }
}
