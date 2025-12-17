using System;
using UnityEngine;
using UnityEngine.UI;

public class DogFillBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;
    
    [Header("Configuration du besoin")]
    [Tooltip("Type du besoin à surveiller")]
    [SerializeField] private NeedType needType = NeedType.Faim;

    private DogBehaviour dog;
    private NeedBase trackedNeed;
    private Action<float, float> needChangeCallback;

    public void Initialize(DogBehaviour dogBehaviour)
    {
        dog = dogBehaviour;
        
        if (dog?.needController == null) return;

        // Trouver le besoin correspondant dans la liste via l'enum
        trackedNeed = dog.needController.needs.Find(need => need.Type == needType);

        if (trackedNeed == null)
        {
            Debug.LogWarning($"[DogFillBar] Besoin '{needType}' introuvable dans needController.needs");
            return;
        }

        // S'abonner à l'événement correspondant selon le type de besoin
        SubscribeToNeedEvent();
        
        // Initialiser la barre avec la valeur actuelle
        UpdateBar(trackedNeed.NeedValue, trackedNeed.MaxValue);
    }

    private void SubscribeToNeedEvent()
    {
        // Créer le callback pour être réutilisable dans OnDestroy
        needChangeCallback = OnNeedChanged;

        // S'abonner à l'événement correspondant au type de besoin
        switch (needType)
        {
            case NeedType.Faim:
                dog.OnHungerChanged += needChangeCallback;
                break;
            case NeedType.Soif:
                dog.OnThirstynessChanged += needChangeCallback;
                break;
            // Ajouter ici d'autres besoins au fur et à mesure
            default:
                Debug.LogWarning($"[DogFillBar] Pas d'événement configuré pour '{needType}'");
                break;
        }
    }

    private void UnsubscribeFromNeedEvent()
    {
        if (dog == null || needChangeCallback == null) return;

        switch (needType)
        {
            case NeedType.Faim:
                dog.OnHungerChanged -= needChangeCallback;
                break;
            case NeedType.Soif:
                dog.OnThirstynessChanged -= needChangeCallback;
                break;
            // Ajouter ici d'autres besoins au fur et à mesure
        }
    }

    /// <summary>
    /// Appelé automatiquement quand le besoin surveillé change via l'événement
    /// </summary>
    private void OnNeedChanged(float current, float max)
    {
        UpdateBar(current, max);
    }

    private void UpdateBar(float current, float max)
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
