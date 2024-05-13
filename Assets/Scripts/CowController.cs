using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR.MRUtilityKit;
using UnityEngine.AI;

public class CowController : MonoBehaviour
{
    private float delayTimer; // interval between random point discovery
    public NavMeshAgent agent;
    private GameObject positionIndicator;
    private float timer; // used to calculate current timer value
    public bool VisualizeTargetPosition = false;
    public float cowSpeed;
    private Animator anim;
    public SceneNavigation navigator;
    public bool isBeingTargetted,isGettingBeamed,isInit;
    public ParticleSystem ExitConfetti;
    public NavMeshInitializer _navMeshInit;
    
    void OnEnable()
    {
        navigator = GameObject.Find("RoomNavMesh").GetComponent<SceneNavigation>();
        _navMeshInit = navigator.transform.GetComponent<NavMeshInitializer>();
        isInit = _navMeshInit.Initialized;
        anim = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        navigator.Agents.Add(agent);
        timer = delayTimer;
        Transform childTransform = transform.Find("PositionIndicator");
        positionIndicator = childTransform.gameObject;
        agent.enabled = true;
    }

    void Update()
    {
        if (!isGettingBeamed)
        {
            timer += Time.deltaTime;

            if (isInit)
            {
            GetRandomWalkingPos();
                if (agent.isStopped)
                {
                    Debug.Log("Eating");
                    anim.Play("Eating");
                }
                else
                {
                    Debug.Log("Walking");
                    anim.Play("Walk Forward In Place");
                }
            }
        }
    }

    public void StopAndEat()
    {
        agent.enabled = false;
        anim.Play("Eating");
        delayTimer = 1000;
    }

    public void GetRandomWalkingPos()
    {
        if (timer >= delayTimer)
        {
            // set the new set of randomize values for target position and speed
            Vector3 newPos = RandomNavPoint();

            var room = MRUK.Instance?.GetCurrentRoom();
            if (!room)
            {
                return;
            }

            bool
                test = room.IsPositionInRoom(newPos,
                    false); // occasionally NavMesh will generate areas outside the room, so we must test the value from RandomNavPoint

            if (!test)
            {
                Debug.Log("[NavMeshAgent] [Error]: destination is outside the room bounds, resetting to 0");
                newPos = Vector3.zero;
            }

            if (VisualizeTargetPosition)
            {
                positionIndicator.transform.parent = null;
                positionIndicator.transform.position = newPos;
            }

            agent.SetDestination(newPos);
            float newDelay = Random.Range(6f, 10.0f);
            delayTimer = newDelay;
            agent.speed = cowSpeed;
            timer = 0;
        }
    }

    // generate a new position on the NavMesh
    public static Vector3 RandomNavPoint()
    {
        // TODO: we can cache this and only update it once the navmesh changes
        var triangulation = UnityEngine.AI.NavMesh.CalculateTriangulation();

        if (triangulation.indices.Length == 0)
        {
            return Vector3.zero;
        }

        // Compute the area of each triangle and the total surface area of the navmesh
        float totalArea = 0.0f;
        List<float> areas = new List<float>();
        for (int i = 0; i < triangulation.indices.Length;)
        {
            var i0 = triangulation.indices[i];
            var i1 = triangulation.indices[i + 1];
            var i2 = triangulation.indices[i + 2];
            var v0 = triangulation.vertices[i0];
            var v1 = triangulation.vertices[i1];
            var v2 = triangulation.vertices[i2];
            var cross = Vector3.Cross(v1 - v0, v2 - v0);
            float area = cross.magnitude * 0.5f;
            totalArea += area;
            areas.Add(area);
            i += 3;
        }

        // Pick a random triangle weighted by surface area (triangles with larger surface
        // area have more chance of being chosen)
        var rand = Random.Range(0, totalArea);
        int triangleIndex = 0;
        for (; triangleIndex < areas.Count - 1; ++triangleIndex)
        {
            rand -= areas[triangleIndex];
            if (rand <= 0.0f)
            {
                break;
            }
        }

        {
            // Get the vertices of the chosen triangle
            var i0 = triangulation.indices[triangleIndex * 3];
            var i1 = triangulation.indices[triangleIndex * 3 + 1];
            var i2 = triangulation.indices[triangleIndex * 3 + 2];
            var v0 = triangulation.vertices[i0];
            var v1 = triangulation.vertices[i1];
            var v2 = triangulation.vertices[i2];

            // Calculate a random point on that triangle
            float u = Random.Range(0.0f, 1.0f);
            float v = Random.Range(0.0f, 1.0f);
            if (u + v > 1.0f)
            {
                if (u > v)
                {
                    u = 1.0f - u;
                }
                else
                {
                    v = 1.0f - v;
                }
            }

            return v0 + u * (v1 - v0) + v * (v2 - v0);
        }
    }

    public void GettingBeamed()
    {
        isGettingBeamed = true;
        anim.Play("Beaming Up");    
    }

    public void FreedAtLast()
    {
        anim.Play("Free");
       // ExitConfetti.Play();
        Debug.Log("FREEEEEEEEEEE");
        DestroySelf(4);
    }

    public void DestroySelf(int time)
    {
        Destroy(gameObject,time);
    }
    
}