using UnityEngine;

public class Core : ItemShip
{
    [SerializeField] private protected float power;
    public float Power { get => power; set => power = value; }
}
