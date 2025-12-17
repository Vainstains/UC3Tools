using OpenTK.Mathematics;
using VProto;

namespace LibUC3.Schema;

public class ColorMsg : Message
{
    [ProtoField(1)] public float R;
    [ProtoField(2)] public float G;
    [ProtoField(3)] public float B;
    [ProtoField(4)] public float A;

    public static implicit operator Color4(ColorMsg msg) => new(msg.R, msg.G, msg.B, msg.A);
    public static implicit operator ColorMsg(Color4 color) => new() { R = color.R, G = color.G, B = color.B, A = color.A };
}