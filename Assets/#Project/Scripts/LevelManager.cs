using UnityEngine;

/// <summary>
/// Gestionnaire du niveau (environnement du jeu).
/// Gère la position du niveau, le point central (référence pour mouvements aléatoires) et la gamelle.
/// Point d'accès centralisé pour tous les éléments du niveau.
/// </summary>
public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// Point central du niveau (référence pour le mouvement aléatoire du chien).
    /// Utilisé par RandomMovement pour générer des destinations aléatoires autour de ce point.
    /// </summary>
    public Transform CenterPoint { get; private set; }

    /// <summary>
    /// Référence à la gamelle du chien.
    /// Contient les croquettes et gère leur consommation et remplissage.
    /// Trouvée automatiquement dans les enfants du niveau lors de l'initialisation.
    /// </summary>
    public LunchBowlBehavior lunchBowl;

    /// <summary>
    /// Initialise le niveau avec sa position, orientation et ses composants.
    /// Configure le point central et initialise la gamelle avec sa quantité de croquettes.
    /// Appelée par GameInitializer lors du démarrage du jeu.
    /// </summary>
    /// <param name="position">Position du niveau dans le monde</param>
    /// <param name="rotation">Orientation du niveau</param>
    /// <param name="centerPoint">Transform représentant le point central du niveau</param>
    /// <param name="lunchBowlQuantity">Quantité initiale de croquettes dans la gamelle</param>
    public void Initialize(Vector3 position, Quaternion rotation, Transform centerPoint, int lunchBowlQuantity)
    {
        // Positionner et orienter le niveau
        transform.SetPositionAndRotation(position, rotation);
        
        // Stocker la référence au point central
        CenterPoint = centerPoint;

        // Chercher automatiquement le composant LunchBowlBehavior dans les enfants du niveau
        lunchBowl = GetComponentInChildren<LunchBowlBehavior>();

        // Initialiser la gamelle avec la quantité de croquettes
        if (lunchBowl != null)
        {
            lunchBowl.Initialize(lunchBowlQuantity);
        }
    }

    /// <summary>
    /// Remplit la gamelle avec une quantité de croquettes supplémentaires.
    /// Utilisée par le joueur pour remplir la gamelle quand elle est vide.
    /// </summary>
    /// <param name="quantity">Nombre de croquettes à ajouter à la gamelle</param>
    public void FillLunchBowl(int quantity)
    {
        // Vérifier que la gamelle existe avant d'ajouter des croquettes
        if(lunchBowl != null)
        {
            lunchBowl.AddQuantity(quantity);
        }
    }

}
