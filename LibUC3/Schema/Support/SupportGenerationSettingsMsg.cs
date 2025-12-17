using VProto;

namespace LibUC3.Schema;

public class SupportGenerationSettingsMsg : Message
{
    [ProtoField(1)] public SupportVisualSettingsMsg SupportVisualSettings;
    [ProtoField(2)] public float Spacing;
}