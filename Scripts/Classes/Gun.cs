using System;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

public class Gun : ItemShip
{
    [Header("Main Stats")]
    [SerializeField] private Bullet.CaliberType barrelType;
    [SerializeField][Range(1, 5)] private int levelUpgrage = 1;
    [SerializeField] private float energyUsage = 0.0f;
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
    [SerializeField][Range(0, 1)] private float volumeModifier = 1.0f;


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
    private const float maxAngleSleeveRotate = 20;
    private const string layerFireAnimatorName = "Base Layer";

    static float randomSeed = 456;

    private void Awake()
    {
        fireScript = transform.root.GetComponent<Fire>();
        shipRB = transform.root.GetComponent<Rigidbody2D>();
        if (fireScript == null || shipRB == null)
            throw new NullReferenceException($"fireScript = {fireScript} and shipRB = {shipRB}");
    }

    void Start()
    {
        SetVolumeModifier();

        FindEndBurrel();

        SetAudioClipsInAudioSourses();

        spread = ReturnSpread();

        SetFireRateInFixedFrame();

        LoadResources();

        ChangeSpeedFireAnimation();
    }


    private void FixedUpdate()
    {
        GunFire();
    }

    private void SetVolumeModifier()
    {
        for (int i = 0; i < audioFires.Length; i++)
            audioFires[i].volume *= volumeModifier;
    }

    private void ChangeSpeedFireAnimation()
    {
        AnimatorController ac = fireAnimator.runtimeAnimatorController as AnimatorController;
        int index = fireAnimator.GetLayerIndex(layerFireAnimatorName);
        if (index != -1)
        {
            ChildAnimatorState[] cas = ac.layers[index].stateMachine.states.Where(x => x.state.tag == "Fire").ToArray();
            if (cas != null)
            {
                float animSpeed = ReturnAnimationSpeed();
                foreach (var item in cas)
                    item.state.speed = animSpeed;
            }
        }


        float ReturnAnimationSpeed()
        {
            if (fireRate <= 600)
                return 4;
            else return fireRate / 150;
        }
    }

    private void LoadResources()
    {
        LoadBulletInGun("Ship Assets/Bullets/CB 13.45"); // make dynamic

        if (chamber)
            LoadChamberInGun();
    }

    private void SetAudioClipsInAudioSourses()
    {
        for (int i = 0; i < AudioFires.Length; i++)
            AudioFires[i].clip = audioClipFire;
    }

    private void SetFireRateInFixedFrame()
    {
        fireRateInFixedFrame = 60 / FireRate;
        if (fireRateInFixedFrame <= 0)
            throw new IndexOutOfRangeException($"fireRateInFixedFrame was < 0 (fireRateInFixedFrame = {fireRateInFixedFrame})");
    }

    private void FindEndBurrel()
    {
        transformFirePoint = transform.Find("EndBurrel")?.GetComponent<Transform>();
        if (transformFirePoint == null)
            throw new NullReferenceException($"transformFirePoint = {transformFirePoint}");
    }

    private void LoadBulletInGun(string path)
    {
        bulletObject = Resources.Load(path, typeof(GameObject)) as GameObject;
        if (bulletObject == null)
            throw new NullReferenceException($"bulletObject = null");
    }

    private void LoadChamberInGun()
    {
        transformChamber = transform.Find("Chamber")?.GetComponent<Transform>();
        if (transformChamber == null)
            chamber = false;
        else
            LoadSleeveInGun("Ship Assets/Bullets/CB 13.45 sleeve"); // make dynamic
    }
    
    private void LoadSleeveInGun(string path)
    {
        sleeveObject = Resources.Load(path, typeof(GameObject)) as GameObject;
        if (sleeveObject == null)
            throw new NullReferenceException($"sleeveObject = null");
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
            if (timeFiring >= fireRateInFixedFrame * shotsFired)
                InitShot();
            timeFiring += Time.fixedDeltaTime;
        }
    }

    private void InitShot()
    {
        InitBullet();

        if (chamber)
            InitSleeve();

        shotsFired += 1;
        //Debug.Log($"timeFiring = {timeFiring} | shotsFired = {shotsFired}");
    }

    private void InitBullet()
    {
        GameObject bulletObject = Instantiate(this.bulletObject, transformFirePoint.position, transformFirePoint.rotation);
        Bullet bullet = bulletObject.GetComponent<Bullet>();
        Rigidbody2D bullet_rb = bulletObject.GetComponent<Rigidbody2D>();
        if (bullet_rb == null)
            return;

        //UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
        UnityEngine.Random.InitState((int)(randomSeed * 10000));

        float angelSpread = randomSeed = maxAngleGunSpread * Random(-1f, 1f) * spread;

        Vector3 bulletVectorEuler = bulletObject.transform.eulerAngles;
        bulletObject.transform.eulerAngles = new Vector3(bulletVectorEuler.x, bulletVectorEuler.y, bulletVectorEuler.z + angelSpread);
        Vector2 force = bulletObject.transform.up * (float)bullet?.Speed + (Vector3)shipRB.velocity;

        AddForceImpulse(bullet_rb, force);

        FireAnimator?.SetTrigger(triggerNameFire);

        if (++ausioSourseIndex >= AudioFires.Length)
            ausioSourseIndex = 0;
        AudioFires[ausioSourseIndex]?.Play();
    }

    private void InitSleeve()
    {
        GameObject sleeveObject = Instantiate(this.sleeveObject, transformChamber.position, transformChamber.rotation);
        Rigidbody2D sleeve_rb = sleeveObject.GetComponent<Rigidbody2D>();
        if (sleeve_rb == null)
            return;

        UnityEngine.Random.InitState((int)(randomSeed * 10000));

        float angelRotate = randomSeed = maxAngleSleeveRotate * Random(-1f, 1f);

        Vector3 sleeveVectorEuler = sleeveObject.transform.eulerAngles;
        float angleRotateShift = -sleeveVectorEuler.z - 90 * (_ = ItemSide == ItemShip.Side.Left ? -1 : 1) * Random(0f, -0.25f); // check
        if (angleRotateShift > 360) angleRotateShift -= 360;
        else if (angleRotateShift < 0) angleRotateShift += 360;
        float radian = angleRotateShift * Mathf.Deg2Rad;
        Vector2 vectorDirection = new Vector2((float)Math.Sin(radian), (float)Math.Cos(radian));

        sleeveObject.transform.eulerAngles = new Vector3(sleeveVectorEuler.x, sleeveVectorEuler.y, sleeveVectorEuler.z + angelRotate + 90);
        Vector2 force = vectorDirection * Random(0.2f, 0.4f) + shipRB.velocity;

        AddForceImpulse(sleeve_rb, force);
        var impulse = (Random(-1000, 1000) * Mathf.Deg2Rad) * sleeve_rb.inertia;
        sleeve_rb.AddTorque(impulse, ForceMode2D.Impulse);
    }

    private void AddForceImpulse(Rigidbody2D rigidbody2D, Vector2 force) => rigidbody2D.AddForce(force, ForceMode2D.Impulse);

    private float Random(float from, float to) => UnityEngine.Random.Range(from, to);

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
