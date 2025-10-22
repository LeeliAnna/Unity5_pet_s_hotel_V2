using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(DogBehavior))]
public class RandomMovement : MonoBehaviour
{
    private LevelManager levelManager;
    private DogBehavior dogBehavior;

    private float range;
    private float cooldownMax;

    private float cooldownActual = 0f;

    public void Initialize(LevelManager levelManager, float range, float cooldownMax )
    {
        this.levelManager = levelManager;
        this.dogBehavior = GetComponent<DogBehavior>();
        this.range = range;
        this.cooldownMax = cooldownMax;

    }

    public void Process()
    {
        if(cooldownActual > 0f)
        {
            cooldownActual -= Time.deltaTime;
        }
        if(dogBehavior.Agent.remainingDistance >= dogBehavior.Agent.stoppingDistance && cooldownActual <= 0f)
        {
            ChooseDestination();
        }
    }

    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }
        result = Vector3.zero;
        return false;
    }

    public void ChooseDestination()
    {
        Vector3 point;
        if(RandomPoint(levelManager.CenterPoint.position, range, out point))
        {
            dogBehavior.Agent.SetDestination(point);
            cooldownActual = cooldownMax;
        }
    }
}
