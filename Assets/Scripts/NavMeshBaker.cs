using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using NaughtyAttributes;
public class NavMeshTester : MonoBehaviour
{
    public Transform target;
    public NavMeshSurface AIsurface;
    
    [Button ("Build Floor")]
    public void BuildNavMesh()
    {
        AIsurface.BuildNavMesh();
    }
    
}
