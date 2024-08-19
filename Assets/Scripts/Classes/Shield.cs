using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : ItemShip
{
    [Header("Shield Settings")]
    [SerializeField] private protected float maxEnergy;
    public float MaxEnergy { get => maxEnergy; set => maxEnergy = value; }

    [SerializeField] private float energyUsage;
    public float EnergyUsage { get => energyUsage; set => energyUsage = value; }

    public Shield()
    {
        maxEnergy = 0;
        energyUsage = 0;
    }

}
