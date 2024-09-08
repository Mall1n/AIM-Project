using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
// [ExecuteAlways]
public class PlayerShipMovement : MonoBehaviour
{
    public static Action<bool> OnPlayerMove;

    [Header("Main Attributes")]
    [SerializeField] private Camera _camera;

    //Log
    [Header("Log Attributes")]
    [SerializeField] private bool LogDrawLinesDirection = true;
    //Log


    [SerializeField][Range(0, 50)] float _inertiaFactor = 5;

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
    private IEnumerator _fixedLateUpdate;


    private void Awake()
    {
        _fixedLateUpdate = FixedLateUpdate();

        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        ShipConfiguration._onChangedShipValues += UpdateValues;
        StartCoroutine(_fixedLateUpdate);
    }

    private void OnDisable()
    {
        ShipConfiguration._onChangedShipValues -= UpdateValues;
        StopCoroutine(_fixedLateUpdate);
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
        //Debug.Log("FixedUpdate");
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

        if (_moveDirection_Raw != Vector2.zero)
            OnPlayerMove?.Invoke(true);
        else
            OnPlayerMove?.Invoke(false);
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
        if (_moveDirection_Raw != Vector2.zero)
            _rb.AddForce(_moveDirection_Raw * _enginesPullPower * Time.fixedDeltaTime, ForceMode2D.Force);
    }

    private IEnumerator FixedLateUpdate()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            SpeedLimit();

            Inertia();
            //Debug.Log("FixedLateUpdate");
        }
    }

    private void SpeedLimit()
    {
        if (_moveDirection_Raw != Vector2.zero)
        {
            _rb.AddForce(-_rb.velocity.normalized * (_enginesPullPower * _ratioSpeed) * Time.fixedDeltaTime, ForceMode2D.Force);
        }
        else
        {
            if (_ratioSpeed > 1)
                _rb.AddForce(-_rb.velocity.normalized * (_enginesPullPower * (_ratioSpeed - 1)) * Time.fixedDeltaTime, ForceMode2D.Force);
        }
    }

    private void Inertia()
    {
        if (_rb.velocity.magnitude <= 0)
            return;

        if (_ratioSpeed > 1)
        {
            AddForce();
        }
        else if (_moveDirection_Raw == Vector2.zero)
        {
            if (_rb.velocity.magnitude <= 0.001f)
                _rb.velocity = Vector2.zero;
            else
                AddForce();
        }

        void AddForce() => _rb.AddForce(-_rb.velocity.normalized * _inertiaFactor * Time.fixedDeltaTime, ForceMode2D.Force);
    }
    public void AddVectorPushBack(Vector2 vectorPush) => _rb.AddForce(vectorPush * 0.005f, ForceMode2D.Impulse);

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
