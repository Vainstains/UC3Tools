using VProto;

namespace LibUC3.Schema;

public class NurbsSequenceSettingsMsg : Message
{
    [ProtoField(1)] public List<NurbsSequenceControlPointSettings> ControlPoint;
}