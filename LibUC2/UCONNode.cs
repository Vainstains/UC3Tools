namespace LibUC2;

public class UCONNode
{
    public string Type;
    public Dictionary<string, string> Fields = new Dictionary<string, string>();

    public UCONNode(string type)
    {
        Type = type;
    }

    private static int FindNumFieldChars(string text, int start)
    {
        //            
        // [abc]{hi:123,abc:[array]{size:2,0:2.3}}
        //       ^ start

        int depth = 0;
        for (int i = start; i < text.Length; i++)
        {
            if (text[i] == '{' || text[i] == '[')
            {
                depth++;
                continue;
            }

            if (text[i] == '}' || text[i] == ']')
            {
                depth--;
                if (depth == -1)
                {
                    return i - start;
                }
            }

            if (text[i] == ',' && depth == 0)
                return i - start;
        }

        return -1;
    }

    public static UCONNode ParseBaseNode(string text)
    {
        // [type]{ ... }

        int typeClosingBracket = text.IndexOf(']');
        string type = text.Substring(1, typeClosingBracket - 1);
        UCONNode node = new UCONNode(type);

        int contentStart = typeClosingBracket + 2;
        int contentEnd = text.Length - 1;
        string content = text.Substring(contentStart, contentEnd - contentStart);

        int pos = 0;
        while (pos < content.Length)
        {
            int colonPos = content.IndexOf(':', pos);
            if (colonPos == -1) break;

            string key = content.Substring(pos, colonPos - pos).Trim();
            pos = colonPos + 1;

            int valueLength = FindNumFieldChars(content, pos);
            if (valueLength == -1) break;

            string value = content.Substring(pos, valueLength).Trim();

            node.Fields[key] = value;

            pos += valueLength;
            if (pos < content.Length && content[pos] == ',')
                pos++;
        }

        return node;
    }

    public bool TryGetNode(string name, out UCONNode node)
    {
        node = null!;
        if (!Fields.TryGetValue(name, out var str)) return false;
        node = ParseBaseNode(str);
        return true;
    }

    public bool TryGetBool(string name, out bool b)
    {
        b = false;
        if (!Fields.TryGetValue(name, out var str)) return false;
        b = str == "T";
        return true;
    }

    public bool TryGetString(string name, out string s)
    {
        return Fields.TryGetValue(name, out s!);
    }

    public bool TryGetInt(string name, out int i)
    {
        i = 0;
        if (!Fields.TryGetValue(name, out var str)) return false;
        return int.TryParse(str, out i);
    }

    public bool TryGetFloat(string name, out float f)
    {
        f = 0;
        if (!Fields.TryGetValue(name, out var str)) return false;
        return float.TryParse(str, out f);
    }

    public bool TryGetIntTuple(string name, out int[] tuple)
    {
        tuple = null!;
        if (!Fields.TryGetValue(name, out var str)) return false;

        var parts = str.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        tuple = new int[parts.Length];

        for (int i = 0; i < parts.Length; i++)
        {
            if (!int.TryParse(parts[i], out tuple[i]))
                return false;
        }

        return true;
    }

    public bool TryGetFloatTuple(string name, out float[] tuple)
    {
        tuple = null!;
        if (!Fields.TryGetValue(name, out var str)) return false;

        var parts = str.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        tuple = new float[parts.Length];

        for (int i = 0; i < parts.Length; i++)
        {
            if (!float.TryParse(parts[i], out tuple[i]))
                return false;
        }

        return true;
    }

    public bool TryGetArray(string name, out string[] array)
    {
        array = null!;
        if (!TryGetNode(name, out var arrayNode)) return false;
        if (arrayNode.Type != "array") return false;

        if (!arrayNode.TryGetInt("size", out int size)) return false;

        array = new string[size];
        for (int i = 0; i < size; i++)
        {
            if (!arrayNode.Fields.TryGetValue(i.ToString(), out array[i]))
                return false;
        }

        return true;
    }
    
    public bool TryGetParseable<T>(string name, out T instance) where T : IParsable
    {
        instance = default!;
        if (!Fields.TryGetValue(name, out var str)) return false;
        instance = Activator.CreateInstance<T>();
        instance.Parse(str);
        return true;
    }
}

public interface IParsable
{
    public void Parse(string text);
}