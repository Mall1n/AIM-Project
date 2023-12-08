using System.Collections.Generic;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    public bool isLeftClicking => Input.GetMouseButton(0);
    private bool havingEasyGuns => easyGuns != null;

    [SerializeField] private List<Gun> easyGuns;
    private GameObject bulletObject;
    [SerializeField] private Rigidbody2D rb;
    private float timeClicking = 0;
    private float[] shotsFired;
    private float[] fireRateInFixedFrame;

    private readonly string triggerNameFire = "TriggerFire";

    void Start()
    {
        ShipConfiguration shipConfiguration = GetComponentInChildren<ShipConfiguration>(); // заменить на добавлениее из едитора
        easyGuns = shipConfiguration?.easyGuns;

        bulletObject = Resources.Load("Ship Assets/Bullets/Bullet Default", typeof(GameObject)) as GameObject;

        shotsFired = new float[easyGuns.Count];
        fireRateInFixedFrame = new float[easyGuns.Count];
        for (int i = 0; i < easyGuns.Count; i++)
        {
            fireRateInFixedFrame[i] = 60 / easyGuns[i].FireRate;
        }
    }

    void Update()
    {
        GetMouseInput();
    }



    private void FixedUpdate()
    {
        EasyGunFire();
    }

    private void EasyGunFire()
    {
        if (isLeftClicking && havingEasyGuns)
        {
            timeClicking += Time.fixedDeltaTime;
            for (int i = 0; i < easyGuns.Count; i++)
                if (timeClicking > fireRateInFixedFrame[i] * shotsFired[i])
                    InitShot(i);
        }
    }

    private void InitShot(int index)
    {
        Transform transformFirePoint = easyGuns[index].GetComponentInChildren<Transform>();
        GameObject _bulletObject = Instantiate(this.bulletObject, transformFirePoint.position, transformFirePoint.rotation);
        Bullet bullet = _bulletObject.GetComponent<Bullet>();

        Vector2 force = transformFirePoint.up * (float)bullet?.Speed;
        if (rb)
            force += rb.velocity;
        AddForce(transformFirePoint.up * (float)bullet?.Speed);

        //if (easyGuns[index].FireAnimator != null)
            easyGuns[index].FireAnimator?.SetTrigger(triggerNameFire);

        //if (easyGuns[index].AudioFire != null)
            easyGuns[index].AudioFire?.Play();

        shotsFired[index] += 1;

        void AddForce(Vector2 force) => _bulletObject.GetComponent<Rigidbody2D>()?.AddForce(force, ForceMode2D.Impulse); 
    }

    private void GetMouseInput()
    {
        if (Input.GetMouseButtonUp(0))
        {
            timeClicking = 0;
            shotsFired = new float[easyGuns.Count];
        }
    }
}
