using System;

public static class FastSaveDataCrypto
{
    private const string DefaultSecret = "CHANGE_THIS_FAST_SAVE_DATA_SECRET_2026";

    public static byte[] Protect(byte[] data, string secret)
    {
        ValidateSecret(secret);
        var key = CreateKey(secret);
        var result = new byte[data.Length + 8];

        Buffer.BlockCopy(BitConverter.GetBytes(Checksum(data, key)), 0, result, 0, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(data.Length), 0, result, 4, 4);

        for (var i = 0; i < data.Length; i++)
            result[i + 8] = (byte)(data[i] ^ key[i % key.Length]);

        return result;
    }

    public static byte[] Unprotect(byte[] data, string secret)
    {
        ValidateSecret(secret);
        if (data == null || data.Length < 8) throw new InvalidOperationException("Invalid FastSaveData file.");

        var key = CreateKey(secret);
        var expectedLength = BitConverter.ToInt32(data, 4);
        if (expectedLength < 0 || expectedLength != data.Length - 8) throw new InvalidOperationException("Invalid FastSaveData length.");

        var result = new byte[expectedLength];
        for (var i = 0; i < result.Length; i++)
            result[i] = (byte)(data[i + 8] ^ key[i % key.Length]);

        var expectedChecksum = BitConverter.ToUInt32(data, 0);
        if (Checksum(result, key) != expectedChecksum) throw new InvalidOperationException("FastSaveData checksum validation failed.");

        return result;
    }

    private static byte[] CreateKey(string secret)
    {
        unchecked
        {
            var hash = 2166136261u;
            for (var i = 0; i < secret.Length; i++)
            {
                hash ^= secret[i];
                hash *= 16777619u;
            }

            var key = new byte[32];
            for (var i = 0; i < key.Length; i++)
            {
                hash ^= (uint)(i * 0x9E3779B9);
                hash *= 16777619u;
                hash ^= hash >> 13;
                key[i] = (byte)(hash >> ((i & 3) * 8));
            }

            return key;
        }
    }

    private static uint Checksum(byte[] data, byte[] key)
    {
        unchecked
        {
            var hash = 2166136261u;
            for (var i = 0; i < data.Length; i++)
            {
                hash ^= (uint)(data[i] ^ key[i % key.Length]);
                hash *= 16777619u;
                hash ^= hash >> 13;
            }
            return hash;
        }
    }

    private static void ValidateSecret(string secret)
    {
        if (string.IsNullOrWhiteSpace(secret) || secret == DefaultSecret)
            throw new InvalidOperationException("Change FastSaveData Lite Secret before shipping.");
    }
}
