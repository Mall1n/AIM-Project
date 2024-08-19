using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class ShipConfiguration : MonoBehaviour
{
    [Header("Main Attributes")]
    //[SerializeField] private PlayerShipMovement playerShipMovement;
    //[SerializeField] private float mass = 0;
    [SerializeField][Range(0, 1)] private float mobilityModifier = 0.5f; // readonly
    [SerializeField] private float _gliderMass = 100;
    [SerializeField][Range(0, 5)] private float _maxSpeedBase = 2;
    [SerializeField] private float _inventoryCapacity = 500;
    private float _ratioCapacity => GetFullShipMass() / _inventoryCapacity;
    private float _maxSpeed;
    public float MaxSpeed { get => _maxSpeed; }
    [Range(0, 1)] private float _mobility;
    public float Mobility { get => _mobility; }
    private float _powerEngines;
    private float _enginesPullPower;
    public float EnginesPullPower { get => _enginesPullPower; }
    private float _fullMass;
    public float FullMass { get => _fullMass; }
    private float _equipmentMass;

    [Header("Components")]
    public Transform[] transformsEngine;
    public Engine[] engines; // make private
    public Transform[] transformsEasyGun;
    public Gun[] easyGuns; // make private
    public Transform[] transformsHeavyGun;
    public Gun[] heavyGuns; // make private
    public Shield shield; // make private
    [SerializeField] private Item[] inventory;

    public static Action<float, float, float> _onChangedShipValues;
    public static Action<float> _onChangedShiledHP; // delete


    private void Awake()
    {
        if (shield == null)
            shield = new Shield();

        FullSetShipValues();
        if (_ratioCapacity > 1)
            Debug.LogWarning($"Ratio Capacity grather then 100% | {this.transform.name}");
    }

    private void Start()
    {
        _onChangedShipValues?.Invoke(_maxSpeed, _enginesPullPower, _mobility);
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

    // FOR HEAVY GUNS
    public int IDShield;


#endif


    public void FullSetShipValues()
    {
        _equipmentMass = GetEquipmentMass();

        _fullMass = GetFullShipMassFast();

        _powerEngines = GetPowerEngines();

        _maxSpeed = GetMaxSpeed();

        _enginesPullPower = GetEnginesAccelerationPower();

        _mobility = GetMobility();
    }

    private float GetMobility() => mobilityModifier - (mobilityModifier * 0.75f * _ratioCapacity);

    private float GetMaxSpeed() => _maxSpeedBase - (_maxSpeedBase * 0.75f * _ratioCapacity);

    private float GetEnginesAccelerationPower()
    {
        //const float modifierValue = 20;
        //float powerEnginesModiried = _powerEngines / modifierValue;
        return _powerEngines - (_powerEngines * _fullMass / 1000);
    }

    private float GetEquipmentMass()
    {
        float mass = 0;
        mass += ForeachReturnValue(engines);
        mass += ForeachReturnValue(easyGuns);
        mass += ForeachReturnValue(heavyGuns);
        mass += shield.Mass;
        return mass;


        float ForeachReturnValue(ItemShip[] list)
        {
            float resultValue = 0;
            foreach (var item in list)
                if (item != null)
                    resultValue += item.Mass;
            return resultValue;
        }
    }

    private float GetFullShipMass()
    {
        float mass = 0;
        mass += _gliderMass;
        mass += GetEquipmentMass();
        return mass;
    }

    private float GetFullShipMassFast()
    {
        float mass = 0;
        mass += _gliderMass;
        mass += _equipmentMass;
        return mass;
    }

    public void SetAnimatorsEngineFire(bool value)
    {
        for (int i = 0; i < engines.Length; i++)
            if (engines[i] != null)
                engines[i].SetAnimatorEngineFire(value);
    }

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

    public float GetHighValueShield()
    {
        return shield.MaxEnergy;
    }

}
