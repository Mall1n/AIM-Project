using System;
using UnityEngine;

public class Gun : ItemShip
{
    [Header("Main Stats")]
    [SerializeField] private Bullet.Type barrelType;
    [SerializeField][Range(1, 5)] private int levelUpgrage = 1;
    [SerializeField] private float energyUsage;
    [SerializeField][Range(0, 1)] private float accuracy = 0.9f;
    public float Accuracy
    {
        get => accuracy;
        set
        {
            accuracy = value;
            spread = ReturnSpread();
        }
    }

    [SerializeField] private float timeReload;


    [Header("Fire rate in minutte")]
    [SerializeField][Range(1, 5400)] private float fireRate;
    public float FireRate { get => fireRate; }


    [Header("Animation Fire")]
    [SerializeField] private Animator fireAnimator;
    public Animator FireAnimator { get => fireAnimator; }
    [SerializeField] private AudioSource[] audioFires;
    [SerializeField] private AudioClip audioClipFire;
    public AudioSource[] AudioFires { get => audioFires; }

    [Header("Additional Settings")]
    [SerializeField] private bool chamber = false;


    //[SerializeField] 
    //[SerializeField] 
    private Rigidbody2D shipRB;
    private Fire fireScript;
    private GameObject bulletObject;
    private GameObject sleeveObject;
    private int shotsFired = 0;
    private float fireRateInFixedFrame;
    private float spread;
    private int ausioSourseIndex = 0;
    private const string triggerNameFire = "TriggerFire";
    private bool isLeftClicking = false;
    private bool reloaded = true;
    private float timeFiring = 0;
    private Transform transformFirePoint;
    private Transform transformChamber;
    private const float maxAngleGunSpread = 10;



    private void FixedUpdate()
    {
        GunFire();
    }

    void Start()
    {
        if (chamber)
        {
            transformChamber = transform.Find("Chamber")?.GetComponent<Transform>();
            if (transformChamber == null)
                chamber = false;
            else
                LoadSleeveInGun();
        }
        LoadBulletInGun();
        transformFirePoint = transform.Find("EndBurrel")?.GetComponent<Transform>();
        if (transformFirePoint == null)
            throw new NullReferenceException($"transformFirePoint = {transformFirePoint}");
        for (int i = 0; i < AudioFires.Length; i++)
            AudioFires[i].clip = audioClipFire;
        spread = ReturnSpread();
        fireRateInFixedFrame = 60 / FireRate;
        if (fireRateInFixedFrame <= 0)
            throw new IndexOutOfRangeException($"fireRateInFixedFrame was < 0 (fireRateInFixedFrame = {fireRateInFixedFrame})");
    }

    private void LoadBulletInGun()
    {
        bulletObject = Resources.Load("Ship Assets/Bullets/CB 13.45", typeof(GameObject)) as GameObject;
    }

    private void LoadSleeveInGun()
    {
        sleeveObject = Resources.Load("Ship Assets/Bullets/CB 13.45 sleeve", typeof(GameObject)) as GameObject;
    }

    private void Awake()
    {
        fireScript = transform.root.GetComponent<Fire>();
        shipRB = transform.root.GetComponent<Rigidbody2D>();
        if (fireScript == null || shipRB == null)
            throw new NullReferenceException($"fireScript = {fireScript} and shipRB = {shipRB}");
    }

    private void OnEnable()
    {
        if (fireScript != null && fireScript.GetType() == typeof(PlayerFire))
        {
            PlayerFire.onPlayerFired += StartFire;
            PlayerFire.onPlayerStopFired += StopFire;
        }
    }

    private void OnDisable()
    {
        if (fireScript != null && fireScript.GetType() == typeof(PlayerFire))
        {
            PlayerFire.onPlayerFired -= StartFire;
            PlayerFire.onPlayerStopFired -= StopFire;
        }
    }

    private void GunFire()
    {
        if (isLeftClicking)
        {
            timeFiring += Time.fixedDeltaTime;
            if (timeFiring > fireRateInFixedFrame * shotsFired)
                InitShot();
        }
    }

    static float randomSeed = 456;
    private void InitShot()
    {
        InitBullet();

        if (chamber)
            InitChamber();

        shotsFired += 1;
    }

    private void InitBullet()
    {
        GameObject _bulletObject = Instantiate(this.bulletObject, transformFirePoint.position, transformFirePoint.rotation);
        Bullet bullet = _bulletObject.GetComponent<Bullet>();
        Rigidbody2D bullet_rb = _bulletObject.GetComponent<Rigidbody2D>();
        if (bullet_rb == null)
            return;

        //UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
        UnityEngine.Random.InitState((int)(randomSeed * 10000));

        float angelSpread = randomSeed = maxAngleGunSpread * UnityEngine.Random.Range(-1f, 1f) * spread;

        Vector3 bulletVectorEuler = _bulletObject.transform.eulerAngles;
        _bulletObject.transform.eulerAngles = new Vector3(bulletVectorEuler.x, bulletVectorEuler.y, bulletVectorEuler.z + angelSpread);
        Vector2 force = _bulletObject.transform.up * (float)bullet?.Speed + (Vector3)shipRB.velocity;

        AddForce(force);

        FireAnimator?.SetTrigger(triggerNameFire);

        if (++ausioSourseIndex >= AudioFires.Length)
            ausioSourseIndex = 0;
        AudioFires[ausioSourseIndex]?.Play();

        void AddForce(Vector2 force) => bullet_rb.AddForce(force, ForceMode2D.Impulse);
    }

    private void InitChamber()
    {
        GameObject _sleeveObject = Instantiate(this.sleeveObject, transformChamber.position, transformChamber.rotation);
        Rigidbody2D sleeve_rb = _sleeveObject.GetComponent<Rigidbody2D>();

        if (sleeve_rb == null)
            return;

        UnityEngine.Random.InitState((int)(randomSeed * 10000));

        float angelRotate = randomSeed = maxAngleGunSpread * UnityEngine.Random.Range(-1f, 1f) * 10;

        Vector3 sleeveVectorEuler = _sleeveObject.transform.eulerAngles;
        _sleeveObject.transform.eulerAngles = new Vector3(sleeveVectorEuler.x, sleeveVectorEuler.y, sleeveVectorEuler.z + angelRotate);

        Vector2 force = (transformChamber.transform.up + new Vector3(0, UnityEngine.Random.Range(-0.2f, 0.2f), 0)) * UnityEngine.Random.Range(0.1f, 0.5f) + (Vector3)shipRB.velocity;

        AddForce(force);

        void AddForce(Vector2 force) => sleeve_rb.AddForce(force, ForceMode2D.Impulse);
    }

    private void StartFire()
    {
        isLeftClicking = true;
    }

    private void StopFire()
    {
        isLeftClicking = false;
        shotsFired = 0;
        timeFiring = 0;
    }

    private float ReturnSpread() => 1 - accuracy;
}
