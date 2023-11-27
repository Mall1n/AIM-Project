using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private readonly ItemID.Gun gunID = 0;
    [SerializeField] private new string name;
    public string Name { get => name; }
    [SerializeField] private float power;
    public float Power { get => power; set => power = value; }
    [SerializeField] private float energyUsage;
    [SerializeField] private float mass;




    public override string ToString()
    {
        return $"Object Name = {transform.gameObject.name} | power = {power} | energyUsage = {energyUsage} | mass = {mass}";
    }
}
