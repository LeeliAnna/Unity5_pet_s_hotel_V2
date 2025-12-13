using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelBoundsProvider))]
/// <summary>
/// Gestionnaire du niveau (environnement du jeu).
/// Gere la position du niveau, le point central (reference pour mouvements aleatoires) et la gamelle.
/// Point d'acces centralise pour tous les elements du niveau.
/// </summary>
public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// Point central du niveau (reference pour le mouvement aleatoire du chien).
    /// Utilise par RandomMovement pour generer des destinations aleatoires autour de ce point.
    /// </summary>
    public Transform CenterPoint { get; private set; }
    public LevelBoundsProvider LevelBoundsProvider {get; private set;}
    public Collider boundsCollider;

    /// <summary>
    /// Reference a la gamelle du chien.
    /// Contient les croquettes et gere leur consommation et remplissage.
    /// Trouvee automatiquement dans les enfants du niveau lors de l'initialisation.
    /// </summary>
    public BowlBehaviour lunchBowl;
    public List<BowlBehaviour> bowls = new List<BowlBehaviour>();
    public BallBehaviour ball;
    public List<BallBehaviour> balls = new List<BallBehaviour>();

    /// <summary>
    /// Initialise le niveau avec sa position, orientation et ses composants.
    /// Configure le point central et initialise la gamelle avec sa quantite de croquettes.
    /// Appelee par GameInitializer lors du demarrage du jeu.
    /// </summary>
    /// <param name="position">Position du niveau dans le monde</param>
    /// <param name="rotation">Orientation du niveau</param>
    /// <param name="centerPoint">Transform representant le point central du niveau</param>
    /// <param name="lunchBowlQuantity">Quantite initiale de croquettes dans la gamelle</param>
    public void Initialize(Vector3 position, Quaternion rotation, Transform centerPoint, float lunchBowlQuantity)
    {
        // Positionner et orienter le niveau
        transform.SetPositionAndRotation(position, rotation);

        LevelBoundsProvider = GetComponent<LevelBoundsProvider>();

        if(boundsCollider == null) Debug.LogError("[LevelManager] Aucun BoxCollider trouvé dans l'enfant du level.");
        else LevelBoundsProvider.Initialize(boundsCollider);

        
        // Stocker la reference au point central
        CenterPoint = centerPoint;

        // Chercher automatiquement le composant LunchBowlBehavior dans les enfants du niveau
        lunchBowl = GetComponentInChildren<BowlBehaviour>();
        ball = GetComponentInChildren<BallBehaviour>();

        // Initialiser la gamelle avec la quantite de croquettes
        if (lunchBowl != null)
        {
            lunchBowl.Initialize(lunchBowlQuantity);
        }
    }

    /// <summary>
    /// Remplit la gamelle avec une quantite de croquettes supplementaires.
    /// Utilisee par le joueur pour remplir la gamelle quand elle est vide.
    /// </summary>
    /// <param name="quantity">Nombre de croquettes a ajouter a la gamelle</param>
    public void FillLunchBowl(int quantity)
    {
        // Verifier que la gamelle existe avant d'ajouter des croquettes
        if(lunchBowl != null)
        {
            lunchBowl.AddQuantity(quantity);
        }
    }

    /// <summary>
    /// Tick central du niveau, appelé par le GameManager.
    /// Regroupe la mise à jour des objets physiques et interactifs du level.
    /// </summary>
    public void Process()
    {
        // Physique de la balle (ralentissement/arrêt/clamp)
        if (ball != null)
        {
            // Méthode dédiée dans BallBehaviour pour la physique par tick
            ball.TickPhysics();
        }
    }

    public void Register<T>(List<T> liste, T item)
    {
        if (!liste.Contains(item))
        {
            liste.Add(item);
        }
    }
}