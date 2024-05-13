using Meta.XR.MRUtilityKit;
using UnityEngine;

public class NavMeshInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    public SceneNavigation _SceneNavigation;
    public bool Initialized;
    public void Init()
    {
        for (int i = 0; i < _SceneNavigation.Agents.Count; i++)
        {
            _SceneNavigation.Agents[i].enabled = true;
            _SceneNavigation.Agents[i].GetComponent<CowController>().isInit = true;
        }

        Initialized = true;
    }
}
