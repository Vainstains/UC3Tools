using OpenTK.Mathematics;
using VProto;

namespace LibUC3.Schema;

public class Vec3Msg : Message
{
    [ProtoField(1)] public float X;
    [ProtoField(2)] public float Y;
    [ProtoField(3)] public float Z;

    public static implicit operator Vector3(Vec3Msg msg) => new(msg.X, msg.Y, msg.Z);
    public static implicit operator Vec3Msg(Vector3 vec) => new() { X = vec.X, Y = vec.Y, Z = vec.Z };
}