using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    public GameObject target;

	public float maxDist = 20f;

	public float timer;

	public float speed = 30f;

	public Vector3 dir;

	public Vector3 startPos;

	public Vector3 targetPos;

	public Vector4 startSway = new Vector4(5f, 0f, 0f, 3f);

	public RaycastHit hit;

	public Weapon targetWeapon;

	public PlayerController player { get; private set; }

	public bool isDashing;

	public int state;

	private void Awake()
	{
		player = GetComponent<PlayerController>();
	}

	public void Reset()
	{
		player.rb.isKinematic = false;
		player.headPosition.Slide(0.75f);
		isDashing = false;
		state = 0;
	}

    public void Update(){

		if (!PlayerController.instance)
		{
			return;
		}
		maxDist = 9f;
		float num = 30f;
		float num2 = 0f;
		int index = -1;

        Weapon[] allWeapons = FindObjectsOfType<Weapon>();

		for (int i = 0; i < allWeapons.Length; i++)
		{
			if (!allWeapons[i].isActiveAndEnabled)
			{
				continue;
			}
			float dist = Vector3.Distance(PlayerController.instance.t.position, allWeapons[i].t.position);
			if (!(dist < maxDist))
			{
				continue;
			}
			bool grounded = PlayerController.instance.grounder.grounded;
            Vector3 playerPos =  PlayerController.instance.tHead.position;
			Vector3 to = (allWeapons[i].t.position - playerPos).normalized;
			num2 = Vector3.Angle(PlayerController.instance.tHead.forward, to);
			if (num2 < num)
			{
				num = num2;
				if (index != i)
				{
					index = i;
				}
			}
		}
		if (index > -1)
		{
			Debug.DrawLine(PlayerController.instance.t.position, allWeapons[index].t.position, Color.cyan);
            targetWeapon = allWeapons[index];

			if((bool)targetWeapon.GetComponent<FloatingWeapon>()){
				target.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
				target.transform.position = allWeapons[index].transform.position + Vector3.up * 0.25f;
			}
			else if((bool)targetWeapon.GetComponent<ThrowedWeapon>()){
				target.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
				target.transform.position = allWeapons[index].transform.position + Vector3.up * 0.5f;
			}
		}
        else{
            if(!isDashing)
                targetWeapon = null;
			target.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        }
    }

	public bool Dash()
	{
		if (targetWeapon == null)
		{
			return false;
		}
		if (!isDashing && state == 0)
		{
			Physics.Raycast(targetWeapon.t.position, Vector3.down, out hit, (player.grounder.grounded) ? 10 : 1, 1);
			if (hit.distance != 0f) //got ground beneath
			{
				targetPos = hit.point + Vector3.up; //dashes to the ground point
			}
			else//not close to ground
			{
				targetPos = targetWeapon.t.position; //dashes to weapon in air
			}
			state = 3;
			isDashing = true;
			targetWeapon.rb.isKinematic = true;
			return true;
		}
		return false;
	}

	public void DashingUpdate()
	{
		switch (state)
		{
		case 3:
			if (player.grounder.grounded)
			{
				player.grounder.Unground();
			}
			player.weapons.Drop(Vector3.up + player.tHead.forward);
			player.rb.isKinematic = true;
			startPos = player.rb.position;
			dir = (targetPos - startPos).normalized;
			speed = Mathf.Abs((8f - Vector3.Distance(player.t.position, targetPos) / 2f));
			speed = Mathf.Clamp(speed, 2f, 8f);
			player.headPosition.Slide(0f);
			//player.fov.kinematicFOV = 15f;
			player.bob.Sway(startSway);
			timer = 0f;
			state--;
			break;
		case 2:
			timer = Mathf.MoveTowards(timer, 1f, Time.deltaTime * speed);
			player.t.position = Vector3.Lerp(startPos, targetPos, timer - 0.2f);
			player.bob.Angle(0f);
			if (timer == 1f)
			{
				state--;
			}
			break;
		case 1:
			player.rb.isKinematic = false;
			player.rb.AddForce(dir * 20f, ForceMode.Impulse);
			player.airControl = 1;
			player.airControlBlockTimer = 0.2f;
			player.headPosition.Slide(0.75f);
			targetWeapon.Interact(player.weapons);
			isDashing = false;
			state--;
			break;
		}
	}
}
