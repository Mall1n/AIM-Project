using UnityEngine;

public class Bullet : Item
{
    [SerializeField] [Range(0.1f, 50)] private float speed;

    public float Speed { get => speed; }
}
