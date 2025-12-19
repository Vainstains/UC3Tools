using OpenTK.Mathematics;
using VProto;

namespace LibUC3.Schema;

public class NurbsSequenceSettingsMsg : Message
{
    [ProtoField(1)] public List<NurbsSequenceControlPointSettings> ControlPoint;
}

public static class SequenceExtensions
{
    public static void AddPoint(this List<NurbsSequenceControlPointSettings> seq,
        Vector3 pos, float weight = 1.0f, bool strict = false)
    {
        seq.Add(new()
        {
            Position = pos,
            Weight = weight,
            Strict = strict
        });
    }
}