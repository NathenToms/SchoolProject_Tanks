using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGunController : MonoBehaviour
{
	Animator animationController;

	// Start is called before the first frame update
	void Start()
	{
		animationController = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update()
	{
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");

		if (Input.GetKeyDown(KeyCode.Space))
		{
			animationController.SetBool("Attacking", true);
		}

		if (Input.GetKeyUp(KeyCode.Space))
		{
			animationController.SetBool("Attacking", false);
		}

		ChangeWeapon(new Vector2(x, y));

	}

	public void Shoot()
	{
		Debug.Log("Shoot");
	}

	public void ChangeWeapon(Vector2 input)
	{
		// UP
		if (input.x == 0 && input.y > 0)
		{
			animationController.SetTrigger("Reset");
			animationController.SetTrigger("Change_Charged");
		}

		// Down
		if (input.x == 0 && input.y < 0)
		{
			animationController.SetTrigger("Reset");
		}

		// RIGHT
		if (input.y == 0 && input.x < 0)
		{
			animationController.SetTrigger("Reset");
			animationController.SetTrigger("Change_Rockets");
		}

		// LEFT
		if (input.y == 0 && input.x > 0)
		{
			animationController.SetTrigger("Reset");
			animationController.SetTrigger("Change_Laser");
		}
	}
}
