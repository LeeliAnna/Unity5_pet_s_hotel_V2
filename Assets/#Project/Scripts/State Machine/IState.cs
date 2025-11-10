using System.Threading.Tasks;
using UnityEngine;

public interface IState
{
    public void Enter();
    public async Task Process() { }
    public async Task Exit() { }

}
