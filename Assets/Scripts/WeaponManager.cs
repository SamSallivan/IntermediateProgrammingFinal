using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that manages and switches all the potential weapons player could accuquire.
//currently there are only the Bat and Slap.
public class WeaponManager : MonoBehaviour
{
	//a list of weapon controllers.
	public WeaponController[] weapons;

	//slap controller is different from all other weapons,
	//for it activates when no weapon is present.
	public SlapController slapController { get; private set; }

	public PlayerController p;

	public int currentWeapon;

	//list of prefabs to be created when a weapon is dropped or thrown.
    public GameObject[] weaponDrops;

	private void Awake()
	{
		weapons = GetComponentsInChildren<WeaponController>(true);
		slapController = GetComponentInChildren<SlapController>();
		p = GetComponentInParent<PlayerController>();
		currentWeapon = -1;
		Deactivate();
	}

	//deactivates the current weapon, 
	//creates a weapon prefab, and throw in given direction.
	public void Drop(Vector3 dir)
	{
		if (currentWeapon > -1)
		{
			Weapon drop = GameObject.Instantiate(weaponDrops[currentWeapon], p.tHead.position, Quaternion.LookRotation(p.tHead.right)).GetComponent<Weapon>();
            drop.Drop(dir * 8f, -90f);
            //((PooledWeapon)QuickPool.instance.Get(weapons[currentWeapon].name, p.tHead.position, Quaternion.LookRotation(p.tHead.right))).Drop(dir * 8f, -90f);
		}
		Pick(-1);
	}

	//receives input for slap or weapons.
	private void Update()
	{
		slapController.SlapInputUpdate();
		if (currentWeapon > -1)
		{
			weapons[currentWeapon].WeaponInputeUpdate();
			weapons[currentWeapon].animator.SetBool("Sliding", p.slide.slideState != 0);
		}

	}

	//returns the charged time of current weapon.
	public float Holding()
	{
		if (currentWeapon <= -1)
		{
			return 0;
		}
		return weapons[currentWeapon].holding;
	}

	public bool IsBlocking()
	{
		if (currentWeapon <= -1)
		{
			return false;
		}
		return weapons[currentWeapon].isBlocking;
	}

	//equips the given index of weapon.
	public void Pick(int index)
	{
		currentWeapon = index;
		Refresh();
	}

	//activates the current weapon's controller.
	public void Refresh()
	{
		for (int i = 0; i < weapons.Length; i++)
		{
			if (i == currentWeapon)
			{
				weapons[i].gameObject.SetActive(true);
			}
			else
			{
				weapons[i].gameObject.SetActive(false);
			}
		}
	}

	//deactivates all weapon controllers.
	public void Deactivate()
	{
		for (int i = 0; i < weapons.Length; i++)
		{
			weapons[i].gameObject.SetActive(false);
		}
	}
}

