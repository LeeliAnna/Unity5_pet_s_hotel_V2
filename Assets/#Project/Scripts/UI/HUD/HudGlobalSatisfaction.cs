using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudGlobalSatisfaction : MonoBehaviour
{
    public AggregateurSatisfactionPension Service{ get; private set;}

    [Header("Satisfaction")]
    [SerializeField] private Slider satisfactionSlider;
    [SerializeField] private TMP_Text satisfactionText;

    [Header("Données Pension")]
    [SerializeField] private TMP_Text namePensionText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text prestigeText;

    public void Initialize(AggregateurSatisfactionPension service)
    {
        Service = service;
    }

    public void Process()
    {
        if(Service == null) return;

        float globalSatisfaction = Service.Current;
        if (satisfactionSlider != null) satisfactionSlider.value = globalSatisfaction;
        if (satisfactionText != null) satisfactionText.text = $"{Mathf.RoundToInt(globalSatisfaction * 100f)}%";

    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    public void SetPension(string name, int money, int prestige)
    {
        if(namePensionText!= null) namePensionText.text = name != null ? name : "-";

        if(moneyText != null) moneyText.text = money != 0 ? $"{money} $" : "0 $";

        if(prestigeText != null) prestigeText.text = prestige != 0 ? $"{prestige}" : "0";
        
    }
}
