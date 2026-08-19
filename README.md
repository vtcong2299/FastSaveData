# FastSaveData Lite

Fast local save system for casual Unity games.

Rules:
- All runtime classes use the `Fast` prefix.
- No namespace.
- No ScriptableObject settings asset.
- No bootstrap GameObject.
- Static API similar to PlayerPrefs.
- Includes `ClearData()` and an Editor menu to clear data without entering Play Mode.

Example:

```csharp
FastSaveData.SetInt(PlayerPrefKey.LEVEL, Level);
FastSaveData.SetFloat(PlayerPrefKey.GOLD, Gold);
FastSaveData.SetBool("NoAds", IsRemoveAds);
FastSaveData.Save();

int level = FastSaveData.GetInt(PlayerPrefKey.LEVEL, 1);
List<int> levels = FastSaveData.GetList<int>("Levels");

FastSaveData.ClearData();
```

Important:
This Lite version intentionally uses lightweight obfuscation and a checksum instead of strong cryptography. It is designed for fast casual-game local saves, not for protecting valuable server-authoritative economy data or secrets.

Before shipping, change the `Secret` constant in `FastSaveData.cs`.
