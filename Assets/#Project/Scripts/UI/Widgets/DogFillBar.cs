using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Barre de remplissage pour afficher un besoin spécifique d'un chien.
/// S'abonne aux événements du ControleurBesoinsChien pour les mises à jour en temps réel.
/// </summary>
public class DogFillBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;
    
    [Header("Configuration du besoin")]
    [Tooltip("Type du besoin à surveiller")]
    [SerializeField] private NeedType needType = NeedType.Faim;

    private DogBehaviour dog;
    private ControleurBesoinsChien needController;
    private NeedBase trackedNeed;
    private Action<NeedType, float, bool> needChangeCallback;

    /// <summary>
    /// Initialise la barre avec un DogBehaviour (compatibilité).
    /// </summary>
    public void Initialize(DogBehaviour dogBehaviour)
    {
        dog = dogBehaviour;
        
        if (dog == null) return;

        // Récupérer le nouveau ControleurBesoinsChien
        needController = dog.GetComponent<ControleurBesoinsChien>();
        
        if (needController == null)
        {
            Debug.LogWarning($"[DogFillBar] ControleurBesoinsChien introuvable sur {dog.name}");
            return;
        }

        InitializeWithController(needController);
    }

    /// <summary>
    /// Initialise la barre directement avec un ControleurBesoinsChien.
    /// </summary>
    public void InitializeWithController(ControleurBesoinsChien controller)
    {
        needController = controller;
        
        if (needController?.needs == null) return;

        // Trouver le besoin correspondant dans la liste via l'enum
        trackedNeed = needController.needs.Find(need => need.Type == needType);

        if (trackedNeed == null)
        {
            Debug.LogWarning($"[DogFillBar] Besoin '{needType}' introuvable dans needController.needs");
            return;
        }

        // S'abonner à l'événement du nouveau système
        SubscribeToNeedEvent();
        
        // Initialiser la barre avec la valeur actuelle
        UpdateBar(trackedNeed.NeedValue, trackedNeed.MaxValue);
    }

    private void SubscribeToNeedEvent()
    {
        if (needController == null) return;
        
        // Créer le callback pour le nouveau système
        needChangeCallback = OnNeedChanged;
        needController.OnNeedChanged += needChangeCallback;
    }

    private void UnsubscribeFromNeedEvent()
    {
        if (needController == null || needChangeCallback == null) return;
        
        needController.OnNeedChanged -= needChangeCallback;
        needChangeCallback = null;
    }

    /// <summary>
    /// Appelé automatiquement quand un besoin change via l'événement du ControleurBesoinsChien.
    /// Filtre pour ne mettre à jour que si c'est le bon type de besoin.
    /// </summary>
    private void OnNeedChanged(NeedType type, float normalizedValue, bool isCritical)
    {
        // Ne traiter que le besoin surveillé
        if (type != needType) return;
        
        // Convertir la valeur normalisée en valeur absolue pour le slider
        if (trackedNeed != null)
        {
            // float currentValue = normalizedValue * trackedNeed.MaxValue;
            float currentValue = trackedNeed.NeedValue;
            UpdateBar(currentValue, trackedNeed.MaxValue);
        }
    }

    public void UpdateBar(float current, float max)
    {
        // Mettre à jour la valeur du slider
        if (slider != null)
        {
            slider.maxValue = max;
            slider.value = current;
        }

        // Mettre à jour la couleur
        UpdateFillColor(current, max);
    }

    private void UpdateFillColor(float current, float max)
    {
        if (fillImage != null && max > 0)
        {
            float ratio = current / max;
            fillImage.color = Color.Lerp(Color.red, Color.green, ratio);
        }
    }

    private void OnDestroy()
    {
        // Se désabonner pour éviter les memory leaks
        UnsubscribeFromNeedEvent();
    }
}
