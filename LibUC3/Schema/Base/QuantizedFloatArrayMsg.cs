using System.Collections;
using VProto;

namespace LibUC3.Schema;

#pragma warning disable CS8618
public class QuantizedFloatArrayMsg : Message, IReadOnlyList<float>
{
    [ProtoField(1)] private byte[] m_data;
    [ProtoField(4)] private float m_minValue;
    [ProtoField(5)] private float m_maxValue;
    [ProtoField(2)] private int m_numFloats;
    private float[] m_floats;
    
    [ProtoField(3)] public int NumBitsPerFloat = 12;
    public override void OnPreSerialize()
    {
        if (m_floats == null || m_floats.Length == 0)
            throw new InvalidOperationException("No float data to serialize.");

        if (NumBitsPerFloat < 1 || NumBitsPerFloat > 32)
            throw new InvalidOperationException("NumBitsPerFloat must be in range 1–32.");

        m_numFloats = m_floats.Length;
        
        float min = float.MaxValue;
        float max = float.MinValue;

        for (int i = 0; i < m_floats.Length; i++)
        {
            float v = m_floats[i];
            if (v < min) min = v;
            if (v > max) max = v;
        }

        m_minValue = min;
        m_maxValue = max;

        int bits = NumBitsPerFloat;
        uint levels = (uint)((1L << bits) - 1);

        int totalBits = m_numFloats * bits;
        int byteCount = (totalBits + 7) / 8;
        m_data = new byte[byteCount];

        ulong bitBuffer = 0;
        int bitCount = 0;
        int byteIndex = 0;

        for (int i = 0; i < m_floats.Length; i++)
        {
            float normalized = (m_floats[i] - min) / (max - min);
            uint quantized = (uint)(levels * normalized);

            bitBuffer |= ((ulong)quantized << bitCount);
            bitCount += bits;

            while (bitCount >= 8)
            {
                m_data[byteIndex++] = (byte)(bitBuffer & 0xFF);
                bitBuffer >>= 8;
                bitCount -= 8;
            }
        }
        
        if (bitCount > 0)
        {
            m_data[byteIndex] = (byte)(bitBuffer & 0xFF);
        }
    }
    
    public override void OnPostDeserialize()
    {
        if (m_data == null || m_numFloats <= 0)
        {
            m_floats = Array.Empty<float>();
            return;
        }

        m_floats = new float[m_numFloats];

        int bits = NumBitsPerFloat;
        uint mask = (uint)((1L << bits) - 1);

        ulong bitBuffer = 0;
        int bitCount = 0;
        int byteIndex = 0;

        for (int i = 0; i < m_numFloats; i++)
        {
            while (bitCount < bits)
            {
                bitBuffer |= ((ulong)m_data[byteIndex++] << bitCount);
                bitCount += 8;
            }

            uint quantized = (uint)(bitBuffer & mask);
            bitBuffer >>= bits;
            bitCount -= bits;

            float normalized = quantized / (float)mask;
            m_floats[i] = m_minValue + normalized * (m_maxValue - m_minValue);
        }
    }

    public IEnumerator<float> GetEnumerator() => ((IEnumerable<float>)m_floats).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => m_floats.GetEnumerator();

    public int Count => m_floats.Length;

    public float this[int index] => m_floats[index];
}