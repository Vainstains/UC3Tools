using VProto;

namespace LibUC3.Schema;

public class CoasterPartGroupSettingsMsg : Message
{
    [ProtoField(1)] public CoasterPartSettingsMsg Part;
    [ProtoField(2)] public string Name;
}