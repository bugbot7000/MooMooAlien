using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AlienKidnapper : MonoBehaviour
{
    public GameObject Ray;
    private Animator anim;
    public List<GameObject> ActiveCows = new();
    private void Start()
    {
        anim = GetComponent<Animator>();
        ActiveCows = GameObject.FindGameObjectsWithTag("Cow").ToList();
    }

    void UpdateActiveCowList()
    {
        ActiveCows.Clear();
        ActiveCows = GameObject.FindGameObjectsWithTag("Cow").ToList();

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
