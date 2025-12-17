using System.Collections.Generic;

namespace VProto.Raw;

internal class RawMessage
{
    private readonly List<RawField> m_fields;

    public RawMessage()
    {
        m_fields = new List<RawField>();
    }

    public void AddField(RawField field)
    {
        m_fields.Add(field);
    }

    /// <summary>
    /// Enumerate all fields in wire order
    /// </summary>
    public IReadOnlyList<RawField> Fields => m_fields;

    /// <summary>
    /// Enumerate all fields with a given field number
    /// </summary>
    public IEnumerable<RawField> GetFields(int fieldNumber)
    {
        foreach (var field in m_fields)
        {
            if (field.Key.FieldNumber == fieldNumber)
                yield return field;
        }
    }

    /// <summary>
    /// Get the first field with the given field number (common for non-repeated fields)
    /// </summary>
    public bool TryGetFirstField(int fieldNumber, out RawField field)
    {
        foreach (var f in m_fields)
        {
            if (f.Key.FieldNumber == fieldNumber)
            {
                field = f;
                return true;
            }
        }

        field = null;
        return false;
    }

    public static RawMessage Parse(ReadState state)
    {
        var msg = new RawMessage();

        while (state.HasNext)
        {
            var field = RawField.Parse(state);
            msg.AddField(field);
        }

        return msg;
    }

    public void Encode(WriteState state)
    {
        foreach (var field in m_fields)
        {
            field.Encode(state);
        }
    }
}