using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject MuzzleFlash;
    [SerializeField] private float MuzzleFlashDuration;
    [SerializeField] private float ammoCost = 6f;

    public float AmmoCost
    {
        get { return ammoCost; }
        set { ammoCost = value; }
    }

    private float muzzleFlashTimer;
    private bool flashing = false;
    private void OnEnable()
    {
        MuzzleFlash.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!flashing)
            return;

        muzzleFlashTimer += Time.deltaTime;
        if (muzzleFlashTimer >= MuzzleFlashDuration)
        {
            MuzzleFlash.SetActive(false);
            flashing = false;
        }
    }

    public void Flash()
    {
        MuzzleFlash.SetActive(true);
        muzzleFlashTimer = 0;
        flashing = true;
    }
}
