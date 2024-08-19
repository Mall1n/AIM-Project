using UnityEngine;

public class ShipSpot : MonoBehaviour
{
    [SerializeField] private ItemShip.Type type;
    public ItemShip.Type Type { get => type; }
    // [SerializeField] private bool leftSide;
    // public bool LeftSide { get => leftSide; }
    [SerializeField] private ItemShip.Side side = 0;
    public ItemShip.Side Side { get => side; set { side = value; } }
}
