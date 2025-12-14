using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Popup d'information sur un chien.
/// Affiche les besoins (faim, soif, satisfaction globale) et se met à jour en temps réel.
/// </summary>
public class DogPopupInfo : MonoBehaviour
{
    public GameManager GameManager { get; private set; }
    private DogBehaviour dog;
    
    // Callbacks pour pouvoir se désabonner proprement
    public Action<float, float> hungerCallback;
    private Action<float, float> thirstCallback;
    private Action<float, float> satisfactionCallback;

    public UnityEvent<float, float> onHungerChanged;
    public UnityEvent<float, float> onThirstChanged;


    [Header("UI Elements")]
    [SerializeField] private TMP_Text dogNameText;
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private Slider thirstynessSlider;
    [SerializeField] private Slider globalSatisfactionSlider;
    [SerializeField] private Button quitButton;
    
    // DogFillBars enfants (initialisées automatiquement)
    private DogFillBar[] fillBars;

    private void Awake()
    {
        // Cacher la popup au démarrage
        Hide();
        
        // Récupérer tous les DogFillBar enfants
        fillBars = GetComponentsInChildren<DogFillBar>(true);
        
        // Configurer le bouton fermer une seule fois
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    /// <summary>
    /// Initialise la popup pour afficher les infos d'un chien.
    /// Peut être appelé plusieurs fois pour différents chiens.
    /// </summary>
    public void Initialize(GameManager gameManager, DogBehaviour dogBehaviour)
    {
        // Se désabonner de l'ancien chien si on change de cible
        UnsubscribeFromDog();
        
        GameManager = gameManager;
        dog = dogBehaviour;

        if (dog == null) return;

        // Afficher le nom du chien (nom plus propre sans le "(Clone)")
        if (dogNameText != null)
        {
            string cleanName = dog.gameObject.name.Replace("(Clone)", "").Trim();
            dogNameText.text = cleanName;
        }

        // Créer les callbacks
        hungerCallback = OnHungerChanged;
        thirstCallback = OnThirstChanged;
        satisfactionCallback = OnSatisfactionChanged;

        // S'abonner aux événements du chien
        dog.OnHungerChanged += hungerCallback;
        dog.OnThirstynessChanged += thirstCallback;
        dog.OnGlobalSatisfactionChanged += satisfactionCallback;

        // Initialiser les sliders directs avec les valeurs actuelles
        InitializeSliders();
        
        // Initialiser tous les DogFillBar enfants
        InitializeFillBars();
    }

    /// <summary>
    /// Initialise les sliders avec les valeurs actuelles des besoins.
    /// </summary>
    private void InitializeSliders()
    {
        if (dog?.needController == null) return;

        // Trouver et afficher la faim
        var hungerNeed = dog.needController.needs.Find(n => n.Type == NeedType.Faim);
        if (hungerNeed != null && hungerSlider != null)
        {
            hungerSlider.value = hungerNeed.NeedValue / hungerNeed.MaxValue;
        }

        // Trouver et afficher la soif
        var thirstNeed = dog.needController.needs.Find(n => n.Type == NeedType.Soif);
        if (thirstNeed != null && thirstynessSlider != null)
        {
            thirstynessSlider.value = thirstNeed.NeedValue / thirstNeed.MaxValue;
        }

        // Afficher la satisfaction globale (moyenne de tous les besoins)
        if (globalSatisfactionSlider != null)
        {
            float globalSat = CalculateGlobalSatisfaction();
            globalSatisfactionSlider.value = globalSat;
        }
    }

    /// <summary>
    /// Initialise tous les DogFillBar enfants avec le chien actuel.
    /// </summary>
    private void InitializeFillBars()
    {
        if (fillBars == null || dog == null) return;
        
        foreach (var fillBar in fillBars)
        {
            if (fillBar != null)
            {
                fillBar.Initialize(dog);
            }
        }
    }

    /// <summary>
    /// Calcule la satisfaction globale du chien (moyenne normalisée).
    /// </summary>
    private float CalculateGlobalSatisfaction()
    {
        if (dog?.needController?.needs == null || dog.needController.needs.Count == 0) 
            return 0f;

        float total = 0f;
        foreach (var need in dog.needController.needs)
        {
            total += need.NeedValue / need.MaxValue;
        }
        return total / dog.needController.needs.Count;
    }

    private void OnHungerChanged(float current, float max)
    {
        if (hungerSlider != null && max > 0)
            hungerSlider.value = current / max;
    }

    private void OnThirstChanged(float current, float max)
    {
        if (thirstynessSlider != null && max > 0)
            thirstynessSlider.value = current / max;
    }

    private void OnSatisfactionChanged(float current, float max)
    {
        if (globalSatisfactionSlider != null && max > 0)
            globalSatisfactionSlider.value = current / max;
    }

    private void OnQuitClicked()
    {
        Hide();
    }

    /// <summary>
    /// Se désabonne des événements du chien actuel.
    /// </summary>
    private void UnsubscribeFromDog()
    {
        if (dog == null) return;

        if (hungerCallback != null)
            dog.OnHungerChanged -= hungerCallback;
        if (thirstCallback != null)
            dog.OnThirstynessChanged -= thirstCallback;
        if (satisfactionCallback != null)
            dog.OnGlobalSatisfactionChanged -= satisfactionCallback;

        hungerCallback = null;
        thirstCallback = null;
        satisfactionCallback = null;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    private void OnDestroy()
    {
        // Nettoyage propre
        UnsubscribeFromDog();
        
        if (quitButton != null)
            quitButton.onClick.RemoveListener(OnQuitClicked);
    }
}
