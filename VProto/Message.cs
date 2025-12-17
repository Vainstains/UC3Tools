using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using VProto.Raw;

namespace VProto;

[AttributeUsage(AttributeTargets.Field)]
public class ProtoFieldAttribute : Attribute
{
    public int FieldNumber { get; }

    public ProtoFieldAttribute(int fieldNumber)
    {
        FieldNumber = fieldNumber;
    }
}

public enum ProtobufType
{
    Int32, // int
    Int64, // long
    UInt32, // uint
    UInt64, // ulong
    SInt32, // int
    SInt64, // long
    Bool, // bool
    Enum, // Whatever enum type
    Fixed64, // ulong
    SFixed64, // long
    Double, // double
    String, // string
    Bytes, // byte[]
    Message, // Whatever class inheriting from Message
    Fixed32, // uint
    SFixed32, // int
    Float // float
}

public abstract class Message
{
    public virtual void OnPreSerialize()
    {
        
    }
    public virtual void OnPostDeserialize()
    {
        
    }

    public class MessageField
    {
        public int FieldNumber;
        public ProtobufType ProtobufType;
        public Type AssociatedType;
        public bool IsListOf; // repeated -> List<T>
        public FieldInfo FieldInfo;
    }

    public class MessageInfo
    {
        private Dictionary<int, MessageField> m_fields = new Dictionary<int, MessageField>();
        public Dictionary<int, MessageField> Fields => m_fields;
        public MessageInfo(Type type)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                var protoAttr = field.GetCustomAttribute<ProtoFieldAttribute>();
                if (protoAttr == null)
                    continue; // Skip fields without [ProtoField]

                var fieldType = field.FieldType;
                var isList = field.FieldType.IsGenericType &&
                             field.FieldType.GetGenericTypeDefinition() == typeof(List<>);
                if (isList)
                {
                    fieldType = isList ? fieldType.GetGenericArguments()[0] : fieldType;
                    Console.WriteLine();
                }

