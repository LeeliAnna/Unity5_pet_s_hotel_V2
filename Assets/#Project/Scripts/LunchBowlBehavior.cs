using UnityEngine;

public class LunchBowlBehavior : MonoBehaviour
{
    private int maxQuantity;
    private int _currentQuantity;
    private bool _isUsable;

    public int CurrentQuantity
    {
        get
        {
            return _currentQuantity;
        }
        set
        {
            _currentQuantity = Mathf.Clamp(value, 0, maxQuantity);
        }
    }

    public bool IsUsable
    {
        get
        {
            return _isUsable;
        }
        private set
        {
            if (CurrentQuantity == 0) _isUsable = true;
            _isUsable = false;
        }
    }

    public void Initialize(int quantity)
    {
        this.maxQuantity = quantity;
        CurrentQuantity =  maxQuantity;
    }

    public void DecreaseQuantity(int quantity)
    {
        CurrentQuantity -= quantity;
    }

    public void AddQuantity(int quantity)
    {
        CurrentQuantity += quantity;
    }

}
