using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshTester : MonoBehaviour
{
    public Transform target;
    public NavMeshAgent _navMeshAgent;
    void Start()
    {
        _navMeshAgent.destination = target.position;
        //_navMeshAgent.ResetPath();
         }
    
}
