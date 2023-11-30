

using System.Text;

public class LipData
{
    public const string HEADER =
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

    private const string COMMA = ",";

    private readonly long _timestamp;
    private readonly ViveSR.anipal.Lip.LipData _lips;

    public LipData(long timestamp, ViveSR.anipal.Lip.LipData lips)
    {
        _timestamp = timestamp;
        _lips = lips;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(_timestamp);
        sb.Append(COMMA);
        sb.Append(_lips.time);
        sb.Append(COMMA);
        for (int i = 0; i < (int)ViveSR.anipal.Lip.LipShape.Max - 1; i++)
        {
            sb.Append(_lips.prediction_data.blend_shape_weight[i]);
            sb.Append(COMMA);
        }
        sb.Append(_lips.prediction_data.blend_shape_weight[(int)ViveSR.anipal.Lip.LipShape.Max - 1]);
        return sb.ToString();
    }
}
