using UnityEngine;

[RequireComponent(typeof(DogNeedController))]
public class DogSatisfaction : MonoBehaviour, ISatisfactionProvider
{
    private DogNeedController needController;

    public void Initialize(DogNeedController needController)
    {
        this.needController = needController;
    }

    public float GetSatisfaction()
    {
        if (needController == null || needController.needs == null || needController.needs.Count == 0) return 1f;

        float sum = 0f;

        foreach(NeedBase need in needController.needs)
        {
            float normalized = need.MaxValue > 0f ? need.NeedValue / need.MaxValue : 1f;
            sum += Mathf.Clamp01(normalized);
        }

        return sum / needController.needs.Count;
    }
}
