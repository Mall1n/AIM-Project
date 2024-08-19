using UnityEngine;


public class Engine : ItemShip
{
    [Header("Engine stats")]
    [SerializeField] private protected float power;
    public float Power { get => power; set => power = value; }
    [SerializeField] private float energyUsage;


    [Header("Plasma animation")]
    [SerializeField] private Animator animatorEngineFire;
    private protected Animator AnimatorEngineFire { get => animatorEngineFire; }


    public void SetAnimatorEngineFire(bool value) => AnimatorEngineFire.SetBool("isMoving", value);


    //private PlayerShipMovement playerShipMovement;

}
