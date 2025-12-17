using OpenTK.Mathematics;
using VProto;

namespace LibUC3.Schema;

public class Mat3Msg : Message
{
    [ProtoField(1)] public float M11;
    [ProtoField(2)] public float M12;
    [ProtoField(3)] public float M13;
    [ProtoField(4)] public float M21;
    [ProtoField(5)] public float M22;
    [ProtoField(6)] public float M23;
    [ProtoField(7)] public float M31;
    [ProtoField(8)] public float M32;
    [ProtoField(9)] public float M33;

    public static implicit operator Matrix3(Mat3Msg msg) => new(
        msg.M11, msg.M12, msg.M13,
        msg.M21, msg.M22, msg.M23,
        msg.M31, msg.M32, msg.M33
    );

    public static implicit operator Mat3Msg(Matrix3 mat) => new()
    {
        M11 = mat.M11, M12 = mat.M12, M13 = mat.M13,
        M21 = mat.M21, M22 = mat.M22, M23 = mat.M23,
        M31 = mat.M31, M32 = mat.M32, M33 = mat.M33
    };
}