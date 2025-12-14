using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popup d'information sur un chien.
/// Affiche les besoins (faim, soif, satisfaction globale) et se met à jour en temps réel.
/// Utilise les données structurées NeedUIData provenant du ControleurBesoinsChien.
/// </summary>
public class DogPopupInfo : MonoBehaviour
{
    public GameManager GameManager { get; private set; }
    private DogBehaviour dog;
    private ControleurBesoinsChien needController;
    
    // Callbacks pour les événements du nouveau système
    private Action<NeedType, float, bool> needChangedCallback;
    private Action<float> satisfactionChangedCallback;

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

        // Récupérer le ControleurBesoinsChien (nouveau système)
        needController = dog.GetComponent<ControleurBesoinsChien>();

        // Afficher le nom du chien (nom plus propre sans le "(Clone)")
        if (dogNameText != null)
        {
            string cleanName = dog.gameObject.name.Replace("(Clone)", "").Trim();
            dogNameText.text = cleanName;
        }

        // Créer les callbacks pour le nouveau système
        needChangedCallback = OnNeedChanged;
        satisfactionChangedCallback = OnSatisfactionChanged;

        // S'abonner aux événements du ControleurBesoinsChien si disponible
        if (needController != null)
        {
            needController.OnNeedChanged += needChangedCallback;
            needController.OnSatisfactionChanged += satisfactionChangedCallback;
        }

        // Initialiser les sliders directs avec les valeurs actuelles
        InitializeSliders();
        
        // Initialiser tous les DogFillBar enfants
        InitializeFillBars();
    }
    
    /// <summary>
    /// Initialise la popup directement depuis un ControleurBesoinsChien.
    /// Alternative à Initialize(GameManager, DogBehaviour) pour le nouveau système.
    /// </summary>
    /// <param name="gameManager">Référence au GameManager</param>
    /// <param name="controller">Contrôleur de besoins du chien</param>
    /// <param name="displayName">Nom à afficher pour le chien</param>
    public void InitializeFromController(GameManager gameManager, ControleurBesoinsChien controller, string displayName)
    {
        UnsubscribeFromDog();
        
        GameManager = gameManager;
        needController = controller;
        dog = controller?.GetComponent<DogBehaviour>();
        
        // Afficher le nom du chien
        if (dogNameText != null)
        {
            dogNameText.text = displayName;
        }
        
        // Créer et s'abonner aux callbacks
        if (needController != null)
        {
            needChangedCallback = OnNeedChanged;
            satisfactionChangedCallback = OnSatisfactionChanged;
            needController.OnNeedChanged += needChangedCallback;
            needController.OnSatisfactionChanged += satisfactionChangedCallback;
        }
        
        InitializeSliders();
        InitializeFillBars();
    }

    /// <summary>
    /// Initialise les sliders avec les valeurs actuelles des besoins.
    /// Utilise les données structurées NeedUIData du ControleurBesoinsChien.
    /// </summary>
    private void InitializeSliders()
    {
        // Utiliser le nouveau système si disponible
        if (needController != null)
        {
            List<NeedUIData> needsData = needController.GetNeedsUIData();
            
            foreach (var needData in needsData)
            {
                switch (needData.type)
                {
                    case NeedType.Faim:
                        if (hungerSlider != null)
                            hungerSlider.value = needData.normalizedValue;
                        break;
                    case NeedType.Soif:
                        if (thirstynessSlider != null)
                            thirstynessSlider.value = needData.normalizedValue;
                        break;
                }
            }
            
            // Satisfaction globale
            if (globalSatisfactionSlider != null)
            {
                globalSatisfactionSlider.value = needController.GetSatisfaction();
            }
            return;
        }

        // Fallback vers l'ancien système si needController n'est pas disponible
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
    /// Fallback si le ControleurBesoinsChien n'est pas disponible.
    /// </summary>
    private float CalculateGlobalSatisfaction()
    {
        // Utiliser le nouveau système si disponible
        if (needController != null)
            return needController.GetSatisfaction();
        
        // Fallback vers l'ancien système
        if (dog?.needController?.needs == null || dog.needController.needs.Count == 0) 
            return 0f;

        float total = 0f;
        foreach (var need in dog.needController.needs)
        {
            total += need.NeedValue / need.MaxValue;
        }
        return total / dog.needController.needs.Count;
    }

    /// <summary>
    /// Callback appelé quand un besoin change (nouveau système).
    /// Met à jour le slider correspondant au type de besoin.
    /// </summary>
    /// <param name="type">Type du besoin qui a changé</param>
    /// <param name="normalizedValue">Nouvelle valeur normalisée (0 à 1)</param>
    /// <param name="isCritical">Indique si le besoin est critique</param>
    private void OnNeedChanged(NeedType type, float normalizedValue, bool isCritical)
    {
        switch (type)
        {
            case NeedType.Faim:
                if (hungerSlider != null)
                    hungerSlider.value = normalizedValue;
                break;
            case NeedType.Soif:
                if (thirstynessSlider != null)
                    thirstynessSlider.value = normalizedValue;
                break;
        }
    }

    /// <summary>
    /// Callback appelé quand la satisfaction globale change (nouveau système).
    /// </summary>
    /// <param name="satisfaction">Nouvelle valeur de satisfaction (0 à 1)</param>
    private void OnSatisfactionChanged(float satisfaction)
    {
        if (globalSatisfactionSlider != null)
            globalSatisfactionSlider.value = satisfaction;
    }

    private void OnQuitClicked()
    {
        Hide();
    }

    /// <summary>
    /// Se désabonne des événements du chien actuel.
    /// Prend en charge le nouveau et l'ancien système.
    /// </summary>
    private void UnsubscribeFromDog()
    {
        // Nouveau système : ControleurBesoinsChien
        if (needController != null)
        {
            if (needChangedCallback != null)
                needController.OnNeedChanged -= needChangedCallback;
            if (satisfactionChangedCallback != null)
                needController.OnSatisfactionChanged -= satisfactionChangedCallback;
        }

        needChangedCallback = null;
        satisfactionChangedCallback = null;
        needController = null;
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
