using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;
//using SymmetryBreakStudio.TastyGrassShader;
using Unity.VisualScripting;

public class NavMeshInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    public SceneNavigation _SceneNavigation;
    public bool Initialized;
    public List<Transform> floorObjs = new();
    public Material GrassyMat;
   // public TgsBakeSettings BakeSettings;
    public void Init()
    {
        for (int i = 0; i < _SceneNavigation.Agents.Count; i++)
        {
            _SceneNavigation.Agents[i].enabled = true;
            _SceneNavigation.Agents[i].GetComponent<CowController>().isInit = true;
        }

        Initialized = true;
    }
    [Button("Set Grass")]
    public void ConfigureGrass()
    {
      //   GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
      //   GameObject[] tempfloorObjs = allObjects.Where(obj => obj.name == "FLOOR").ToArray();
      //   for (int i = 0; i < tempfloorObjs.Length; i++)
      //   {
      //       for (int j = 0; j < tempfloorObjs[i].transform.childCount; j++)
      //       {
      //           floorObjs.Add(tempfloorObjs[i].transform.GetChild(j));
      //           Debug.Log("Floor name " + floorObjs[i].name );
      //       }
      //   }
      //
      //   for (int k = 0; k < floorObjs.Count; k++)
      //   {
      // //      floorObjs[k].GetComponent<MeshRenderer>().material = GrassyMat;
      //       floorObjs[k].gameObject.SetActive(false);
      //       floorObjs[k].AddComponent<TgsForMeshFilter>();
      //       floorObjs[k].GetComponent<TgsForMeshFilter>().bakeSettings = BakeSettings;
      //       floorObjs[k].GetComponent<TgsForMeshFilter>().quickSettings.scale = 0.25f;
      //       floorObjs[k].GetComponent<TgsForMeshFilter>().quickSettings.density = 100;
      //       //floorObjs[k].GetComponent<TgsForMeshFilter>().quickSettings.tint = new Color(121, 34, 100);
      //       floorObjs[k].gameObject.SetActive(true);
        //  }
    }
}
