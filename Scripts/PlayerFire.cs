using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    public bool isLeftClicking => Input.GetMouseButton(0);

    [SerializeField] private List<Gun> easyGuns;
    private GameObject bulletObject;

    void Start()
    {
        ShipConfiguration shipConfiguration = GetComponentInChildren<ShipConfiguration>(); // заменить на добавлениее из едитора
        easyGuns = shipConfiguration?.easyGuns;

        bulletObject = Resources.Load("Ship Assets/Bullets/Bullet Default", typeof(GameObject)) as GameObject;
        //bullet.GetComponent<Rigidbody2D>()?.AddForce(new Vector2(0, 10));
    }

    void Update()
    {
        //GetMouseInput();
    }

    private void FixedUpdate()
    {
        if (isLeftClicking)
        {
            for (int i = 0; i < easyGuns.Count; i++)
            {
                Transform transformFirePoint = easyGuns[i].GetComponentInChildren<Transform>();
                GameObject _bullet = Instantiate(this.bulletObject, transformFirePoint.position, transformFirePoint.rotation);
                _bullet.GetComponent<Rigidbody2D>()?.AddForce(transformFirePoint.up * 5, ForceMode2D.Impulse);
            }
        }
    }

    private void GetMouseInput()
    {

    }
}
