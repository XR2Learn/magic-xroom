
namespace DataCollection.Data
{ 
    public class EyeData
    {
        public const string HEADER =
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

        private readonly long _timestamp;
        private readonly ViveSR.anipal.Eye.EyeData _eyes;

        public EyeData(long timestamp, ViveSR.anipal.Eye.EyeData eyes)
        {
            _timestamp = timestamp;
            _eyes = eyes;
        }

        public override string ToString()
        {
            return _timestamp + "," +
                    _eyes.timestamp + "," +
                    _eyes.verbose_data.left.gaze_origin_mm.x + "," +
                    _eyes.verbose_data.left.gaze_origin_mm.y + "," +
                    _eyes.verbose_data.left.gaze_origin_mm.z + "," +
                    _eyes.verbose_data.left.gaze_direction_normalized.x + "," +
                    _eyes.verbose_data.left.gaze_direction_normalized.y + "," +
                    _eyes.verbose_data.left.gaze_direction_normalized.z + "," +
                    _eyes.verbose_data.left.pupil_diameter_mm + "," +
                    _eyes.verbose_data.left.eye_openness + "," +
                    _eyes.verbose_data.left.pupil_position_in_sensor_area.x + "," +
                    _eyes.verbose_data.left.pupil_position_in_sensor_area.y + "," +
                    _eyes.verbose_data.right.gaze_origin_mm.x + "," +
                    _eyes.verbose_data.right.gaze_origin_mm.y + "," +
                    _eyes.verbose_data.right.gaze_origin_mm.z + "," +
                    _eyes.verbose_data.right.gaze_direction_normalized.x + "," +
                    _eyes.verbose_data.right.gaze_direction_normalized.y + "," +
                    _eyes.verbose_data.right.gaze_direction_normalized.z + "," +
                    _eyes.verbose_data.right.pupil_diameter_mm + "," +
                    _eyes.verbose_data.right.eye_openness + "," +
                    _eyes.verbose_data.right.pupil_position_in_sensor_area.x + "," +
                    _eyes.verbose_data.right.pupil_position_in_sensor_area.y;
        }
    }
}
