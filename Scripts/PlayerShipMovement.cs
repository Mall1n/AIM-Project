using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[ExecuteAlways]
public class PlayerShipMovement : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5;
    [SerializeField][Range(0, 50)] private float acceleration = 0;
    public float Acceleration { set { acceleration = value; DefineAcceleration(); } }
    [SerializeField][Range(0.01f, 1f)] private float mobility = 0.6f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Camera _camera;

    //Log
    [SerializeField] private bool LogDrawLinesDirection = true;
    //Log


    private float _acceleration = 0;
    //[SerializeField]
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

        Application.targetFrameRate = 60; // delete

        DefineFields();
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
            ChangeDirection();

        Move();

        ObjectRotate(GetMousePosition());
    }

    private void DefineFields()
    {
        DefineAcceleration();

        DefineMobility();
    }

    private void DefineAcceleration() => _acceleration = acceleration / maxSpeed;

    private void DefineMobility()
    {
        if (mobility < 1)
        {
            float reverseMobility = 1 - mobility;
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

    private void ObjectRotate(float angle)
    {
        //rb.rotation = angle;

        Quaternion q = Quaternion.LookRotation(Vector3.forward, mousePosition);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, _rotateMobility);
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
