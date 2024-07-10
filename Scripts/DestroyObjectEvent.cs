using System;
using System.Collections;
using UnityEngine;

public class DestroyObjectEvent : MonoBehaviour
{
    public void DestroyThisObject() => Destroy(this.gameObject);

    // public IEnumerator DestroyThisObjectInTime(float time)
    // {
    //     yield return new WaitForSeconds(time);
    //     Destroy(this.gameObject);
    // }

}
