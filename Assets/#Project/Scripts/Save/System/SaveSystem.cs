using System;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    public const int MAX_SLOTS = 3;
    private const string FILE_PATTERN = "save_slot_{0}.json";

    private static string GetSavePath(int slotIndex)
    {
        string fileName = string.Format(FILE_PATTERN, slotIndex);
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    /// <summary>
    /// Ecrit un JSON
    /// </summary>
    /// <param name="data"></param>
    public static void Save(SaveGameData data, int slotIndex)
    {
        if(slotIndex < 0 || slotIndex >= MAX_SLOTS)
        {
            Debug.LogError($"[SaveSystem] Slot invalide : {slotIndex}");
            return;
        }
        if (data == null)
        {
            Debug.LogError("[SaveSystem] Impossible de sauvegarder : data est null");
            return;
        }

        string path = GetSavePath(slotIndex);
        string jason = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, jason);

        Debug.Log($"[SaveSystem] Sauvegarde écrite : {path}");
    }

    /// <summary>
    /// Essaye de charger un fichier
    /// </summary>
    /// <param name="data">Donnee de sauvegardes</param>
    /// <returns>True si le fichier existe / False si le fichier n'existe pas ou si une erreur se produit</returns>
    public static bool TryLoad(int slotIndex, out SaveGameData data)
    {
        data = null;

        if(slotIndex < 0 || slotIndex >= MAX_SLOTS)
        {
            Debug.LogError($"[SaveSystem] Slot invalide : {slotIndex}");
            return false;
        }

        string path = GetSavePath(slotIndex);
        if (!SaveExists(slotIndex))
        {
            Debug.Log("[SaveSystem] Aucun fichier de sauvegarde trouvé");
            return false;
        }

        try
        {
            string jason = File.ReadAllText(path);
            data = JsonUtility.FromJson<SaveGameData>(jason);

            if(data == null)
            {
                Debug.LogError("[SaveSystem] Echec de désérialisation JSON");
                return false;
            }

            Debug.Log("[SaveSystem] Sauvegarde chargée avec succès");
            return true;

        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[SaveSystem] Erreur lors du chargement : {ex}");
            return false;
        }
    }

    /// <summary>
    /// Verifie si la sauvegarde existe
    /// </summary>
    /// <returns>True si existe / false si n'existe pas</returns>
    public static bool SaveExists(int slotIndex)
    {
        string path = GetSavePath(slotIndex);
        return File.Exists(path);
    }

    /// <summary>
    /// Supprime la sauvegarde
    /// </summary>
    public static void DeleteSave(int slotIndex)
    {
        string path = GetSavePath(slotIndex);
        if (SaveExists(slotIndex))
        {
            File.Delete(path);
            Debug.Log($"[SaveSystem] Slot {slotIndex} supprimé : {path}");
        }
        else Debug.Log($"[SaveSystem] Aucun fichier à supprimer pour le slot {slotIndex}");
    }

    /// <summary>
    /// Cherche si il existe au moins une sauvegarde
    /// </summary>
    /// <returns>True si oui / false si non</returns>
    public static bool AnySaveExists()
    {
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            if(SaveExists(i)) return true;
        }
        return false;
    }

    /// <summary>
    /// Trouve le slot contenant la plus recente sauvegarde (par la date de modification du fichier.)
    /// </summary>
    /// <param name="lastSlotInt">renvoie le numero de la derniere sauvegarde trouvee ou -1 si aucune</param>
    /// <returns> true si trouve / false si non</returns>
    public static bool TryGetLastSaveSlot(out int lastSlotInt)
    {
        lastSlotInt = -1;
        DateTime lastWriteTime = DateTime.MinValue;

        for(int i = 0; i < MAX_SLOTS; i++)
        {
            string path = GetSavePath(i);
            if(!File.Exists(path)) continue;

            DateTime writeTime = File.GetLastWriteTime(path);
            if(writeTime > lastWriteTime)
            {
                lastWriteTime = writeTime;
                lastSlotInt = i;
            }
        }
        return lastSlotInt != -1;
    }

}
