using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private protected new string name;
    public string Name { get => name; }
    [SerializeField] private protected float mass;
    //[SerializeField] 

    public override string ToString()
    {
        return $"Object Name = {name} | mass = {mass}";
    }
}
