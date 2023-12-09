using System;
using UnityEngine;

public class Gun : ItemShip
{
    [Header("Main Stats")]
    [SerializeField] private float energyUsage;
    [SerializeField] private float damage;
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

    [SerializeField] private float speedReload;


    [Header("Fire rate in minutte")]
    [SerializeField][Range(1, 5400)] private float fireRate;
    public float FireRate { get => fireRate; }


    [Header("Animation Fire")]
    [SerializeField] private Animator fireAnimator;
    public Animator FireAnimator { get => fireAnimator; }
    [SerializeField] private AudioSource[] audioFires;
    [SerializeField] private AudioClip audioClipFire;
    public AudioSource[] AudioFires { get => audioFires; }


    //[SerializeField] 
    //[SerializeField] 
    private Rigidbody2D rb;
    private Fire fireScript;
    private GameObject bulletObject;
    private int shotsFired = 0;
    private float fireRateInFixedFrame;
    private float spread;
    private int ausioSourseIndex = 0;
    private const string triggerNameFire = "TriggerFire";
    private bool isLeftClicking = false;
    private bool reloaded = true;
    private float timeFiring = 0;
    private Transform transformFirePoint;


    private void FixedUpdate()
    {
        GunFire();
    }

    void Start()
    {
        rb = transform.root.GetComponent<Rigidbody2D>();
        bulletObject = Resources.Load("Ship Assets/Bullets/Bullet Default", typeof(GameObject)) as GameObject;
        transformFirePoint = GetComponentInChildren<Transform>();
        for (int i = 0; i < AudioFires.Length; i++)
            AudioFires[i].clip = audioClipFire;
        spread = ReturnSpread();
        fireRateInFixedFrame = 60 / FireRate;
        if (fireRateInFixedFrame <= 0)
            throw new IndexOutOfRangeException($"fireRateInFixedFrame was < 0 (fireRateInFixedFrame = {fireRateInFixedFrame})");
    }

    private void Awake()
    {
        fireScript = transform.root.GetComponent<Fire>();
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

    private void InitShot()
    {
        // Quaternion q = Quaternion.Euler(transformFirePoint.rotation.x, transformFirePoint.rotation.y, transformFirePoint.rotation.z + 45);
        // GameObject gameObject = new GameObject();
        // gameObject.transform.eulerAngles = new Vector3(transformFirePoint.rotation.x, transformFirePoint.rotation.y, transformFirePoint.rotation.z + 45);
        // //t.eulerAngles = new Vector3(transformFirePoint.rotation.x, transformFirePoint.rotation.y, transformFirePoint.rotation.z + 45);
        GameObject _bulletObject = Instantiate(this.bulletObject, transformFirePoint.position, transformFirePoint.rotation);
        Bullet bullet = _bulletObject.GetComponent<Bullet>();
        Rigidbody2D bullet_rb = _bulletObject.GetComponent<Rigidbody2D>();
        if (bullet_rb == null)
            return;

        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
        float randomDegrees = UnityEngine.Random.Range(-0.5f, 0.5f);

        //bullet_rb.rotation += 45;

        //float radian = 90 * (Mathf.PI / 180);
        //Vector2 tmp = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));

        // float degrees = Mathf.Atan2(transformFirePoint.up.x, transformFirePoint.up.y) * Mathf.Rad2Deg;
        // degrees += 45;
        // float radian = 45 * (Mathf.PI / 180);
        // Vector2 vectorBullet = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));

        //bullet_rb.rotation += 45;

        

        Vector2 force = transformFirePoint.up * (float)bullet?.Speed;
        //Vector2 force = vectorBullet * (float)bullet?.Speed;
        if (rb)
            force += rb.velocity;
        // System.Random rnd = new System.Random(5);
        // float rndttemp = (float)rnd.NextDouble() * 0.5f - 1;
        // force += (Vector2)transformFirePoint.right * rndttemp * spread;
        //UnityEngine.Random.seed = System.DateTime.Now.Millisecond;


        //Vector2 tmp = -(Vector2)transformFirePoint.right * 0.5f * (float)bullet?.Speed;

        // float radian = 90 * (Mathf.PI / 180);
        // Vector2 tmp = -(Vector2)transformFirePoint.right * new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * (float)bullet?.Speed;
        // force += tmp;

        //force += (Vector2)transformFirePoint.right * UnityEngine.Random.Range(-0.5f, 0.5f) * spread;
        AddForce(force);

        FireAnimator?.SetTrigger(triggerNameFire);

        if (++ausioSourseIndex >= AudioFires.Length)
            ausioSourseIndex = 0;
        AudioFires[ausioSourseIndex]?.Play();

        shotsFired += 1;

        void AddForce(Vector2 force) => bullet_rb.AddForce(force, ForceMode2D.Impulse);
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
