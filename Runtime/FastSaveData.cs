using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FastSaveData
{
    private const string Secret = "FastSave_8F29Kx7Qm2P6";
    private const bool Backup = true;

    private static Dictionary<string, object> entries;
    private static bool initialized;
    private static bool dirty;

    public static bool IsDirty => dirty;
    public static int Count => GetEntries().Count;
    public static string FilePath => FastSaveDataStorage.FilePath;

    private static Dictionary<string, object> GetEntries()
    {
        EnsureInitialized();
        return entries;
    }

    private static void EnsureInitialized()
    {
        if (initialized) return;
        entries = FastSaveDataStorage.Load(Secret);
        initialized = true;
        dirty = false;
    }

    public static void SetInt(string key, int value) => Set(key, value);
    public static int GetInt(string key, int defaultValue = 0) => Get(key, defaultValue);

    public static void SetFloat(string key, float value) => Set(key, value);
    public static float GetFloat(string key, float defaultValue = 0f) => Get(key, defaultValue);

    public static void SetDouble(string key, double value) => Set(key, value);
    public static double GetDouble(string key, double defaultValue = 0d) => Get(key, defaultValue);

    public static void SetLong(string key, long value) => Set(key, value);
    public static long GetLong(string key, long defaultValue = 0L) => Get(key, defaultValue);

    public static void SetBool(string key, bool value) => Set(key, value);
    public static bool GetBool(string key, bool defaultValue = false) => Get(key, defaultValue);

    public static void SetString(string key, string value) => Set(key, value);
    public static string GetString(string key, string defaultValue = "") => Get(key, defaultValue);

    public static void SetByte(string key, byte value) => Set(key, value);
    public static byte GetByte(string key, byte defaultValue = 0) => Get(key, defaultValue);

    public static void SetShort(string key, short value) => Set(key, value);
    public static short GetShort(string key, short defaultValue = 0) => Get(key, defaultValue);

    public static void SetUInt(string key, uint value) => Set(key, value);
    public static uint GetUInt(string key, uint defaultValue = 0) => Get(key, defaultValue);

    public static void SetULong(string key, ulong value) => Set(key, value);
    public static ulong GetULong(string key, ulong defaultValue = 0) => Get(key, defaultValue);

    public static void Set<T>(string key, T value)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key cannot be null or empty.");
        var data = GetEntries();
        if (data.TryGetValue(key, out var old) && Equals(old, value)) return;
        data[key] = value;
        dirty = true;
    }

    public static T Get<T>(string key, T defaultValue = default)
    {
        var data = GetEntries();
        if (!data.TryGetValue(key, out var value) || value == null) return defaultValue;
        if (value is T typed) return typed;
        if (value is List<object> list && typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
        {
            var itemType = typeof(T).GetGenericArguments()[0];
            var result = (System.Collections.IList)Activator.CreateInstance(typeof(T));
            foreach (var item in list) result.Add(ConvertValue(item, itemType));
            return (T)result;
        }
        try { return (T)Convert.ChangeType(value, typeof(T)); }
        catch { return defaultValue; }
    }

    public static void SetList<T>(string key, List<T> value) => Set(key, value);
    public static List<T> GetList<T>(string key, List<T> defaultValue = null) => Get(key, defaultValue);

    public static bool HasKey(string key) => GetEntries().ContainsKey(key);

    public static void DeleteKey(string key)
    {
        if (GetEntries().Remove(key)) dirty = true;
    }

    public static void DeleteAll() => ClearData();

    public static void Save()
    {
        EnsureInitialized();
        if (!dirty) return;
        FastSaveDataStorage.Save(entries, Secret, Backup);
        dirty = false;
    }

    public static void Reload()
    {
        entries = FastSaveDataStorage.Load(Secret);
        initialized = true;
        dirty = false;
    }

    public static void ClearData()
    {
        FastSaveDataStorage.Clear();
        if (initialized) entries.Clear();
        else entries = new Dictionary<string, object>();
        initialized = true;
        dirty = false;
    }

    private static object ConvertValue(object value, Type type)
    {
        if (value == null) return null;
        if (type.IsInstanceOfType(value)) return value;
        return Convert.ChangeType(value, type);
    }
}
