using System;
using UnityEngine;

/// <summary>
/// Gère l'interaction au clic sur un chien.
/// Détecte les clics et transmet les informations au système UI via un événement.
/// Script simplifié qui utilise les méthodes natives Unity (OnMouseDown).
/// </summary>
[RequireComponent(typeof(Collider))]
public class InteractionChien : MonoBehaviour
{
    #region Événements
    
    /// <summary>
    /// Événement déclenché quand le joueur clique sur ce chien.
    /// Paramètre : ce composant InteractionChien pour accéder aux données
    /// </summary>
    public event Action<InteractionChien> OnClicked;
    
    #endregion

    #region Références
    
    /// <summary>Référence au contrôleur de besoins du chien (pour accéder aux données)</summary>
    private ControleurBesoinsChien needController;
    
    /// <summary>Référence au DogBehaviour parent (pour compatibilité)</summary>
    private DogBehaviour dogBehaviour;
    
    #endregion

    #region Propriétés Publiques
    
    /// <summary>
    /// Accès au contrôleur de besoins pour récupérer les données UI.
    /// </summary>
    public ControleurBesoinsChien NeedController => needController;
    
    /// <summary>
    /// Accès au comportement du chien pour compatibilité avec le système existant.
    /// </summary>
    public DogBehaviour DogBehaviour => dogBehaviour;
    
    /// <summary>
    /// Nom du chien pour l'affichage (sans le "(Clone)").
    /// </summary>
    public string DisplayName => gameObject.name.Replace("(Clone)", "").Trim();
    
    #endregion

    #region Méthodes Unity
    
    /// <summary>
    /// Initialisation au réveil du composant.
    /// Récupère les références aux composants nécessaires.
    /// </summary>
    private void Awake()
    {
        // Récupérer le contrôleur de besoins (peut être sur le même objet ou un enfant)
        needController = GetComponent<ControleurBesoinsChien>();
        
        // Récupérer le DogBehaviour pour compatibilité
        dogBehaviour = GetComponent<DogBehaviour>();
        
        // Vérifier que le collider est configuré comme trigger ou non (selon le besoin)
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogWarning($"[InteractionChien] Aucun Collider trouvé sur {gameObject.name}. " +
                           "Un Collider est requis pour détecter les clics.");
        }
    }
    
    /// <summary>
    /// Appelé par Unity quand le joueur clique sur le collider de cet objet.
    /// Déclenche l'événement OnClicked et notifie le DogBehaviour si présent.
    /// </summary>
    private void OnMouseDown()
    {
        // Émettre l'événement pour les nouveaux systèmes
        OnClicked?.Invoke(this);
        
        // Compatibilité : appeler Select() sur DogBehaviour si présent
        if (dogBehaviour != null)
        {
            dogBehaviour.Select();
        }
        
        Debug.Log($"[InteractionChien] Clic détecté sur {DisplayName}");
    }
    
    #endregion

    #region Méthodes d'Initialisation (pour compatibilité)
    
    /// <summary>
    /// Initialise manuellement les références si nécessaire.
    /// Utile si les composants ne sont pas sur le même GameObject.
    /// </summary>
    /// <param name="controller">Contrôleur de besoins à utiliser</param>
    /// <param name="behaviour">Comportement du chien (optionnel)</param>
    public void Initialize(ControleurBesoinsChien controller, DogBehaviour behaviour = null)
    {
        needController = controller;
        dogBehaviour = behaviour;
    }
    
    #endregion

    #region Méthodes Publiques
    
    /// <summary>
    /// Déclenche manuellement l'événement de clic.
    /// Utile pour les tests ou pour déclencher depuis un autre système.
    /// </summary>
    public void TriggerClick()
    {
        OnMouseDown();
    }
    
    #endregion
}
