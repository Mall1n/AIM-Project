using UnityEngine;

public class Gun : ItemShip
{
    [SerializeField] private float energyUsage;
    [SerializeField] private float damage;
    [SerializeField] private float accuracy; // Here

    [Header("Fire rate in minutte")]
    [SerializeField][Range(1, 5400)] private float fireRate;
    public float FireRate { get => fireRate; }


    [Header("Animation Fire")]
    [SerializeField] private Animator fireAnimator;
    public Animator FireAnimator { get => fireAnimator; }
    [SerializeField] private AudioSource audioFire;
    public AudioSource AudioFire { get => audioFire; }
}
