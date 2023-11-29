using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
// [ExecuteAlways]
public class PlayerShipMovement : MonoBehaviour
{
    [SerializeField][Range(0, 25)] private float maxSpeed = 5;
    [SerializeField][Range(0, 50)] private float acceleration = 0;
    public float Acceleration
    {
        get => acceleration;
        set
        {
            acceleration = value;
            DefineAcceleration();
        }
    }
    [SerializeField][Range(0.01f, 1f)] private float mobility = 0.6f;
    public float Mobility { get => mobility; set { mobility = value; DefineMobility(); } }
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Camera _camera;

    //Log
    [SerializeField] private bool LogDrawLinesDirection = true;
    //Log

    [SerializeField] // Log
    private float _acceleration = 0;
    private float _rotateMobility = 0;

    private Vector2 mousePosition;
    private Vector2 moveDirection_Raw;
    private Vector2 moveDirection;
    private Vector2 moveDirection_From;
    private float moveX_From = 0;
    private float moveY_From = 0;
    private float moveXraw = 0;
    private float moveYraw = 0;
    private float lerp = 0;
    private float factorDecreaseSpeed = 2;



    private void Start()
    {
        if (rb == null)
            throw new NullReferenceException("Rigidbody2D is null");

        DefineFields();
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
        if (CheckChangeDirection())
            ChangeDirection();

        Move();

        ObjectRotate();
    }

    private void DefineFields()
    {
        DefineAcceleration();

        DefineMobility();
    }

    private void DefineAcceleration() => _acceleration = acceleration / maxSpeed;

    private void DefineMobility()
    {
        if (Mobility < 1)
        {
            float reverseMobility = 1 - Mobility;
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

    private float GetMousePosition()
    {
        mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        return Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg - 90;
    }

    private void ObjectRotate()
    {
        GetMousePosition();

        Quaternion q = Quaternion.LookRotation(Vector3.forward, mousePosition);

        rb.rotation = Quaternion.RotateTowards(transform.rotation, q, _rotateMobility).eulerAngles.z;
        //rb.rotation = Quaternion.RotateTowards(transform.rotation, q, _rotateMobility);
    }

    private void Move()
    {
        lerp += Time.deltaTime;
        if (moveXraw == 0 && moveYraw == 0)
            moveDirection = Vector2.MoveTowards(moveDirection_From, moveDirection_Raw, lerp * _acceleration / factorDecreaseSpeed);
        else
            moveDirection = Vector2.MoveTowards(moveDirection_From, moveDirection_Raw, lerp * _acceleration);

        rb.velocity = new Vector2(moveDirection.x * maxSpeed, moveDirection.y * maxSpeed);
    }

    private bool CheckChangeDirection()
    {
        if (moveXraw != moveX_From || moveYraw != moveY_From)
            return true;
        return false;
    }

    private void ChangeDirection()
    {
        moveX_From = moveXraw;
        moveY_From = moveYraw;
        moveDirection_From = moveDirection;
        lerp = 0;
    }

    public void UpdateValues(float maxSpeed, float acceleration, float mobility)
    {
        this.maxSpeed = maxSpeed;
        Acceleration = acceleration;
        Mobility = mobility;
    }

    private void OnDrawGizmos()
    {
        if (!LogDrawLinesDirection)
            return;

        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        Gizmos.DrawLine(transform.position, position + moveDirection_Raw);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, position + moveDirection_From);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, position + moveDirection);

        //Debug.Log($"from {transform.position} to {moveDirection}");
    }

}
