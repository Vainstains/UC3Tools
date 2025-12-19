using VProto;

namespace LibUC3.Schema;

public class SceneryColorChannelSettingsMsg : Message
{
    [ProtoField(1)] public int ChannelIndex;
    [ProtoField(2)] public CoasterTextureArrayTextureEnum Texture;
    [ProtoField(3)] public ColorMsg Color;
    [ProtoField(4)] public float TextureScale;
}