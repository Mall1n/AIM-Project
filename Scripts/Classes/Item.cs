using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item Info")]
    [SerializeField] private protected new string name;
    public string Name { get => name; }
    [SerializeField] private protected float mass;

}
