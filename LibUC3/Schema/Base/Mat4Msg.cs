using OpenTK.Mathematics;
using VProto;

namespace LibUC3.Schema;

public class Mat4Msg : Message
{
    [ProtoField(1)] public float M11;
    [ProtoField(2)] public float M12;
    [ProtoField(3)] public float M13;
    [ProtoField(4)] public float M14;
    [ProtoField(5)] public float M21;
    [ProtoField(6)] public float M22;
    [ProtoField(7)] public float M23;
    [ProtoField(8)] public float M24;
    [ProtoField(9)] public float M31;
    [ProtoField(10)] public float M32;
    [ProtoField(11)] public float M33;
    [ProtoField(12)] public float M34;
    [ProtoField(13)] public float M41;
    [ProtoField(14)] public float M42;
    [ProtoField(15)] public float M43;
    [ProtoField(16)] public float M44;

    public static implicit operator Matrix4(Mat4Msg msg) => new(
        msg.M11, msg.M12, msg.M13, msg.M14,
        msg.M21, msg.M22, msg.M23, msg.M24,
        msg.M31, msg.M32, msg.M33, msg.M34,
        msg.M41, msg.M42, msg.M43, msg.M44
    );

    public static implicit operator Mat4Msg(Matrix4 mat) => new()
    {
        M11 = mat.M11, M12 = mat.M12, M13 = mat.M13, M14 = mat.M14,
        M21 = mat.M21, M22 = mat.M22, M23 = mat.M23, M24 = mat.M24,
        M31 = mat.M31, M32 = mat.M32, M33 = mat.M33, M34 = mat.M34,
        M41 = mat.M41, M42 = mat.M42, M43 = mat.M43, M44 = mat.M44
    };
}