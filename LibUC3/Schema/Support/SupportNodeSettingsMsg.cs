using VProto;

namespace LibUC3.Schema;

public class SupportNodeSettingsMsg : Message
{
    [ProtoField(1)] public float ControlPointIndex;
    [ProtoField(2)] public SupportVisualSettingsMsg SupportVisualSettings;
}