using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private const string SAVE_FILE_NAME = "savegame.json";

    private static string SavePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

    /// <summary>
    /// Ecrit un JSON
    /// </summary>
    /// <param name="data"></param>
    public static void Save(SaveGameData data)
    {
        if (data == null)
        {
            Debug.LogError("[SaveSystem] Impossible de sauvegarder : data est null");
            return;
        }

        string jason = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, jason);

        Debug.Log($"[SaveSystem] Sauvegarde écrite : {SavePath}");
    }

    /// <summary>
    /// Essaye de charger un fichier
    /// </summary>
    /// <param name="data">Donnee de sauvegardes</param>
    /// <returns>True si le fichier existe / False si le fichier n'existe pas ou si une erreur se produit</returns>
    public static bool TryLoad(out SaveGameData data)
    {
        data = null;

        if (!SaveExists())
        {
            Debug.Log("[SaveSystem] Aucun fichier de sauvegarde trouvé");
            return false;
        }

        try
        {
            string jason = File.ReadAllText(SavePath);
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
    public static bool SaveExists()
    {
        return File.Exists(SavePath);
    }

    /// <summary>
    /// Supprime la sauvegarde
    /// </summary>
    public static void DeleteSave()
    {
        if (SaveExists())
        {
            File.Delete(SavePath);
            Debug.Log("[SaveSystem] Sauvegarde supprimée");
        }
    }

}
