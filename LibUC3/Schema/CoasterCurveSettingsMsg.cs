using VProto;

namespace LibUC3.Schema;

public class CoasterCurveSettingsMsg : Message
{
    public class PrefabSettingsMsg : Message
    {
        public class AnchorMsg : Message
        {
            [ProtoField(1)] public Vec3Msg EndPosition;
            [ProtoField(2)] public Vec3Msg PenultimatePosition;
        }

        [ProtoField(1)] public string PrefabTypeId;
        [ProtoField(2)] public AnchorMsg StartAnchor;
        [ProtoField(3)] public AnchorMsg EndAnchor;
        [ProtoField(4)] public List<float> FloatVariables;
        [ProtoField(5)] public List<bool> BoolVariables;
    }

    public class ArcSettingsMsg : Message
    {
        public class ClassicSettingsMsg : Message
        {
            [ProtoField(1)] public float Length;
            [ProtoField(2)] public float Pitch;
            [ProtoField(3)] public float Yaw;
        }

        public class EndpointSettingsMsg : Message
        {
            [ProtoField(1)] public Vec3Msg EndPosition;
        }

        public enum Type
        {
            Classic = 0,
            Endpoint = 1
        }

        [ProtoField(1)] public Vec3Msg StartPosition;
        [ProtoField(2)] public bool ForwardDirection;
        [ProtoField(3)] public Vec3Msg StartDirection;
        [ProtoField(4)] public ClassicSettingsMsg Classic;
        [ProtoField(5)] public EndpointSettingsMsg Endpoint;
        [ProtoField(6)] public Type ArcType;
    }
    
    [ProtoField(1)] public NurbsSequenceSettingsMsg NurbsSequence;
    [ProtoField(2)] public List<BankNodeSettingsMsg> BankNodes;
    [ProtoField(6)] public List<SegmentNodeSettingsMsg> SegmentNodes;
    [ProtoField(3)] public List<SupportNodeSettingsMsg> SupportNodes;
    [ProtoField(4)] public string ShapeTypeId;
    [ProtoField(5)] public PrefabSettingsMsg PrefabSettings;
    [ProtoField(7)] public ArcSettingsMsg ArcSettings;
}