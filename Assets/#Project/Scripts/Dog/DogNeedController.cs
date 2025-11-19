using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Gère tous les besoins du chien (faim, etc.).
/// Orchestre la mise à jour des besoins, l'identification du besoin urgent et le système de priorités.
/// </summary>
public class DogNeedController : MonoBehaviour
{
    /// <summary>Liste de tous les besoins du chien (extensible pour ajouter sommeil, jeu, etc.)</summary>
    public List<NeedBase> needs { get; private set; }

    /// <summary>Référence directe au besoin de faim pour un accès rapide</summary>
    public HungerNeed HungerNeed { get; private set; }

    /// <summary>
    /// Indique si le chien a faim de manière critique.
    /// true si le besoin de faim atteint son seuil critique, false sinon.
    /// </summary>
    public bool IsHungry => HungerNeed != null && HungerNeed.IsCritical;

    /// <summary>
    /// Initialise le contrôleur de besoins avec les configurations nécessaires.
    /// Crée le besoin de faim et l'ajoute à la liste des besoins.
    /// </summary>
    /// <param name="hungerConfig">Asset de configuration de la faim</param>
    public void Initialize(HungerConfig hungerConfig)
    {
        // Créer la liste des besoins
        needs = new();

        // Créer le besoin de faim avec sa configuration
        HungerNeed = new(hungerConfig);

        // Ajouter la faim à la liste des besoins (prêt pour extension avec d'autres besoins)
        needs.Add(HungerNeed);
    }

    /// <summary>
    /// Met à jour tous les besoins du chien chaque frame.
    /// Chaque besoin diminue progressivement et peut atteindre un état critique.
    /// </summary>
    public void AllProcess()
    {
        // Traiter chaque besoin (diminuer sa valeur, vérifier les seuils)
        foreach (NeedBase need in needs)
        {
            need.Process();
        }
    }

    /// <summary>
    /// Identifie le besoin le plus urgent (celui avec la plus haute priorité).
    /// Utile pour déterminer le comportement prioritaire du chien.
    /// </summary>
    /// <returns>Le besoin avec la priorité la plus élevée, ou null si la liste est vide</returns>
    public NeedBase GetMostUrgent()
    {
        // Trier les besoins par priorité décroissante et retourner le premier
        return needs.OrderByDescending(n => n.Priority).FirstOrDefault();
    }

    /// <summary>
    /// Vérifie si un besoin d'un type spécifique existe dans la liste.
    /// Utile pour vérifier la présence d'un besoin sans l'obtenir.
    /// </summary>
    /// <typeparam name="T">Type du besoin à chercher (ex : HungerNeed)</typeparam>
    /// <returns>true si un besoin du type T existe, false sinon</returns>
    public bool NeedIsPresent<T>()
    {
        // Parcourir la liste et chercher un besoin du type T
        for (int i = 0; i < needs.Count; i++)
        {
            if (needs[i] is T)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Cherche et retourne un besoin spécifique de la liste.
    /// Comparaison par référence (instance exacte).
    /// </summary>
    /// <param name="need">Le besoin à chercher</param>
    /// <returns>Le besoin trouvé, ou null si absent ou si le paramètre est null</returns>
    public NeedBase FindNeed(NeedBase need)
    {
        // Vérifier que le paramètre n'est pas null
        if (need == null) return null;

        // Chercher la même instance dans la liste
        return needs.FirstOrDefault(n => n == need);
    }
}

