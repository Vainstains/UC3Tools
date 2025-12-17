using OpenTK.Mathematics;
using VProto;

namespace LibUC3.Schema;

public class Int3Msg : Message
{
    [ProtoField(1)] public int X;
    [ProtoField(2)] public int Y;
    [ProtoField(3)] public int Z;

    public Int3Msg(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static implicit operator Vector3i(Int3Msg msg) => new(msg.X, msg.Y, msg.Z);
    public static implicit operator Int3Msg(Vector3i vec) => new(x: vec.X, y: vec.Y, z: vec.Z);
}