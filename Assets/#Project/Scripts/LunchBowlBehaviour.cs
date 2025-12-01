using UnityEngine;

[RequireComponent(typeof(Collider))]
/// <summary>
/// Gere la gamelle du chien et ses croquettes.
/// Stocke la quantite de croquettes, permet leur consommation et leur remplissage.
/// Determine automatiquement si la gamelle est utilisable (contient des croquettes).
/// </summary>
public class LunchBowlBehaviour : MonoBehaviour, IInteractable
{
    /// <summary>Quantite maximale de croquettes que peut contenir la gamelle</summary>
    private int maxQuantity;

    /// <summary>Quantite actuelle de croquettes dans la gamelle (0 a maxQuantity)</summary>
    private int _currentQuantity;

    /// <summary>
    /// Quantite actuelle de croquettes dans la gamelle.
    /// Toujours comprise entre 0 et maxQuantity (automatiquement clampee).
    /// </summary>
    public int CurrentQuantity
    {
        get
        {
            return _currentQuantity;
        }
        set
        {
            // Clamper la valeur entre 0 et la quantite maximale pour eviter les valeurs invalides
            _currentQuantity = Mathf.Clamp(value, 0, maxQuantity);
        }
    }

    /// <summary>
    /// Indique si la gamelle est utilisable (contient au moins une croquette).
    /// true si la gamelle contient des croquettes, false si elle est vide.
    /// Utilise par le chien et les etats pour verifier s'il peut manger.
    /// </summary>
    public bool IsUsable => CurrentQuantity != 0;

    /// <summary>
    /// Initialise la gamelle avec une quantite maximale de croquettes.
    /// Remplit la gamelle a sa capacite maximale au demarrage.
    /// Appelee par LevelManager lors de l'initialisation du niveau.
    /// </summary>
    /// <param name="quantity">Quantite maximale de croquettes pour cette gamelle</param>
    public void Initialize(int quantity)
    {
        // Definir la capacite maximale de la gamelle
        this.maxQuantity = quantity;

        // Remplir completement la gamelle au demarrage
        CurrentQuantity = maxQuantity;
    }

    /// <summary>
    /// Diminue la quantite de croquettes dans la gamelle.
    /// Utilisee quand le chien mange (consomme des croquettes).
    /// La quantite est automatiquement clampee a 0 si elle tente de descendre en dessous.
    /// </summary>
    /// <param name="quantity">Nombre de croquettes a consommer</param>
    public void DecreaseQuantity(int quantity)
    {
        // Soustraire les croquettes consommees (le clamp dans la propriete gere les debordements)
        CurrentQuantity -= quantity;
    }

    /// <summary>
    /// Augmente la quantite de croquettes dans la gamelle.
    /// Utilisee quand le joueur remplit la gamelle.
    /// La quantite est automatiquement limitee a maxQuantity si elle tente de depasser.
    /// </summary>
    /// <param name="quantity">Nombre de croquettes a ajouter</param>
    public void AddQuantity(int quantity)
    {
        // Ajouter les croquettes remplies (le clamp dans la propriete gere les debordements)
        CurrentQuantity += quantity;
    }

    /// <summary>
    /// Appeler quand on click sur l'objet
    /// </summary>
    public void Interact()
    {
        AddQuantity(maxQuantity);
        Debug.Log($"[Lunchbowl] Quantité {CurrentQuantity}, la gamelle est utilisable {IsUsable}");
    }
}
