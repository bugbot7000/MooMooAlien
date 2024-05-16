using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks.Triggers;
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
    public AudioSource Explosion, Beam;
    private GameManager gm;
    private CowSpawner _cowSpawner;
    public bool cowSaved = true;
    private void OnEnable()
    {
        _cowSpawner = GameObject.Find("Cow Spawn Handler").GetComponent<CowSpawner>();
        _cowSpawner._cowSpawned += UpdateActiveCowList;
    }

    private void FetchNewCow()
    {
        // Destroy(SelectedCow.gameObject);
        // _cowController = null;
        // GetRandomCow();
        // randDelay = 2;
    }

    private void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        startPos = transform.localPosition;
        // maxHealth = maxHealth * 5;
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        ActiveCows = GameObject.FindGameObjectsWithTag("Cow").ToList();
        GetRandomCow();
        RandomizePitch();
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
            Debug.Log("ONTRIGGER CALLED");
            OnHit();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (randDelay != 0 && SelectedCow!=null)
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
        gm.shotsHit++;
        if (currentHealth > 0)
        {
            anim.Play("UFO Hit");
            currentHealth--;
        }
        else
        {
            gm.aliensKilled++;
            DieUFO();
        }
    }

    private void DieUFO()
    {
        gameObject.tag = "Untagged";
        Destroy(transform.GetComponent<BoxCollider>());
        if(_cowController!=null){_cowController.FreedAtLast();}
        anim.gameObject.SetActive(false);
        GameObject ExitConf = GameObject.Instantiate(CowConfetti);
        moveSpeed = 0;
        beamSpeed = 0;
        ExitConf.transform.position = CowHolder.transform.position;
        ExitConf.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
        ExitConf.transform.rotation = Quaternion.identity;
        DieParticle.Play();
        Destroy(gameObject,2.5f);
        if (cowSaved == true && _cowController!=null)
        {
            Explosion.Play();
            gm.cowsSaved++;
        }
        transform.parent.GetComponent<AlienSpawner>().currentAlienCount--;
    }
    
    private void BeamCowUp()
    {
        if (!Ray.isPlaying)
        {
            Ray.Play();
            Beam.Play();
        }
        CowHolder.transform.localPosition =
            Vector3.MoveTowards(CowHolder.transform.localPosition, Ray.transform.localPosition, beamSpeed);
        finalRange = Vector3.Distance(CowHolder.transform.localPosition, Ray.transform.localPosition);
        if (finalRange < 0.1f)
        {
            CowHolder.gameObject.SetActive(false);
            Ray.Stop();
            Beam.Stop();
            cowSaved = false;
            VanishUFO();
            _cowController = null;
           // FetchNewCow();
        }
        ScaleFactor = Mathf.InverseLerp(0, initialRange, finalRange);
        ScaleFactor += 0.25f;
        ScaleFactor = Mathf.Clamp(ScaleFactor, 0f, 1f);
        CowHolder.transform.localScale = new Vector3(ScaleFactor,ScaleFactor,ScaleFactor);
    }

    private void VanishUFO()
    {
        anim.Play("Exit");
    }

    void RandomizePitch()
    {
        float randNo = 10f;
        Explosion.pitch *= 1 + Random.Range(-randNo / 100, randNo / 100);
        Beam.pitch *= 1 + Random.Range(-randNo / 100, randNo / 100);
    }
    private void OnDisable()
    {
        _cowSpawner._cowSpawned -= FetchNewCow;
    }
}