using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class AlienKidnapper : MonoBehaviour
{
    public float hoverHeight = 0.1f;
    public float hoverSpeed = 1f;
    public float spinSpeed = 100f;
    public ParticleSystem Ray;
    public Animator anim;
    public List<GameObject> ActiveCows = new();
    public bool canHover, canSpin;
    private Vector3 startPos;
    public GameObject SelectedCow,CowConfetti;
    public CowController _cowController;
    private int randDelay, MaxHealth;
    public float moveSpeed, beamSpeed, heightAboveCow;
    private Vector3 targetPos;
    private bool reachedCow;
    public Transform CowHolder;
    public float initialRange, finalRange, ScaleFactor;
    public int maxHealth, currentHealth;
    private Rigidbody rb;
    public ParticleSystem DieParticle;
    private Rotator rot;
    private void Start()
    {
        startPos = transform.localPosition;
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        ActiveCows = GameObject.FindGameObjectsWithTag("Cow").ToList();
        GetRandomCow();
        if (_cowController != null)
        {
            randDelay = Random.Range(3, 5);
            _cowController.StopAndEat();
            targetPos = new Vector3(SelectedCow.transform.position.x, SelectedCow.transform.position.y + heightAboveCow,
                SelectedCow.transform.position.z);
            Invoke("TargetCow", randDelay);
            anim.Play("UFO Hover");
        }
    }

    private void TargetCow()
    {
        transform.localPosition = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            randDelay = 0;
            reachedCow = true;
            Ray.gameObject.SetActive(true);
            CowHolder.transform.position = SelectedCow.transform.Find("PositionIndicator").position;
            SelectedCow.transform.SetParent(CowHolder.transform);
            CowHolder.GetComponent<Rotator>().ForwardX = true;
            CowHolder.GetComponent<Rotator>().ReverseZ = true;
            initialRange = Vector3.Distance(CowHolder.transform.localPosition, Ray.transform.localPosition);
            _cowController.GettingBeamed();
        }
    }

    private void GetRandomCow()
    {
        for (int i = 0; i < ActiveCows.Count; i++)
        {
            if (ActiveCows[i].GetComponent<CowController>().isBeingTargetted == false && SelectedCow == null)
            {
                SelectedCow = ActiveCows[i];
                _cowController = SelectedCow.GetComponent<CowController>();
                _cowController.isBeingTargetted = true;
            }
        }
    }

    void UpdateActiveCowList()
    {
        ActiveCows.Clear();
        ActiveCows = GameObject.FindGameObjectsWithTag("Cow").ToList();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            OnHit();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (randDelay != 0)
        {
            TargetCow();
        }

        if (reachedCow)
        {
            BeamCowUp();
        }
    }
    [Button("Got Hit")]
    public void OnHit()
    {
        if (currentHealth > 0)
        {
            anim.Play("UFO Hit");
            currentHealth--;
        }
        else
        {
            DieUFO();
        }
    }

    private void DieUFO()
    {
        _cowController.FreedAtLast();
        anim.gameObject.SetActive(false);
        GameObject ExitConf = GameObject.Instantiate(CowConfetti);
        ExitConf.transform.position = CowHolder.transform.position;
        ExitConf.transform.localScale = new Vector3(0.075f, 0.075f, 0.075f);
        ExitConf.transform.rotation = Quaternion.identity;
        DieParticle.Play();
        Destroy(gameObject,5);
    }

    private void BeamCowUp()
    {
        if (!Ray.isPlaying)
        {
            Ray.Play();
        }
        CowHolder.transform.localPosition =
            Vector3.MoveTowards(CowHolder.transform.localPosition, Ray.transform.localPosition, beamSpeed);
        finalRange = Vector3.Distance(CowHolder.transform.localPosition, Ray.transform.localPosition);
        ScaleFactor = Mathf.InverseLerp(0, initialRange, finalRange);
        ScaleFactor += 0.25f;
        ScaleFactor = Mathf.Clamp(ScaleFactor, 0f, 1f);
        CowHolder.transform.localScale = new Vector3(ScaleFactor,ScaleFactor,ScaleFactor);
        
    }
}