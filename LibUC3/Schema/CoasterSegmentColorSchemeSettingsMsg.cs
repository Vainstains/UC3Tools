using VProto;

namespace LibUC3.Schema;

public class CoasterSegmentColorSchemeSettingsMsg : Message
{
    [ProtoField(1)] public ColorMsg SupportColor;
    [ProtoField(2)] public ColorMsg SupportFooterColor;
    [ProtoField(3)] public ColorMsg CatwalkColor;
    [ProtoField(4)] public ColorMsg LeftRailColor;
    [ProtoField(5)] public ColorMsg RightRailColor;
    [ProtoField(6)] public ColorMsg BraceColor;
    [ProtoField(7)] public ColorMsg SpineColor;
    [ProtoField(8)] public ColorMsg SpineAltColor;
    [ProtoField(9)] public ColorMsg MagneticFinColor;
    [ProtoField(10)] public ColorMsg MachineryColor;
    [ProtoField(11)] public ColorMsg TunnelColor;
    
    [ProtoField(20)] public bool UseSimpleEditor;
}