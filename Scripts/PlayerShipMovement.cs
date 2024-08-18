using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(AudioSource))]
// [ExecuteAlways]
public class PlayerShipMovement : MonoBehaviour
{
    [Header("Main Attributes")]
    [SerializeField] private ShipConfiguration _shipConfiguration;
    [SerializeField] private Camera _camera;


    [Header("Log Attributes")]
    //Log
    [SerializeField] private bool LogDrawLinesDirection = true;
    //Log


    [SerializeField][Range(0, 2)] float _inertiaFactor = 0.1f;

    private float _maxSpeed = 0;
    private float _enginesPullPower = 0;
    //public float PullEnginesPower { get => _enginesPullPower; set { _enginesPullPower = value; } }
    private float _mobility = 0;
    public void SetMobility(float value)
    {
        _mobility = value;
        DefineRotateMobility();
    }
    //public float Mobility { get => _mobility; set { _mobility = value; DefineRotateMobility(); } }
    //[SerializeField]
    private float _rotateMobility = 0;


    private Rigidbody2D _rb;
    public Rigidbody2D Rb { get => _rb; }
    private Vector2 mousePosition;
    private Vector2 moveDirection_Raw;
    private float moveXraw = 0;
    private float moveYraw = 0;
    //private float lerp = 0;


    private void Awake()
    {
        if (_shipConfiguration == null)
            throw new NullReferenceException("shipConfiguration equals null");

        _rb = GetComponent<Rigidbody2D>();
        StartCoroutine(WaitFor());
    }

    private void ChangeEngineAnimationValues(bool value)
    {
        _shipConfiguration.SetAnimatorsEngineFire(value); // rework to Action
    }

    private void OnEnable()
    {
        ShipConfiguration._onChangedShipValues += UpdateValues;
    }

    private void OnDisable()
    {
        ShipConfiguration._onChangedShipValues -= UpdateValues;

        StopAllCoroutines();
    }

    private void Update()
    {
        //Log
        //DefineFields();
        //Log

        GetMoveAxis();
    }

    private void FixedUpdate()
    {
        Move();

        BodyRotate();
    }

    private void UpdateValues(float maxSpeed, float enginesPullPower, float mobility)
    {
        _maxSpeed = maxSpeed;
        _enginesPullPower = enginesPullPower;
        //_mobility = mobility;
        SetMobility(mobility);
    }

    private void DefineRotateMobility()
    {
        if (_mobility < 1)
        {
            float reverseMobility = 1 - _mobility;
            _rotateMobility = (360 - (reverseMobility * 360)) / (90 * reverseMobility) + 1;
            if (_rotateMobility > 180) _rotateMobility = 180;
        }
        else { _rotateMobility = 180; }
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

        _rb.rotation = Quaternion.RotateTowards(transform.rotation, q, _rotateMobility).eulerAngles.z;
    }

    private void Move()
    {
        if (moveDirection_Raw == Vector2.zero)
        {
            ChangeEngineAnimationValues(false);
            if (_rb.velocity.magnitude > 0)
            {
                // Vector2 vectorInertia = _rb.velocity.normalized * _inertiaFactor * Time.fixedDeltaTime;

                // if (_rb.velocity.magnitude - vectorInertia.magnitude < 0)
                //     _rb.velocity = Vector2.zero;
                // else
                //     _rb.velocity -= vectorInertia;
                if (_rb.velocity.magnitude <= 0.001f)
                    _rb.velocity = Vector2.zero;
                else
                    _rb.AddForce(-_rb.velocity.normalized * _inertiaFactor * 100 * Time.deltaTime, ForceMode2D.Force);
            }
        }
        else
        {
            ChangeEngineAnimationValues(true);

            //if (_rb.velocity.magnitude <= _maxSpeed)
            {
                //if (_rb.velocity.magnitude <= _maxSpeed)
                    _rb.AddForce(moveDirection_Raw * _enginesPullPower * Time.fixedDeltaTime, ForceMode2D.Force);

                if (_rb.velocity.magnitude > _maxSpeed)
                    _rb.AddForce(moveDirection_Raw * _enginesPullPower * Time.fixedDeltaTime, ForceMode2D.Force);

                // if (_rb.velocity.magnitude == 0)
                //     _rb.AddForce(moveDirection_Raw * _enginesPullPower * Time.fixedDeltaTime, ForceMode2D.Force);
                // else
                // {
                //     //float f = Mathf.Pow(_maxSpeed / _rb.velocity.magnitude, -1);
                //     float f = 1 - _rb.velocity.magnitude / _maxSpeed;
                //     Debug.Log(f);
                //     _rb.AddForce(moveDirection_Raw * _enginesPullPower * Time.fixedDeltaTime * f, ForceMode2D.Force);
                // }

            }
            // else if (_rb.velocity.magnitude == _maxSpeed)
            // {

            // }
        }

        if (pushBack != Vector2.zero)
        {
            pushBack *= 500;
            _rb.AddForce(pushBack, ForceMode2D.Force);
            pushBack = Vector2.zero;
        }
    }

    private Vector2 pushBack = Vector2.zero;
    public void AddVectorPushBack(Vector2 vector) => pushBack += vector;

    private IEnumerator WaitFor()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (_rb.velocity.magnitude > _maxSpeed && moveDirection_Raw != Vector2.zero)
            {
                // Vector2 vectorInertia = _rb.velocity.normalized * _inertiaFactor * Time.deltaTime;
                // _rb.velocity -= vectorInertia;

                // if (_rb.velocity.magnitude < _maxSpeed)
                //     _rb.velocity = _rb.velocity.normalized * _maxSpeed;

                //_rb.AddForce(-_rb.velocity.normalized * _inertiaFactor * Time.deltaTime, ForceMode2D.Force);
                _rb.AddForce(-_rb.velocity.normalized * _inertiaFactor * 100 * Time.deltaTime, ForceMode2D.Force);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!LogDrawLinesDirection)
            return;

        _rb = GetComponent<Rigidbody2D>();

        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        DrawLine(moveDirection_Raw);

        Gizmos.color = Color.red;
        DrawLine(_rb.velocity);


        //Debug.Log($"from {transform.position} to {moveDirection}");
        void DrawLine(Vector2 vector) => Gizmos.DrawLine(transform.position, position + vector);
    }

}
