using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    #region Singleton
    public static UIController instance;
    
    private void Awake()
    {
        instance = this;
    }
    #endregion

    [SerializeField] private Image healthBar;
    [SerializeField] private Image batteryCharge;

    public Image HealthBar
    {
        get { return healthBar; }
        set { healthBar = value; }
    }

    public Image BatteryCharge
    {
        get { return batteryCharge; }
        set { BatteryCharge = value; }
    }
}
