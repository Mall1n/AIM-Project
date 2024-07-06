using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

public class ShipConfiguration : MonoBehaviour
{
    [Header("Main Attributes")]
    //[SerializeField] private PlayerShipMovement playerShipMovement;
    //[SerializeField] private float mass = 0;
    [SerializeField][Range(0, 1)] private float mobility;
    public Transform[] transformsEngine;
    public Engine[] engines; // make private
    public Transform[] transformsEasyGun;
    public Gun[] easyGuns; // make private
    public Transform[] transformsHeavyGun;
    public Gun[] heavyGuns; // make private
    [SerializeField] private Item[] inventory;
    private float fullMass = 0;



    private float health = 0.0f;

    public static Action<float, float, float> onChangedMovementValues;
    public static Action<float> onChangedHealth;


    private void Start()
    {
        //UpdateEmptyLists();

        fullMass = GetFullShipMass();
        FullSetShipValues();

        SetHealth(50);
    }

    public void SetInventoryCapacity(int capacity)
    {
        int capacityNow = inventory.Count(s => s != null);
        if (capacity >= capacityNow)
            Array.Resize(ref inventory, capacity);
        else
            throw new IndexOutOfRangeException($"Inventory capacity = {capacityNow} tried resize to {capacity}");
    }

#if UNITY_EDITOR

    public bool showDefaultInspector = true;
    public bool showEditor = true;

    // // FOR ENGINE
    // public List<Transform> transformsEngines;
    public List<int> IDEngines;

    // // FOR EASY GUNS
    // public List<Transform> transformEasyGuns;
    public List<int> IDEasyGuns;

    // // FOR HEAVY GUNS
    // public List<Transform> transformHeavyGuns;
    public List<int> IDHeavyGuns;

#endif


    public void FullSetShipValues()
    {
        float powerEngines = GetPowerEngines();

        float maxSpeed = powerEngines / 20;

        float _powerEngines = powerEngines / 25;
        _powerEngines -= _powerEngines * fullMass / 10000;

        float _mobility = mobility - (mobility * fullMass / 10000); // добавить зависимость от грузоподъёмности

        onChangedMovementValues?.Invoke(maxSpeed, _powerEngines, _mobility);
    }

    private float GetFullShipMass()
    {
        float mass = 0;
        foreach (var item in engines)
            mass += item.Mass;
        foreach (var item in easyGuns)
            mass += item.Mass;
        return mass;
    }


    public void SetAnimatorsEngineFire(bool value)
    {
        for (int i = 0; i < engines.Length; i++)
            if (engines[i] != null)
                engines[i].SetAnimatorEngineFire(value);
    }

    // private void DeleteEmpty<T>(List<T> list) => list.RemoveAll(s => s == null);

    // private void UpdateEmptyLists()
    // {
    //     DeleteEmpty(engines);
    //     DeleteEmpty(easyGuns);
    //     DeleteEmpty(heavyGuns);
    // }

    public float GetPowerEngines()
    {
        float result = 0;
        foreach (Engine item in engines)
        {
            if (item.Power < 0)
                throw new ArgumentOutOfRangeException($"Engine powel lower 0 ({item.Power})");

            result += item.Power;
        }
        return result;
    }

    private void SetHealth(float _health)
    {
        health = _health;
        onChangedHealth?.Invoke(_health);
    }

}
