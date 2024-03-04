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


namespace XR2Learn.DataCollection.Data
{
    public interface IData
    {
        public bool IsEmpty();

        public struct Headers
        {
            public const string VR =
                "timestamp," +
                "head_posX," +
                "head_posY," +
                "head_posZ," +
                "head_rotX," +
                "head_rotY," +
                "head_rotZ," +
                "head_rotW," +
                "lcontroller_posX," +
                "lcontroller_pposY," +
                "lcontroller_posZ," +
                "lcontroller_rotX," +
                "lcontroller_rotY," +
                "lcontroller_rotZ," +
                "lcontroller_rotW," +
                "rcontroller_posX," +
                "rcontroller_pposY," +
                "rcontroller_posZ," +
                "rcontroller_rotX," +
                "rcontroller_rotY," +
                "rcontroller_rotZ," +
                "rcontroller_rotW";

            public const string EVENTS =
                "timestamp," +
                "event_type," +
                "info";

            public const string LIP =
                "timestamp," +
                "int_timestamp," +
                "none," +
                "jaw_forward," +
                "jaw_right," +
                "jaw_left," +
                "jaw_open," +
                "mouth_ape_shape," +
                "mouth_o_shape," +
                "mouth_pout," +
                "mouth_lower_right," +
                "mouth_lower_left," +
                "mouth_smile_right," +
                "mouth_smile_left," +
                "mouth_sad_right," +
                "mouth_sad_left," +
                "cheek_puff_right," +
                "cheek_puff_left," +
                "mouth_lower_inside," +
                "mouth_upper_inside," +
                "mouth_lower_overlay," +
                "mouth_upper_overlay," +
                "cheek_suck," +
                "mouth_lower_right_down," +
                "mouth_lower_left_down," +
                "mouth_upper_right_up," +
                "mouth_upper_left_up," +
                "mouth_philtrum_right," +
                "mouth_philtrum_left";

            public const string EYE =
                "timestamp," +
                "int_timestamp," +
                "left_gaze_origin_x," +
                "left_gaze_origin_y," +
                "left_gaze_origin_z," +
                "left_gaze_dir_norm_x," +
                "left_gaze_dir_norm_y," +
                "left_gaze_dir_norm_z," +
                "left_pupil_diameter," +
                "left_eye_openness," +
                "left_pos_norm_x," +
                "left_pos_norm_y," +
                "right_gaze_origin_x," +
                "right_gaze_origin_y," +
                "right_gaze_origin_z," +
                "right_gaze_dir_norm_x," +
                "right_gaze_dir_norm_y," +
                "right_gaze_dir_norm_z," +
                "right_pupil_diameter," +
                "right_eye_openness," +
                "right_pos_norm_x," +
                "right_pos_norm_y";

            public const string SHIMMER =
                "timestamp," +
                "int_timestamp," +
                "accel_x," +
                "accel_y," +
                "accel_z," +
                "gsr," +
                "ppg," +
                "hr";
        }

        

        
    }
}