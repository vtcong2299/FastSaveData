#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public static class FastSaveDataEditor
{
    [MenuItem("Window/Fast Save Data/Open Save Folder")]
    private static void OpenFolder()
    {
        var path = FastSaveData.FilePath;
        var folder = Path.GetDirectoryName(path);
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        EditorUtility.RevealInFinder(folder);
    }

    [MenuItem("Window/Fast Save Data/Clear Data")]
    private static void ClearData()
    {
        if (!EditorUtility.DisplayDialog("Clear FastSaveData", "Delete all FastSaveData files and reset the local save?", "Clear", "Cancel")) return;
        FastSaveData.ClearData();
        Debug.Log("[FastSaveData] Save data cleared.");
    }

    [MenuItem("Window/Fast Save Data/Clear Data", true)]
    private static bool ValidateClearData()
    {
        return !EditorApplication.isPlaying;
    }
}
#endif
