using Unity.VisualScripting;
using UnityEngine;

public class DestroyObjectEvent : MonoBehaviour
{
    public void DestroyThisObject() => Destroy(this.gameObject);
    
}
