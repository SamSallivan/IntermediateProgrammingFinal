using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController_Hammer: WeaponController
{
    public GameObject objWeapon;
    public Projectile bullet;
    public int rateOfFire = 1;
    private float fireInterval;
    private float fireCooldown;

    // Start is called before the first frame update
    void Start()
    {
        fireInterval = 1 / rateOfFire;
    }

    private void OnEnable()
    {
        if (!objWeapon.activeInHierarchy)
        {
            objWeapon.SetActive(true);
        }
        player.bob.Sway(new Vector4(0f, 0f, -5f, 2f));
    }

    // Update is called once per frame
    void Update()
    {
        if(fireCooldown > 0) {
            fireCooldown -= Time.deltaTime;
            return;
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Instantiate(bullet, transform.position + player.tHead.forward * 2, transform.rotation);
            fireCooldown = fireInterval;
        }

    }
}
