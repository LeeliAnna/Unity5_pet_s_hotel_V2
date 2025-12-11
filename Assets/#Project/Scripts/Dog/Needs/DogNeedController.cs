using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Gere tous les besoins du chien (faim, etc.).
/// Orchestre la mise e jour des besoins, l'identification du besoin urgent et le systeme de priorites.
/// </summary>
public class DogNeedController : MonoBehaviour
{
    /// <summary>Liste de tous les besoins du chien (extensible pour ajouter sommeil, jeu, etc.)</summary>
    public List<NeedBase> needs { get; private set; }

    /// <summary>Reference directe au besoin de faim pour un acces rapide</summary>
    public HungerNeed HungerNeed { get; private set; }

    /// <summary>
    /// Indique si le chien a faim de maniere critique.
    /// true si le besoin de faim atteint son seuil critique, false sinon.
    /// </summary>
    public bool IsHungry => HungerNeed != null && HungerNeed.IsCritical;

    /// <summary>
    /// Initialise le controleur de besoins avec les configurations necessaires.
    /// Cree le besoin de faim et l'ajoute a la liste des besoins.
    /// </summary>
    /// <param name="hungerConfig">Asset de configuration de la faim</param>
    public void Initialize(HungerConfig hungerConfig)
    {
        // Creer la liste des besoins
        needs = new();

        // Creer le besoin de faim avec sa configuration
        HungerNeed = new(hungerConfig);

        // Ajouter la faim à la liste des besoins (pret pour extension avec d'autres besoins)
        needs.Add(HungerNeed);
    }

    /// <summary>
    /// Met a jour tous les besoins du chien chaque frame.
    /// Chaque besoin diminue progressivement et peut atteindre un etat critique.
    /// </summary>
    public void AllProcess()
    {
        // Traiter chaque besoin (diminuer sa valeur, verifier les seuils)
        foreach (NeedBase need in needs)
        {
            need.Process();
        }
    }

    /// <summary>
    /// Identifie le besoin le plus urgent (celui avec la plus haute priorit�).
    /// Utile pour determiner le comportement prioritaire du chien.
    /// </summary>
    /// <returns>Le besoin avec la priorite la plus elevee, ou null si la liste est vide</returns>
    public NeedBase GetMostUrgent()
    {
        // Trier les besoins par priorite decroissante et retourner le premier
        return needs.OrderByDescending(n => n.Priority).FirstOrDefault();
    }

    /// <summary>
    /// Verifie si un besoin d'un type sp�cifique existe dans la liste.
    /// Utile pour verifier la presence d'un besoin sans l'obtenir.
    /// </summary>
    /// <typeparam name="T">Type du besoin a chercher (ex : HungerNeed)</typeparam>
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
    /// Cherche et retourne un besoin specifique de la liste.
    /// Comparaison par reference (instance exacte).
    /// </summary>
    /// <param name="need">Le besoin a chercher</param>
    /// <returns>Le besoin trouve, ou null si absent ou si le parametre est null</returns>
    public NeedBase FindNeed(NeedBase need)
    {
        // Verifier que le parametre n'est pas null
        if (need == null) return null;

        // Chercher la meme instance dans la liste
        return needs.FirstOrDefault(n => n == need);
    }
}

