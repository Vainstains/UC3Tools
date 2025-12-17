using System.Collections.Generic;
using System.Linq;

namespace VProto.Raw;

internal class WriteState
{
    private readonly IList<byte> m_bytes;
    
    public WriteState(IList<byte> bytes)
    {
        m_bytes = bytes;
    }
    
    public WriteState()
    {
        m_bytes = new List<byte>();
    }

    public void Write(byte b)
    {
        m_bytes.Add(b);
    }
    
    public void Write(IReadOnlyList<byte> bytes)
    {
        foreach (var b in bytes)
            Write(b);
    }

    public static implicit operator byte[](WriteState state)
    {
        return state.m_bytes.ToArray();
    }
}