                m_fields[protoAttr.FieldNumber] = new MessageField
                {
                    FieldNumber = protoAttr.FieldNumber,
                    ProtobufType = DetermineProtobufType(fieldType),
                    AssociatedType = fieldType,
                    IsListOf = isList,
                    FieldInfo = field
                };
            }
        }
        
        private static ProtobufType DetermineProtobufType(Type type)
        {
            if (type == typeof(int)) return ProtobufType.Int32;
            if (type == typeof(long)) return ProtobufType.Int64;
            if (type == typeof(uint)) return ProtobufType.UInt32;
            if (type == typeof(ulong)) return ProtobufType.UInt64;
            if (type == typeof(bool)) return ProtobufType.Bool;
            if (type == typeof(double)) return ProtobufType.Double;
            if (type == typeof(float)) return ProtobufType.Float;
            if (type == typeof(string)) return ProtobufType.String;
            if (type == typeof(byte[])) return ProtobufType.Bytes;
            if (type.IsEnum) return ProtobufType.Enum;
            if (typeof(Message).IsAssignableFrom(type)) return ProtobufType.Message;
            // Add Fixed32/Fixed64/SFixed types if needed

            throw new NotSupportedException($"Unsupported type: {type.FullName}");
        }

        public bool TryGetField(int fieldNumber, out MessageField field)
        {
            return m_fields.TryGetValue(fieldNumber, out field);
        }
    }
    
    private static Dictionary<Type, MessageInfo> s_fieldInfos = new();

    public static MessageInfo GetMessageInfo(Type type)
    {
        if (s_fieldInfos.TryGetValue(type, out var info))
            return info;
        var messageInfo = new MessageInfo(type);
        s_fieldInfos.Add(type, messageInfo);
        return messageInfo;
    }

    public static TMessage Parse<TMessage>(byte[] bytes) where TMessage : Message
    {
        return (TMessage)ParseInternal(typeof(TMessage), bytes);
    }
    private static Message ParseInternal(Type messageType, ReadState state)
    {
        var messageInfo = GetMessageInfo(messageType);
        var rawMessage = RawMessage.Parse(state);
        var message = (Message)Activator.CreateInstance(messageType);

        foreach (var field in rawMessage.Fields)
        {
            var num = field.Key.FieldNumber;
            byte[] data = field.Data;
            if (!messageInfo.TryGetField(num, out var messageField))
                continue; // unknown field, skip
            
            object value = DecodeValue(data, messageField.ProtobufType, messageField.AssociatedType);
            
            if (messageField.IsListOf)
            {
                Type listType = typeof(List<>).MakeGenericType(messageField.AssociatedType);
                IList listInstance = (IList)messageField.FieldInfo.GetValue(message);
                if (listInstance == null)
                {
                    listInstance = (IList)Activator.CreateInstance(listType);
                    messageField.FieldInfo.SetValue(message, listInstance);
                }
                listInstance.Add(value);
            }
            else
            {
                messageField.FieldInfo.SetValue(message, value);
            }
        }
        message.OnPostDeserialize();
        return message;
    }

    private static object DecodeValue(byte[] data, ProtobufType type, Type associatedType)
    {
        switch (type)
        {
            case ProtobufType.Int32:
                return (int)Utils.DecodeVarint((ReadState)data);

            case ProtobufType.Int64:
                return (long)Utils.DecodeVarint((ReadState)data);

            case ProtobufType.UInt32:
                return (uint)Utils.DecodeVarint((ReadState)data);

            case ProtobufType.UInt64:
                return Utils.DecodeVarint((ReadState)data);

            case ProtobufType.SInt32:
            {
                var raw = Utils.DecodeVarint((ReadState)data, out var _);
                return (int)Utils.ZigZagDecode(raw);
            }

            case ProtobufType.SInt64:
            {
                var raw = Utils.DecodeVarint((ReadState)data, out var _);
                return Utils.ZigZagDecode(raw);
            }

            case ProtobufType.Bool:
                return Utils.DecodeVarint((ReadState)data) != 0;

            case ProtobufType.Enum:
                return (int)Utils.DecodeVarint((ReadState)data);

            case ProtobufType.Fixed32:
                if (data.Length != 4)
                    throw new FormatException("Invalid Fixed32 length");
                return BitConverter.ToUInt32(data, 0);

            case ProtobufType.Fixed64:
                if (data.Length != 8)
                    throw new FormatException("Invalid Fixed64 length");
                return BitConverter.ToUInt64(data, 0);

            case ProtobufType.SFixed32:
                if (data.Length != 4)
                    throw new FormatException("Invalid SFixed32 length");
                return BitConverter.ToInt32(data, 0);

            case ProtobufType.SFixed64:
                if (data.Length != 8)
                    throw new FormatException("Invalid SFixed64 length");
                return BitConverter.ToInt64(data, 0);

            case ProtobufType.Float:
                if (data.Length != 4)
                    throw new FormatException("Invalid Float length");
                return BitConverter.ToSingle(data, 0);

            case ProtobufType.Double:
                if (data.Length != 8)
                    throw new FormatException("Invalid Double length");
                return BitConverter.ToDouble(data, 0);

            case ProtobufType.String:
                return System.Text.Encoding.UTF8.GetString(data);

            case ProtobufType.Bytes:
                return data;

            case ProtobufType.Message:
            {
                return ParseInternal(associatedType, (ReadState)data);
            }

            default:
                throw new NotSupportedException($"Unsupported ProtobufType: {type}");
        }
    }
    
    private static byte[] EncodeValue(object value, ProtobufType type)
    {
        switch (type)
        {
            case ProtobufType.Int32:
            case ProtobufType.Int64:
            case ProtobufType.UInt32:
            case ProtobufType.UInt64:
            case ProtobufType.Bool:
            case ProtobufType.Enum:
                return Utils.EncodeVarint(Convert.ToUInt64(value)).ToArray();

            case ProtobufType.SInt32:
                return Utils.EncodeVarint(Utils.ZigZagEncode((int)value)).ToArray();

            case ProtobufType.SInt64:
                return Utils.EncodeVarint(Utils.ZigZagEncode((long)value)).ToArray();

            case ProtobufType.Fixed32:
                return BitConverter.GetBytes((uint)value);

            case ProtobufType.Fixed64:
                return BitConverter.GetBytes((ulong)value);

            case ProtobufType.SFixed32:
                return BitConverter.GetBytes((int)value);

            case ProtobufType.SFixed64:
                return BitConverter.GetBytes((long)value);

            case ProtobufType.Float:
                return BitConverter.GetBytes((float)value);

            case ProtobufType.Double:
                return BitConverter.GetBytes((double)value);

            case ProtobufType.String:
                return System.Text.Encoding.UTF8.GetBytes((string)value);

            case ProtobufType.Bytes:
                return (byte[])value;

            case ProtobufType.Message:
                var nestedRaw = ToRawMessage((Message)value);
                var nestedState = new List<byte>();
                nestedRaw.Encode(new WriteState(nestedState));
                return nestedState.ToArray();

            default:
                throw new NotSupportedException($"Unsupported type: {type}");
        }
    }
    
    public byte[] SerializeToBytes()
    {
        var state = new WriteState();
        var raw = ToRawMessage(this);
        raw.Encode(state);
        return state;
    }

    private static RawMessage ToRawMessage(Message message)
    {
        message.OnPreSerialize();
        var messageType = message.GetType();
        var messageInfo = GetMessageInfo(messageType);
        
        var fieldsDict = messageInfo.Fields;

        var rawMessage = new RawMessage();

        foreach (var kvp in fieldsDict)
        {
            var fieldInfoObj = kvp.Value;
            var fieldField = fieldInfoObj.GetType().GetField("FieldInfo");
            var protobufTypeField = fieldInfoObj.GetType().GetField("ProtobufType");
            var isListField = fieldInfoObj.GetType().GetField("IsListOf");
            var fieldNumberField = fieldInfoObj.GetType().GetField("FieldNumber");

            var fieldInfo = (FieldInfo)fieldField.GetValue(fieldInfoObj);
            var type = (ProtobufType)protobufTypeField.GetValue(fieldInfoObj);
            var isList = (bool)isListField.GetValue(fieldInfoObj);
            var fieldNumber = (int)fieldNumberField.GetValue(fieldInfoObj);

            if (isList)
            {
                var list = (IEnumerable)fieldInfo.GetValue(message);
                if (list == null) continue;

                foreach (var item in list)
                {
                    var data = EncodeValue(item, type);
                    var key = new RawFieldKey(WireTypeFromProtobufType(type), fieldNumber);
                    rawMessage.AddField(new RawField(key, data));
                }
            }
            else
            {
                var value = fieldInfo.GetValue(message);
                if (value == null) continue;

                var data = EncodeValue(value, type);
                var key = new RawFieldKey(WireTypeFromProtobufType(type), fieldNumber);
                rawMessage.AddField(new RawField(key, data));
            }
        }

        return rawMessage;
    }
    
    private static WireType WireTypeFromProtobufType(ProtobufType type)
    {
        return type switch
        {
            ProtobufType.Int32 => WireType.Varint,
            ProtobufType.Int64 => WireType.Varint,
            ProtobufType.UInt32 => WireType.Varint,
            ProtobufType.UInt64 => WireType.Varint,
            ProtobufType.SInt32 => WireType.Varint,
            ProtobufType.SInt64 => WireType.Varint,
            ProtobufType.Bool => WireType.Varint,
            ProtobufType.Enum => WireType.Varint,
            ProtobufType.Fixed32 => WireType.Fixed32,
            ProtobufType.Fixed64 => WireType.Fixed64,
            ProtobufType.SFixed32 => WireType.Fixed32,
            ProtobufType.SFixed64 => WireType.Fixed64,
            ProtobufType.Float => WireType.Fixed32,
            ProtobufType.Double => WireType.Fixed64,
            ProtobufType.String => WireType.LengthDelimited,
            ProtobufType.Bytes => WireType.LengthDelimited,
            ProtobufType.Message => WireType.LengthDelimited,
            _ => throw new NotSupportedException($"Unsupported ProtobufType: {type}")
        };
    }
}