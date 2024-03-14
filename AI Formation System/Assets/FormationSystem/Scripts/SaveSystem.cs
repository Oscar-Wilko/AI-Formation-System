using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class SaveSystem
{
    public static int saveIndex = -1;
    #region Saving
    /// <summary>
    /// Save image information with file name
    /// </summary>
    /// <param name="info">SavedImage of image information</param>
    /// <param name="file_name">String of filename to save with</param>
    public static void SaveInfo(FullFormation info, string file_name)
    {
        FolderCheck();
        string saved_data = JsonUtility.ToJson(info, true);
        File.WriteAllText(GetSaveFileLocation(file_name), saved_data);
    }

    public static void SaveInfo(FullFormation info, int index) => SaveInfo(info, IndexToName(index));
    #endregion
    #region Loading
    /// <summary>
    /// Load image information from file name
    /// </summary>
    /// <param name="file_name">String of file name</param>
    /// <returns>SavedImage of exported image information</returns>
    public static FullFormation LoadInfo(string file_name)
    {
        FolderCheck();
        if (File.Exists(GetSaveFileLocation(file_name)))
        {
            string loaded_data = File.ReadAllText(GetSaveFileLocation(file_name));
            FullFormation data = JsonUtility.FromJson<FullFormation>(loaded_data);
            if (data != null)
                return data;
        }
        return null;
    }

    public static FullFormation LoadInfo(int index)
    {
        string[] files = Directory.GetFiles(GetSavedDataLocation());
        int i_count = 0;
        for (int i = 0; i < files.Length; i ++)
        {
            if (files[i].Contains("meta"))
                continue;
            if (files[i].Contains("json"))
            {
                if (i_count == index)
                    return LoadInfo(IndexToName(FileNameToIndex(files[i])));
                i_count++;
            }
        }
        return null;
    }
    #endregion
    #region Folder & File Checking
    /// <summary>
    /// Get File Location Of Saved Information
    /// </summary>
    /// <returns>String of file location</returns>
    private static string GetSaveFileLocation(string file_name)
    {
        return GetSavedDataLocation() + "/" + file_name + ".json";
    }

    /// <summary>
    /// Get File Location Of SavedData Folder
    /// </summary>
    /// <returns>String of file location</returns>
    private static string GetSavedDataLocation()
    {
        return "Assets/FormationSystem/Saves";
    }

    /// <summary>
    /// Checks if save folder exists, if it doesn't, then it creates it
    /// </summary>
    private static void FolderCheck()
    {
        if (!Directory.Exists(GetSavedDataLocation()))
            Directory.CreateDirectory(GetSavedDataLocation());
    }

    private static string IndexToName(int index)
    {
        return "Save" + index;
    }

    public static int SaveCount()
    {
        return Directory.GetFiles(GetSavedDataLocation(), "Save*json").Length;
    }

    public static int NextAvailabeSaveIndex()
    {
        string[] files = Directory.GetFiles(GetSavedDataLocation());
        int save_index = 1;
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Contains("meta"))
                continue;
            int file_index = FileNameToIndex(files[i]);
            if (file_index >= save_index) save_index = file_index + 1;
        }
        return save_index;
    }

    public static int FileNameToIndex(string name)
    {
        string tname = name;
        name = name.Replace(".json", "");
        name = name.Replace("\\Save", "");
        name = name.Replace(GetSavedDataLocation(), "");
        return int.Parse(name);
    }
    #endregion
}