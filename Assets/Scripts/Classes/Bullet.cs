using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bullet : ItemInvertory
{
    [SerializeField][Range(0.1f, 50)] private float speed;
    public float Speed { get => speed; }
    [SerializeField] private float damage;
    [SerializeField] private CaliberType bulletСaliber;
    public CaliberType BulletСaliber { get => bulletСaliber; }
    private const float destroyObjectTime = 10.0f;

    public enum CaliberType
    {
        caliber_13_45 = 1
    }



    private void Start()
    {
        StartCoroutine(DestroyObjectInTime());
    }

    private IEnumerator DestroyObjectInTime()
    {
        yield return new WaitForSeconds(destroyObjectTime);
        if (this.gameObject != null)
            DestroyThis();
        //Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (this.gameObject != null)
            DestroyThis();
        //Destroy(this.gameObject);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void DestroyThis()
    {
        StopCoroutine(DestroyObjectInTime());
        Destroy(this.gameObject);
    }
}
