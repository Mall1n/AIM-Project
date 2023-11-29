using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Engine : ItemShip
{
    [SerializeField] private protected float power;
    public float Power { get => power; set => power = value; }
    [SerializeField] private protected float energyUsage;
}
