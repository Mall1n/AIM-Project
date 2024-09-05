using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
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
    private float _mobility = 0;
    public void SetMobility(float value)
    {
        _mobility = value;
        DefineRotateMobility();
    }
    //[SerializeField]
    private float _rotateMobility = 0;


    private Rigidbody2D _rb;
    public Rigidbody2D Rb { get => _rb; }
    private Vector2 _mousePosition;
    private Vector2 _moveDirection_Raw;
    private float _moveXraw = 0;
    private float _moveYraw = 0;
    private float _ratioSpeed => _rb.velocity.magnitude / _maxSpeed;


    private void Awake()
    {
        if (_shipConfiguration == null)
            throw new NullReferenceException("shipConfiguration equals null");

        _rb = GetComponent<Rigidbody2D>();
        _inertiaFactor *= 100;
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
        _moveXraw = Input.GetAxisRaw("Horizontal");
        _moveYraw = Input.GetAxisRaw("Vertical");

        _moveDirection_Raw = new Vector2(_moveXraw, _moveYraw).normalized;
    }

    private Vector2 GetMousePosition()
    {
        return _camera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        //return Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg - 90;
    }

    private void BodyRotate()
    {
        _mousePosition = GetMousePosition();
        Quaternion q = Quaternion.LookRotation(Vector3.forward, _mousePosition);
        _rb.rotation = Quaternion.RotateTowards(transform.rotation, q, _rotateMobility).eulerAngles.z;
    }

    private void Move()
    {
        if (_moveDirection_Raw == Vector2.zero)
        {
            ChangeEngineAnimationValues(false);
            if (_rb.velocity.magnitude > 0)
            {
                if (_rb.velocity.magnitude <= 0.001f)
                    _rb.velocity = Vector2.zero;
                else
                    _rb.AddForce(-_rb.velocity.normalized * _inertiaFactor * Time.deltaTime, ForceMode2D.Force);
            }
        }
        else
        {
            ChangeEngineAnimationValues(true); // Rework to Action
            _rb.AddForce(_moveDirection_Raw * _enginesPullPower * Time.fixedDeltaTime, ForceMode2D.Force);
        }
    }

    private void LateUpdate()
    {
        if (_moveDirection_Raw != Vector2.zero)
        {
            _rb.AddForce(-_rb.velocity * (_enginesPullPower * _ratioSpeed) * Time.fixedDeltaTime, ForceMode2D.Force);
        }
    }

    //private Vector2 pushBack = Vector2.zero;
    //public void AddVectorPushBack(Vector2 vector) => pushBack += vector;
    public void AddVectorPushBack(Vector2 vectorPush) => _rb.AddForce(vectorPush / 100, ForceMode2D.Impulse);

    private void OnDrawGizmos()
    {
        if (!LogDrawLinesDirection)
            return;

        _rb = GetComponent<Rigidbody2D>();

        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        DrawLine(_moveDirection_Raw);

        Gizmos.color = Color.red;
        DrawLine(_rb.velocity);


        //Debug.Log($"from {transform.position} to {moveDirection}");
        void DrawLine(Vector2 vector) => Gizmos.DrawLine(transform.position, position + vector);
    }

}
