using System;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Collider))]
/// <summary>
/// Gere la gamelle du chien et ses croquettes.
/// Stocke la quantite de croquettes, permet leur consommation et leur remplissage.
/// Determine automatiquement si la gamelle est utilisable (contient des croquettes).
/// </summary>
public class BowlBehaviour : MonoBehaviour, IInteractable
{
    /// <summary>Quantite maximale de croquettes que peut contenir la gamelle</summary>
    private float maxQuantity;
    private BowlFillBar bowlFillBar;
    private GameObject contentVisual;

    /// <summary>Quantite actuelle de croquettes dans la gamelle (0 a maxQuantity)</summary>
    private float _currentQuantity;

    /// <summary>
    /// Quantite actuelle de croquettes dans la gamelle.
    /// Toujours comprise entre 0 et maxQuantity (automatiquement clampee).
    /// </summary>
    public float CurrentQuantity
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
    /// Evenement declenche quand la quantite de croquettes change.
    /// Parametres : (float CurrentQuantite, float maxQuantite)
    /// </summary>
    public event Action<float, float> OnQuantityChanged;

    /// <summary>
    /// Initialise la gamelle avec une quantite maximale de croquettes.
    /// Remplit la gamelle a sa capacite maximale au demarrage.
    /// Appelee par LevelManager lors de l'initialisation du niveau.
    /// </summary>
    /// <param name="quantity">Quantite maximale de croquettes pour cette gamelle</param>
    public void Initialize(float quantity)
    {
        // Definir la capacite maximale de la gamelle
        this.maxQuantity = quantity;

        // Remplir completement la gamelle au demarrage
        CurrentQuantity = maxQuantity;

        // Initialiser la barre de remplissage si presente
        bowlFillBar = GetComponentInChildren<BowlFillBar>();
        if(bowlFillBar != null)
            bowlFillBar.Initialize(this);

        contentVisual = transform.Find("Content").gameObject;
        
        // Notifier les ecouteurs de la quantite initiale
        OnQuantityChanged?.Invoke(CurrentQuantity, maxQuantity);
        OnQuantityChanged += (current, max) => UpdateFoodVisual();
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
        OnQuantityChanged?.Invoke(CurrentQuantity, maxQuantity);
    }

    /// <summary>
    /// Augmente la quantite de croquettes dans la gamelle.
    /// Utilisee quand le joueur remplit la gamelle.
    /// La quantite est automatiquement limitee a maxQuantity si elle tente de depasser.
    /// </summary>
    /// <param name="quantity">Nombre de croquettes a ajouter</param>
    public void AddQuantity(float quantity)
    {
        // Ajouter les croquettes remplies (le clamp dans la propriete gere les debordements)
        CurrentQuantity += quantity;
        OnQuantityChanged?.Invoke(CurrentQuantity, maxQuantity);
    }

    /// <summary>
    /// Appeler quand on click sur l'objet
    /// </summary>
    public void Interact()
    {
        AddQuantity(maxQuantity);
        Debug.Log($"[Lunchbowl] Quantité {CurrentQuantity}, la gamelle est utilisable {IsUsable}");
    }

    private void UpdateFoodVisual()
    {
        if (contentVisual != null)
        {
            float fillRatio = CurrentQuantity / maxQuantity;

            if(CurrentQuantity <= 0)
            {
                contentVisual.SetActive(false);
                return;
            }

            contentVisual.SetActive(true);

            float minY = -0.05f; // Position Y lorsque la gamelle est vide
            float maxY = 0f; // Position Y lorsque la gamelle est pleine

            Vector3 position = contentVisual.transform.localPosition;
            position.y = Mathf.Lerp(minY, maxY, fillRatio);
            contentVisual.transform.localPosition = position;

        }
    }
}
