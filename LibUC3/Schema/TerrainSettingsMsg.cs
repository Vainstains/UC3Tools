using VProto;

namespace LibUC3.Schema;

public class TerrainSettingsMsg : Message
{
    [ProtoField(1)] public QuantizedFloatArrayMsg Heightmap;
    [ProtoField(2)] public int VerticesPerDirection;
    [ProtoField(3)] public float VertexSpacing;
    [ProtoField(5)] public byte[] TintmapBytes;
    [ProtoField(6)] public float WaterYLevel;
    [ProtoField(7)] public ColorMsg WaterColor;
    [ProtoField(8)] public SkyboxSettingsMsg SkyboxSettings;
}