using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private int _FPSRate = 60;
    public int FPSRate { get => _FPSRate; set => _FPSRate = value; }


    private int _FPSRateDefault = 60;


    void Start()
    {
        if (_FPSRate > 0)
            Application.targetFrameRate = _FPSRate;
        else
            Application.targetFrameRate = _FPSRateDefault;
    }
}
