using System;
using System.Collections.Generic;

namespace VProto.Raw;

internal class ReadState
{
    private readonly IReadOnlyList<byte> m_bytes;
    private int m_position;
    public bool HasNext => m_position < m_bytes.Count;

    private ReadState(IReadOnlyList<byte> bytes)
    {
        m_bytes = bytes;
        m_position = 0;
    }

    public byte Next()
    {
        return m_bytes[m_position++];
    }
    
    public static implicit operator ReadState(List<byte> bytes) => new ReadState(bytes);
    public static implicit operator ReadState(byte[] bytes) => new ReadState(bytes);

    public byte[] ReadBytes(int n)
    {
        var bytes = new byte[n];
        for (var i = 0; i < n; i++)
            bytes[i] = m_bytes[m_position + i];
        m_position += n;
        return bytes;
    }
}