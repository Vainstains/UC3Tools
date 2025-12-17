using VProto;

namespace LibUC3.Schema;

public class CoasterSoundSettingsMsg : Message
{
    [ProtoField(1)] public LoopingSoundEnum ChainLiftSound;
    [ProtoField(2)] public LoopingSoundEnum ChainEngineSound;
    [ProtoField(3)] public LoopingSoundEnum LSMSound;
    [ProtoField(4)] public LoopingSoundEnum MagneticFinsBrakeSound;
    [ProtoField(5)] public LoopingSoundEnum MagneticFinsLaunchSound;
    [ProtoField(6)] public LoopingSoundEnum TrackSoundLayer0;
    [ProtoField(7)] public LoopingSoundEnum TrackSoundLayer1;
}