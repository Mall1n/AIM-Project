using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : ItemShip
{
    [SerializeField] private protected float shieldPoints;
    public float ShieldPoints { get => shieldPoints; set => shieldPoints = value; }
}
