using VProto;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
namespace LibUC3.Schema;

public class CoasterWorldSettingsMsg : Message
{
    [ProtoField(1)] public List<CoasterSettingsMsg> Coasters;
    [ProtoField(2)] public TerrainSettingsMsg Terrain;
    [ProtoField(3)] public SceneryStateSettingsMsg SceneryState;
    [ProtoField(4)] public Vec3Msg InitialCameraPos;
    [ProtoField(5)] public float InitialCameraYaw;
    [ProtoField(6)] public float InitialCameraPitch;
}