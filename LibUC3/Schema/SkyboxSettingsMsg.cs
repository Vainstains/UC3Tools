using VProto;

namespace LibUC3.Schema;

public class SkyboxSettingsMsg : Message
{
    [ProtoField(1)] public float TimeOfDay;
    [ProtoField(2)] public float EclipticAngle;
    [ProtoField(13)] public float MoonAngle;
    [ProtoField(11)] public ColorMsg DaySkyColor;
    [ProtoField(12)] public ColorMsg NightSkyColor;
    [ProtoField(20)] public float CloudCover;
    [ProtoField(21)] public ColorMsg CloudColor;
    [ProtoField(30)] public float FogMaxY;
    [ProtoField(31)] public float FogDistanceIntensity;
    [ProtoField(32)] public float FogVerticalDensity;
}