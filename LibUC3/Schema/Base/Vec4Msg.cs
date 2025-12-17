using OpenTK.Mathematics;
using VProto;

namespace LibUC3.Schema;

public class Vec4Msg : Message
{
    [ProtoField(1)] public float X;
    [ProtoField(2)] public float Y;
    [ProtoField(3)] public float Z;
    [ProtoField(4)] public float W;

    public static implicit operator Vector4(Vec4Msg msg) => new(msg.X, msg.Y, msg.Z, msg.W);
    public static implicit operator Vec4Msg(Vector4 vec) => new() { X = vec.X, Y = vec.Y, Z = vec.Z, W = vec.W };
}