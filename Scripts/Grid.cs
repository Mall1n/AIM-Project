using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Grid : MonoBehaviour
{
    // Start is called before the first frame update
    // void Start()
    // {

    // }

    // // Update is called once per frame
    // void Update()
    // {

    // }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;
        for (int i = 1; i < 50; i++)
        {
            Gizmos.DrawLine(new Vector3(-100, i, 10), new Vector3(100, i, 10));
            Gizmos.DrawLine(new Vector3(-100, -i, 10), new Vector3(100, -i, 10));

            Gizmos.DrawLine(new Vector3(i, -100, 10), new Vector3(i, 100, 10));
            Gizmos.DrawLine(new Vector3(-i, -100, 10), new Vector3(-i, 100, 10));
        }


        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(0, -100, 10), new Vector3(0, 100, 10));

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(-100, 0, 10), new Vector3(100, 0, 10));
    }
}
