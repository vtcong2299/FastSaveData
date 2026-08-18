using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public static class FastSaveDataSerializer
{
    private const int Version = 1;

    public static byte[] Serialize(IReadOnlyDictionary<string, object> entries)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);
        writer.Write(Encoding.ASCII.GetBytes("FSDL"));
        writer.Write(Version);
        writer.Write(entries.Count);

        foreach (var pair in entries)
        {
            writer.Write(pair.Key);
            WriteValue(writer, pair.Value);
        }

        writer.Flush();
        return stream.ToArray();
    }

    public static Dictionary<string, object> Deserialize(byte[] data)
    {
        using var stream = new MemoryStream(data);
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var magic = Encoding.ASCII.GetString(reader.ReadBytes(4));
        if (magic != "FSDL") throw new InvalidDataException("Invalid FastSaveData file.");

        var version = reader.ReadInt32();
        if (version != Version) throw new InvalidDataException($"Unsupported FastSaveData version: {version}.");

        var count = reader.ReadInt32();
        if (count < 0 || count > 100000) throw new InvalidDataException("Invalid entry count.");

        var result = new Dictionary<string, object>(count);
        for (var i = 0; i < count; i++)
        {
            var key = reader.ReadString();
            result[key] = ReadValue(reader);
        }

        return result;
    }

    private static void WriteValue(BinaryWriter writer, object value)
    {
        if (value == null) { writer.Write((byte)0); return; }

        switch (value)
        {
            case bool v: writer.Write((byte)1); writer.Write(v); return;
            case int v: writer.Write((byte)2); writer.Write(v); return;
            case long v: writer.Write((byte)3); writer.Write(v); return;
            case float v: writer.Write((byte)4); writer.Write(v); return;
            case double v: writer.Write((byte)5); writer.Write(v); return;
            case string v: writer.Write((byte)6); writer.Write(v); return;
            case byte v: writer.Write((byte)7); writer.Write(v); return;
            case short v: writer.Write((byte)8); writer.Write(v); return;
            case uint v: writer.Write((byte)9); writer.Write(v); return;
            case ulong v: writer.Write((byte)10); writer.Write(v); return;
            case ushort v: writer.Write((byte)11); writer.Write(v); return;
            case sbyte v: writer.Write((byte)12); writer.Write(v); return;
            case decimal v: writer.Write((byte)13); writer.Write(v); return;
            case char v: writer.Write((byte)14); writer.Write(v); return;
            case DateTime v: writer.Write((byte)15); writer.Write(v.Ticks); return;
            case Guid v: writer.Write((byte)16); writer.Write(v.ToByteArray()); return;
            case Vector2 v: writer.Write((byte)17); writer.Write(v.x); writer.Write(v.y); return;
            case Vector3 v: writer.Write((byte)18); writer.Write(v.x); writer.Write(v.y); writer.Write(v.z); return;
            case Vector4 v: writer.Write((byte)19); writer.Write(v.x); writer.Write(v.y); writer.Write(v.z); writer.Write(v.w); return;
            case Quaternion v: writer.Write((byte)20); writer.Write(v.x); writer.Write(v.y); writer.Write(v.z); writer.Write(v.w); return;
            case Color v: writer.Write((byte)21); writer.Write(v.r); writer.Write(v.g); writer.Write(v.b); writer.Write(v.a); return;
            case Color32 v: writer.Write((byte)22); writer.Write(v.r); writer.Write(v.g); writer.Write(v.b); writer.Write(v.a); return;
            case IList list:
                writer.Write((byte)23);
                writer.Write(list.Count);
                for (var i = 0; i < list.Count; i++) WriteValue(writer, list[i]);
                return;
            // case Array array:
            //     writer.Write((byte)24);
            //     writer.Write(array.Length);
            //     for (var i = 0; i < array.Length; i++) WriteValue(writer, array.GetValue(i));
            //     return;
            default:
                throw new NotSupportedException($"FastSaveData Lite does not support type: {value.GetType().FullName}");
        }
    }

    private static object ReadValue(BinaryReader reader)
    {
        switch (reader.ReadByte())
        {
            case 0: return null;
            case 1: return reader.ReadBoolean();
            case 2: return reader.ReadInt32();
            case 3: return reader.ReadInt64();
            case 4: return reader.ReadSingle();
            case 5: return reader.ReadDouble();
            case 6: return reader.ReadString();
            case 7: return reader.ReadByte();
            case 8: return reader.ReadInt16();
            case 9: return reader.ReadUInt32();
            case 10: return reader.ReadUInt64();
            case 11: return reader.ReadUInt16();
            case 12: return reader.ReadSByte();
            case 13: return reader.ReadDecimal();
            case 14: return reader.ReadChar();
            case 15: return new DateTime(reader.ReadInt64());
            case 16: return new Guid(reader.ReadBytes(16));
            case 17: return new Vector2(reader.ReadSingle(), reader.ReadSingle());
            case 18: return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            case 19: return new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            case 20: return new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            case 21: return new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            case 22: return new Color32(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
            case 23:
            case 24:
                var count = reader.ReadInt32();
                if (count < 0 || count > 100000) throw new InvalidDataException("Invalid collection size.");
                var list = new List<object>(count);
                for (var i = 0; i < count; i++) list.Add(ReadValue(reader));
                return list;
            default:
                throw new InvalidDataException("Unknown FastSaveData value type.");
        }
    }
}
