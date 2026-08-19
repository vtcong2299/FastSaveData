using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FastSaveDataStorage
{
    public static string FilePath => Path.Combine(Application.persistentDataPath, "FastSaveData.dat");
    private static string BackupPath => FilePath + ".bak";
    private static string TempPath => FilePath + ".tmp";

    public static void Save(IReadOnlyDictionary<string, object> entries, string secret, bool backup)
    {
        var raw = FastSaveDataSerializer.Serialize(entries);
        var protectedData = FastSaveDataCrypto.Protect(raw, secret);

        File.WriteAllBytes(TempPath, protectedData);

        if (backup && File.Exists(FilePath))
            File.Copy(FilePath, BackupPath, true);

        if (File.Exists(FilePath)) File.Delete(FilePath);
        File.Move(TempPath, FilePath);
    }

    public static Dictionary<string, object> Load(string secret)
    {
        if (!File.Exists(FilePath)) return new Dictionary<string, object>();

        try
        {
            return FastSaveDataSerializer.Deserialize(FastSaveDataCrypto.Unprotect(File.ReadAllBytes(FilePath), secret));
        }
        catch
        {
            if (!File.Exists(BackupPath)) return new Dictionary<string, object>();

            try
            {
                return FastSaveDataSerializer.Deserialize(FastSaveDataCrypto.Unprotect(File.ReadAllBytes(BackupPath), secret));
            }
            catch
            {
                return new Dictionary<string, object>();
            }
        }
    }

    public static void Clear()
    {
        DeleteIfExists(FilePath);
        DeleteIfExists(BackupPath);
        DeleteIfExists(TempPath);
    }

    private static void DeleteIfExists(string path)
    {
        if (File.Exists(path)) File.Delete(path);
    }
}
