using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Données de configurations de la penion :
/// - valeurs de depart : argent, prestige, etc
/// - liste de noms possible pour le bouton aleatoire
/// </summary>
[CreateAssetMenu(fileName = "Pension Settings", menuName = "Game/Pension Settings")]
public class PensionSettings : ScriptableObject
{
    [Header("Valeurs de départ")]
    public int startingMoney = 1000;
    public int startingPrestige = 0;

    [Header("Noms possible pour la pension")]
    public List<string> randomNames = new()
    {
        "La Patte Heureuse",
        "Le Refuge Doux",
        "Chez Toutou",
        "La Maison des Museaux",
        "La Pension des Copains",
        "La Cabane à Wouf",
        "Le Panier Douillet",
        "Le Nid à Chiens",
        "Les Pattes Joyeuses",
        "La Pension Bisous & Croquettes",
        "La Meute Joyeuse",
        "Le Royaume des Toutous",
        "La Tanière des Amis",
        "Le Chalet des Chiens Heureux",
        "Le Domaine Canin",
        "Le Manoir des Pattes",
        "Le Palais des Chiens",
        "La Pension Élégance",
        "Le Grand Refuge",
        "Les Suites du Museau",
        "Le Pavillon Canin",
        "La Villa des Pattes Dorées",
        "Le Loft des Loulous",
        "Croc & Relax",
        "La Pension Wouf-Wouf",
        "Le Spa-Toutou",
        "Le Woof Palace",
        "Les Boubous en Vacances",
        "Les Croquettes & Co",
        "Le Wouf Resort",
        "La Pension Patator",
        "Le Club des Léchouilles",
        "Bark & Breakfast",
        "Les Pattes Sauvages",
        "Le Bois des Chiens Libres",
        "Le Refuge des Montagnes",
        "Le Camp des Pattes Folles",
        "La Prairie Canine",
        "Le Sentier des Loupiots",
        "L’Oasis des Quatre Pattes",
        "Pet’s Hotel",
        "Paw’s Lodge",
        "The Doggy Retreat",
        "The Happy Tails Inn",
        "Pension Manager",
        "Le Centre Canin Tycoon",
        "L’Hôtel des Pattes VIP",
        "DoggyHub",
        "Pet’s Paradise",
        "Museau & Prestige",
        "Patte de Luxe",
        "Le Grand Wouf Hôtel",
        "La Pension des Petits Seigneurs",
        "Le Museau Royal",
        "Le Nid des Étoiles Canines",
        "Le Sommet des Pattes",
        "La Pension Diamant",
        "Le Jardin des Loulous",
        "Le Relais des Huskies",
    };
}
