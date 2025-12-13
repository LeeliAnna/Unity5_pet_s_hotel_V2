using UnityEngine;

/// <summary>
/// Represente la pension du joueur pendant une partie.
/// </summary>
public class Pension
{
    private int money;
    private int prestige;
    public string Name {get; private set;}
    public int Money 
    {
        get => money; 
        private set => money = Mathf.Max(0,value);
    }
    public int Prestige 
    {
        get => prestige; 
        private set => prestige = Mathf.Max(0,value);
    }

    /// <summary>
    /// Constructeur de la pension
    /// </summary>
    /// <param name="name">nom de la pension</param>
    /// <param name="settings">setting recupere dans les datas pensions settings</param>
    public Pension(string name, PensionSettings settings)
    {
        Name = string.IsNullOrWhiteSpace(name) ? "Ma pension" : name.Trim();
        Money = settings.startingMoney;
        Prestige = settings.startingPrestige;
    }

    /// <summary>
    /// Ajouter l'argent à la pension
    /// </summary>
    /// <param name="amount">montent a ajouter</param>
    public void AddMoney(int amount)
    {
        Money += amount;
    }

    /// <summary>
    /// Retirer de l'argent de la pension
    /// </summary>
    /// <param name="amount">montant à retirer</param>
    public void RemoveMoney(int amount)
    {
        Money -= amount;
    }

    /// <summary>
    /// Ajouter le prestige à la pension
    /// </summary>
    /// <param name="amount">quantité de prestige a ajouter</param>
    public void AddPrestige(int amount)
    {
        Prestige += amount;
    }

    /// <summary>
    /// Définir les valeurs de la pension (utilisé lors du chargement)
    /// </summary>
    public void SetValues(int money, int prestige)
    {
        Money = money;
        Prestige = prestige;
    }

}
