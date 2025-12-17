using VProto;

namespace LibUC3.Schema;

public class CoasterPartSettingsMsg : Message
{
    public class ConnectNodeSettingsMsg : Message
    {
        [ProtoField(1)] public string PartUuid;
        [ProtoField(2)] public bool IsStartOfPart;
    }

    [ProtoField(1)] public List<CoasterCurveSettingsMsg> Curves;
    [ProtoField(2)] public bool IsClosedCircuit;
    [ProtoField(3)] public string Name;
    [ProtoField(4)] public string Uuid;
    [ProtoField(5)] public bool ClosedCircuitWasForward;
    [ProtoField(6)] public ConnectNodeSettingsMsg InitialStartConnection;
    [ProtoField(7)] public ConnectNodeSettingsMsg InitialEndConnection;
}