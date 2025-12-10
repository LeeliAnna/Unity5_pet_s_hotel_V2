using UnityEngine;

public class EndOfDayMenu : MonoBehaviour, IMenu
{
    public GameManager GameManager { get; private set;}

    //[Header("Boutons UI")]
    public void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;

        Hide();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }


    public void Show()
    {
        gameObject.SetActive(true);
    }
}
