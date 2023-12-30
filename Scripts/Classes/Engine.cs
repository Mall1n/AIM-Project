using UnityEngine;


public class Engine : ItemShip
{
    [Header("Engine stats")]
    [SerializeField] private float power;
    public float Power { get => power; set => power = value; }
    [SerializeField] private float energyUsage;


    [Header("Plasma animation")]
    [SerializeField] private Animator animator;
    public Animator Animator { get => animator; }

    //private PlayerShipMovement playerShipMovement;

}
