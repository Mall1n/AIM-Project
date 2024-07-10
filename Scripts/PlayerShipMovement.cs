using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
// [ExecuteAlways]
public class PlayerShipMovement : MonoBehaviour
{
    [Header("Main Attributes")]
    [SerializeField] private ShipConfiguration shipConfiguration;
    [SerializeField] private Camera _camera;


    // [Header("Engine Sound")]
    // [SerializeField] private AudioClip audioEngineSound;
    // private AudioSource audioEngineSource;


    [Header("Log Attributes")]
    //Log
    [SerializeField] private bool LogDrawLinesDirection = true;
    //Log


    [SerializeField][Range(0, 10)] private float maxSpeed = 0;
    [SerializeField]float inertiaFactor = 0.1f;



    [Range(0, 20)] private float pullEnginesPower = 0;
    public float PullEnginesPower
    {
        get => pullEnginesPower;
        set { pullEnginesPower = value; DefineAcceleration(); }
    }
    [SerializeField]
    private float acceleration = 0;
    public float Acceleration { get => acceleration; }


    [Range(0.01f, 1f)] private float mobility = 0;
    public float Mobility { get => mobility; set { mobility = value; DefineMobility(); } }


    [SerializeField]
    private float rotateMobility = 0;


    private Rigidbody2D rb;
    public Rigidbody2D Rb { get => rb; }

    private Vector2 mousePosition;
    private Vector2 moveDirection_Raw;
    private Vector2 moveDirection;
    private Vector2 moveDirection_From;
    private float moveX_From = 0;
    private float moveY_From = 0;
    private float moveXraw = 0;
    private float moveYraw = 0;
    private float lerp = 0;
    private readonly float factorDecreaseSpeed = 2;



    private void Awake()
    {
        if (shipConfiguration == null)
            throw new NullReferenceException("shipConfiguration equals null");

        rb = GetComponent<Rigidbody2D>();
        //audioEngineSource = GetComponent<AudioSource>();

        DefineFields();
    }

    private void ChangeEngineValues(bool value)
    {
        shipConfiguration.SetAnimatorsEngineFire(value);
    }

    private void OnEnable()
    {
        ShipConfiguration.onChangedShipValues += UpdateValues;
    }

    private void OnDisable()
    {
        ShipConfiguration.onChangedShipValues -= UpdateValues;
    }

    private void Update()
    {
        //Log
        DefineFields();
        //Log

        GetMoveAxis();
    }

    private void FixedUpdate()
    {
        if (CheckChangeDirection())
            ResetValuesDirection();

        Move();

        BodyRotate();
    }

    private void DefineFields()
    {
        DefineAcceleration();

        DefineMobility();
    }

    private void DefineAcceleration()
    {
        if (pullEnginesPower == 0 || maxSpeed == 0)
            acceleration = 0;
        else
            acceleration = pullEnginesPower / maxSpeed;
    }

    private void DefineMobility()
    {
        if (Mobility < 1)
        {
            float reverseMobility = 1 - Mobility;
            rotateMobility = (360 - (reverseMobility * 360)) / (90 * reverseMobility) + 1;
            if (rotateMobility > 180) rotateMobility = 180;
        }
        else { rotateMobility = 180; }
    }

    private void GetMoveAxis()
    {
        moveXraw = Input.GetAxisRaw("Horizontal");
        moveYraw = Input.GetAxisRaw("Vertical");

        moveDirection_Raw = new Vector2(moveXraw, moveYraw).normalized;
    }

    private Vector2 GetMousePosition()
    {
        //mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        return _camera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        //return Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg - 90;
    }

    private void BodyRotate()
    {
        mousePosition = GetMousePosition();

        Quaternion q = Quaternion.LookRotation(Vector3.forward, mousePosition);

        rb.rotation = Quaternion.RotateTowards(transform.rotation, q, rotateMobility).eulerAngles.z;
    }

    private void Move()
    {
        lerp += Time.deltaTime;
        if (moveXraw == 0 && moveYraw == 0)
        {
            ChangeEngineValues(false);
            moveDirection = Vector2.MoveTowards(moveDirection_From, moveDirection_Raw, lerp * inertiaFactor / factorDecreaseSpeed); // acceleration insted of inertiaFactor
        }
        else
        {
            ChangeEngineValues(true);
            moveDirection = Vector2.MoveTowards(moveDirection_From, moveDirection_Raw, lerp * acceleration);
        }

        rb.velocity = new Vector2(moveDirection.x * maxSpeed, moveDirection.y * maxSpeed);
    }

    private bool CheckChangeDirection()
    {
        if (moveXraw != moveX_From || moveYraw != moveY_From)
            return true;
        return false;
    }

    private void ResetValuesDirection()
    {
        moveX_From = moveXraw;
        moveY_From = moveYraw;
        moveDirection_From = moveDirection;
        lerp = 0;
    }

    public void UpdateValues()
    {
        this.maxSpeed = shipConfiguration.MaxSpeed;
        PullEnginesPower = shipConfiguration.EnginesAccelerationPower;
        Mobility = shipConfiguration.Mobility;
    }

    private void OnDrawGizmos()
    {
        if (!LogDrawLinesDirection)
            return;

        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        DrawLine(moveDirection_Raw);

        Gizmos.color = Color.blue;
        DrawLine(moveDirection_From);

        Gizmos.color = Color.red;
        DrawLine(moveDirection);

        // Gizmos.color = Color.yellow;
        // DrawLine(DrowLineTest);

        //Debug.Log($"from {transform.position} to {moveDirection}");
        void DrawLine(Vector2 vector) => Gizmos.DrawLine(transform.position, position + vector);
    }

}
