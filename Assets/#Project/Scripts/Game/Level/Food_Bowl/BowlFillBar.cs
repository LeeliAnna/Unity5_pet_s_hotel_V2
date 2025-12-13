using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BowlFillBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;

    private BowlBehaviour lunchBowl;

    public void Initialize(BowlBehaviour bowl)
    {
        lunchBowl = bowl;
        if (slider != null && bowl != null)
        {
            //slider.onValueChanged.AddListener(UpdateFillColor);
            bowl.OnQuantityChanged += UpdateFillColor;
        }
    }

    private void UpdateFillColor(float current, float max)
    {
        if (fillImage != null)
        {
            fillImage.color = Color.Lerp(Color.red, Color.green, current / max);
        }
    }
}
