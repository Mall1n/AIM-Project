using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;

public class ShipConfiguration : MonoBehaviour
{
    [Header("Main Attributes")]
    //[SerializeField] private PlayerShipMovement playerShipMovement;
    //[SerializeField] private float mass = 0;
    [SerializeField][Range(0, 1)] private float mobilityModified = 0.5f; // readonly
    [SerializeField] private float gliderMass = 100f;
    [Range(0, 1)] private float mobility;
    public float Mobility { get => mobility; }
    private float powerEngines;
    private float maxSpeed;
    public float MaxSpeed { get => maxSpeed; }
    private float enginesAccelerationPower;
    public float EnginesAccelerationPower { get => enginesAccelerationPower; }
    private float fullMass = 0;
    public float FullMass { get => fullMass; }

    public Transform[] transformsEngine;
    public Engine[] engines; // make private
    public Transform[] transformsEasyGun;
    public Gun[] easyGuns; // make private
    public Transform[] transformsHeavyGun;
    public Gun[] heavyGuns; // make private
    public Shield shield; // make private
    [SerializeField] private Item[] inventory;

    public static Action onChangedShipValues;
    public static Action<float> onChangedShiledHP;


    private void Awake()
    {
        if (shield == null)
            shield = new Shield();
        FullSetShipValues();
    }

    private void Start()
    {
        onChangedShipValues?.Invoke();
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
        fullMass = GetFullShipMass();

        powerEngines = GetPowerEngines();

        maxSpeed = GetMaxSpeed();

        enginesAccelerationPower = GetEnginesAccelerationPower();

        mobility = GetMobility();
    }

    private float GetMobility() => mobilityModified - (mobilityModified * fullMass / 2500); 

    private float GetMaxSpeed() => powerEngines / 20;

    private float GetEnginesAccelerationPower()
    {
        const float modifierValue = 20;
        float powerEnginesModiried = powerEngines / modifierValue;
        return powerEnginesModiried - (powerEnginesModiried * fullMass / 1000);
    }

    private float GetFullShipMass()
    {
        float mass = 0;
        mass += gliderMass;
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
