using UnityEngine;

public class Bullet : ItemInvertory
{
    [SerializeField][Range(0.1f, 50)] private float speed;
    public float Speed { get => speed; }
    [SerializeField] private float damage;
    [SerializeField] private Type bulletСaliber;
    public Type BulletСaliber { get => bulletСaliber; }

    public enum Type
    {
        caliber_13_45 = 1
    }
}
