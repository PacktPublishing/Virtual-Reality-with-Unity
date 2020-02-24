using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK.Examples;
using UnityEngine.UI;
using VRTK; // VRTK_ControllerEvents

public class Handgun : GunShoot
{
    public Animator anim;

    public GameObject flashPrefab;

    public AudioClip shootSound;

    private int ammo;
    private int magazineSize = 10;

    public Text ammoText;

    public VRTK_ControllerEvents events;

    private bool reloading;

    void Start()
    {
        ammo = magazineSize;
        ammoText.text = ammo.ToString();
        reloading = false;

        events = GameObject.FindObjectOfType<VRTK_ControllerEvents>();
    }

    void Update()
    {
        if(events.buttonOnePressed && !reloading)
        {
            Reload();
        }
    }

    protected override void FireProjectile()
    {

        if((ammo > 0) && !reloading)
        {
            base.FireProjectile();

            anim.SetTrigger("Fire");

            var muzzleFlash = Instantiate(flashPrefab,
                                          projectileSpawnPoint.position,
                                          projectileSpawnPoint.rotation);

            Destroy(muzzleFlash, 0.3f);

            AudioSource.PlayClipAtPoint(shootSound, transform.position);

            // Debug.Log("Handgun Shot");

            ammo--;
            ammoText.text = ammo.ToString();

            if(ammo == 0)
            {
                Reload();
            }
        }
        
    }

    void Reload()
    {
        ammoText.text = "Reloading...";
        reloading = true;
        Invoke("FinishReload", 1.0f);
    }

    void FinishReload()
    {
        ammo = magazineSize;
        ammoText.text = ammo.ToString();
        reloading = false;
    }
}
