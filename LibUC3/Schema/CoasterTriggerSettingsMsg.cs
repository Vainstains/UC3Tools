using VProto;

namespace LibUC3.Schema;

public class CoasterTriggerSettingsMsg : Message
{
    public enum TriggerTypeEnum
    {
        AtStart = 0,
        SynchronizeSegments = 1,
        OnTrainStopOnSegment = 2,
        OnTrainStartOnSegment = 3,
        OnTrainEnterSegment = 4,
        OnTrainExitSegment = 5
    }
    [ProtoField(1)] public List<string> SegmentName;
    [ProtoField(2)] public TriggerTypeEnum Type;
    public class SynchronizeSegmentsSettingsMsg : Message
    {
        [ProtoField(1)] public bool SyncAcrossCoasters;
    }
    [ProtoField(3)] public SynchronizeSegmentsSettingsMsg SynchronizeSegments;
    public class OnTrainStopOnSegmentSettingsMsg : Message
    {
    }
    [ProtoField(4)] public OnTrainStopOnSegmentSettingsMsg OnTrainStopOnSegment;
    public class OnTrainEnterSegmentSettingsMsg : Message
    {
    }
    [ProtoField(5)] public OnTrainEnterSegmentSettingsMsg OnTrainEnterSegment;
    public class OnTrainExitSegmentSettingsMsg : Message
    {
    }
    [ProtoField(6)] public OnTrainExitSegmentSettingsMsg OnTrainExitSegment;
    public class TriggeredActionMsg : Message
    {
        public enum TriggeredActionTypeEnum
        {
            None = 0,
            Wait = 1,
            MovePartGroup = 2,
            EvaluateExpression = 3,
            JumpToAction = 4,
            FinishTrigger = 5,
            PlaySound = 6,
            RunLuaScript = 7
        }
        
        [ProtoField(1)] public TriggeredActionTypeEnum TypeEnum;
        
        public class WaitSettingsMsg : Message
        {
            [ProtoField(1)] public float Time;
        }

        [ProtoField(2)] public WaitSettingsMsg WaitSettings;
        public class MovePartGroupSettingsMsg : Message
        {
            [ProtoField(1)] public string MovingPartGroupName;
            [ProtoField(2)] public string TargetPartGroupName;
            [ProtoField(3)] public string MovingPartName;
            [ProtoField(4)] public string TargetPartName;
            [ProtoField(5)] public bool MovingConnectionIsStart;
            [ProtoField(6)] public bool TargetConnectionIsStart;
            [ProtoField(7)] public float TransitionTime;
            [ProtoField(8)] public float TransitionBezierX1;
            [ProtoField(9)] public float TransitionBezierX2;
        }

        [ProtoField(3)] public MovePartGroupSettingsMsg MovePartGroupSettings;
        public class EvaluateExpressionSettingsMsg : Message
        {
            [ProtoField(1)] public string Expression;
            [ProtoField(2)] public bool EnableBranching;
            [ProtoField(3)] public int TrueActionIndex;
            [ProtoField(4)] public string VariableName;
        }

        [ProtoField(4)] public EvaluateExpressionSettingsMsg EvaluateExpressionSettings;
        public class JumpToActionSettingsMsg : Message
        {
            [ProtoField(1)] public int ActionIndex;
        }

        [ProtoField(5)] public JumpToActionSettingsMsg JumpToActionSettings;
        public class FinishTriggerSettingsMsg : Message
        {
        }

        [ProtoField(6)] public FinishTriggerSettingsMsg FinishTriggerSettings;
        public class PlaySoundSettingsMsg : Message
        {
            [ProtoField(1)] public OneShotSound Sound;
            [ProtoField(2)] public float Volume;
            [ProtoField(3)] public string PartGroupName;
        }

        [ProtoField(7)] public PlaySoundSettingsMsg PlaySoundSettings;
        
        public class RunLuaScriptSettingsMsg : Message
        {
            [ProtoField(1)] public string Script;
            [ProtoField(2)] public string ScriptName;
            [ProtoField(3)] public int InstructionLimit;
        }

        [ProtoField(8)] public RunLuaScriptSettingsMsg RunLuaScriptSettings;
    }
    
    [ProtoField(10)] public List<TriggeredActionMsg> TriggeredAction;
}