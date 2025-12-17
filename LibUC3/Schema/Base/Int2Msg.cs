using OpenTK.Mathematics;
using VProto;

namespace LibUC3.Schema;

public class Int2Msg : Message
{
    [ProtoField(1)] public int X;
    [ProtoField(2)] public int Y;
    
    public static implicit operator Vector2i(Int2Msg msg) => new (msg.X, msg.Y);
    public static implicit operator Int2Msg(Vector2i msg) => new() { X = msg.X, Y = msg.Y };
}