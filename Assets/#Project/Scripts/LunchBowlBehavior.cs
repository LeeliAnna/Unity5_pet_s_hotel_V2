using UnityEngine;

/// <summary>
/// Gère la gamelle du chien et ses croquettes.
/// Stocke la quantité de croquettes, permet leur consommation et leur remplissage.
/// Détermine automatiquement si la gamelle est utilisable (contient des croquettes).
/// </summary>
public class LunchBowlBehavior : MonoBehaviour
{
    /// <summary>Quantité maximale de croquettes que peut contenir la gamelle</summary>
    private int maxQuantity;

    /// <summary>Quantité actuelle de croquettes dans la gamelle (0 à maxQuantity)</summary>
    private int _currentQuantity;

    /// <summary>Drapeau indiquant si la gamelle est utilisable (actuellement inutilisé, redondant avec IsUsable)</summary>
    private bool _isUsable;

    /// <summary>
    /// Quantité actuelle de croquettes dans la gamelle.
    /// Toujours comprise entre 0 et maxQuantity (automatiquement clampée).
    /// </summary>
    public int CurrentQuantity
    {
        get
        {
            return _currentQuantity;
        }
        set
        {
            // Clampér la valeur entre 0 et la quantité maximale pour éviter les valeurs invalides
            _currentQuantity = Mathf.Clamp(value, 0, maxQuantity);
        }
    }

    /// <summary>
    /// Indique si la gamelle est utilisable (contient au moins une croquette).
    /// true si la gamelle contient des croquettes, false si elle est vide.
    /// Utilisé par le chien et les états pour vérifier s'il peut manger.
    /// </summary>
    public bool IsUsable => CurrentQuantity != 0;

    /// <summary>
    /// Initialise la gamelle avec une quantité maximale de croquettes.
    /// Remplit la gamelle à sa capacité maximale au démarrage.
    /// Appelée par LevelManager lors de l'initialisation du niveau.
    /// </summary>
    /// <param name="quantity">Quantité maximale de croquettes pour cette gamelle</param>
    public void Initialize(int quantity)
    {
        // Définir la capacité maximale de la gamelle
        this.maxQuantity = quantity;

        // Remplir complètement la gamelle au démarrage
        CurrentQuantity = maxQuantity;
    }

    /// <summary>
    /// Diminue la quantité de croquettes dans la gamelle.
    /// Utilisée quand le chien mange (consomme des croquettes).
    /// La quantité est automatiquement clampée à 0 si elle tente de descendre en dessous.
    /// </summary>
    /// <param name="quantity">Nombre de croquettes à consommer</param>
    public void DecreaseQuantity(int quantity)
    {
        // Soustraire les croquettes consommées (le clamp dans la propriété gère les débordements)
        CurrentQuantity -= quantity;
    }

    /// <summary>
    /// Augmente la quantité de croquettes dans la gamelle.
    /// Utilisée quand le joueur remplit la gamelle.
    /// La quantité est automatiquement limitée à maxQuantity si elle tente de dépasser.
    /// </summary>
    /// <param name="quantity">Nombre de croquettes à ajouter</param>
    public void AddQuantity(int quantity)
    {
        // Ajouter les croquettes remplies (le clamp dans la propriété gère les débordements)
        CurrentQuantity += quantity;
    }

}
