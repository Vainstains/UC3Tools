using VProto;

namespace LibUC3.Schema;

public class CoasterWorldSaveFileMsg : Message
{
    [ProtoField(1)] public CoasterWorldSettingsMsg World;
    [ProtoField(2)] public byte[] PreviewImage;
    [ProtoField(3)] public string EditPasswordHash;
}