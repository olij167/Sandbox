﻿using UnityEngine;
using System;

[Serializable]
public enum DriveType
{
	RearWheelDrive,
	FrontWheelDrive,
	AllWheelDrive
}

public class WheelDrive : Interactable
{
	public bool beingDriven;
	public Transform driverSeat;

	[Tooltip("Maximum steering angle of the wheels")]
	public float maxAngle = 30f;
	[Tooltip("Maximum torque applied to the driving wheels")]
	public float maxTorque = 300f;
	[Tooltip("Maximum brake torque applied to the driving wheels")]
	public float brakeTorque = 30000f;
	[Tooltip("If you need the visual wheels to be attached automatically, drag the wheel shape here.")]
	public GameObject wheelShape;

	[Tooltip("The vehicle's speed when the physics engine can use different amount of sub-steps (in m/s).")]
	public float criticalSpeed = 5f;
	[Tooltip("Simulation sub-steps when the speed is above critical.")]
	public int stepsBelow = 5;
	[Tooltip("Simulation sub-steps when the speed is below critical.")]
	public int stepsAbove = 1;

	[Tooltip("The vehicle's drive type: rear-wheels drive, front-wheels drive or all-wheels drive.")]
	public DriveType driveType;

	private WheelCollider[] m_Wheels;
	private PlayerController playerController;

	// Find all the WheelColliders down in the hierarchy.
	void Start()
	{
		m_Wheels = GetComponentsInChildren<WheelCollider>();
		playerController = FindObjectOfType<PlayerController>();

		for (int i = 0; i < m_Wheels.Length; ++i)
		{
			var wheel = m_Wheels[i];

			// Create wheel shapes only when needed.
			if (wheelShape != null)
			{
				var ws = Instantiate(wheelShape);
				ws.transform.parent = wheel.transform;
				beingDriven = true;
			}
		}

		beingDriven = false;

	}

	// This is a really simple approach to updating wheels.
	// We simulate a rear wheel drive car and assume that the car is perfectly symmetric at local zero.
	// This helps us to figure our which wheels are front ones and which are rear.
	void Update()
	{
		if (beingDriven)
		{
			m_Wheels[0].ConfigureVehicleSubsteps(criticalSpeed, stepsBelow, stepsAbove);

			float angle = maxAngle * Input.GetAxis("Horizontal");
			float torque = maxTorque * Input.GetAxis("Vertical");

			float handBrake = Input.GetKey(KeyCode.X) ? brakeTorque : 0;

			foreach (WheelCollider wheel in m_Wheels)
			{
				// A simple car where front wheels steer while rear ones drive.
				if (wheel.transform.localPosition.z > 0)
					wheel.steerAngle = angle;

				if (wheel.transform.localPosition.z < 0)
				{
					wheel.brakeTorque = handBrake;
				}

				if (wheel.transform.localPosition.z < 0 && driveType != DriveType.FrontWheelDrive)
				{
					wheel.motorTorque = torque;
				}

				if (wheel.transform.localPosition.z >= 0 && driveType != DriveType.RearWheelDrive)
				{
					wheel.motorTorque = torque;
				}

			}
		}
        else
        {
			foreach (WheelCollider wheel in m_Wheels)
			{
				// A simple car where front wheels steer while rear ones drive.
				if (wheel.transform.localPosition.z > 0)
					wheel.steerAngle = 0;

				if (wheel.transform.localPosition.z < 0)
				{
					wheel.brakeTorque = brakeTorque;
				}

				if (wheel.transform.localPosition.z < 0 && driveType != DriveType.FrontWheelDrive)
				{
					wheel.motorTorque = 0;
				}

				if (wheel.transform.localPosition.z >= 0 && driveType != DriveType.RearWheelDrive)
				{
					wheel.motorTorque = 0;
				}

			}
		}
			foreach (WheelCollider wheel in m_Wheels)
			{
				// Update visual wheels if any.
				if (wheelShape)
				{
					Quaternion q;
					Vector3 p;
					wheel.GetWorldPose(out p, out q);

					// Assume that the only child of the wheelcollider is the wheel shape.
					Transform shapeTransform = wheel.transform.GetChild(0);

					if (wheel.name == "a0l" || wheel.name == "a1l" || wheel.name == "a2l")
					{
						shapeTransform.rotation = q * Quaternion.Euler(0, 180, 0);
						shapeTransform.position = p;
					}
					else
					{
						shapeTransform.position = p;
						shapeTransform.rotation = q;
					}
				}
			}
		
	}

	  public void EnterCar()
    {
        //playerController.characterControllerMovement = false;
        //playerController.GetComponent<CharacterController>().enabled = false;
        //playerController.GetComponent<BoxCollider>().enabled = false;
        Pause.instance.freezeMovement = true;

		GetComponent<Rigidbody>().isKinematic = false;

		//FindObjectOfType<ThirdPersonCam>().freezeOrientation = true;
		Pause.instance.freezeCameraOrbit = true;

		FindObjectOfType<ThirdPersonCam>().orientation.forward = transform.forward;

		playerController.transform.position = driverSeat.position;

        playerController.transform.parent = transform;

		playerController.animator.SetBool("isSitting", true);
		//playerController.animator.SetBool("isUsingBoth", true);
		playerController.animator.SetBool("isWalking", false);
		playerController.animator.SetBool("isRunning", false);

        //playerController.model.SetActive( false);
		

        beingDriven = true;
    }
    
    public void ExitCar()
    {
		//      playerController.characterControllerMovement = true;
		//      playerController.GetComponent<CharacterController>().enabled = true;
		//playerController.GetComponent<BoxCollider>().enabled = true;
		Pause.instance.freezeMovement = false;

		GetComponent<Rigidbody>().isKinematic = true;

		//FindObjectOfType<ThirdPersonCam>().freezeOrientation = false;
		Pause.instance.freezeCameraOrbit = false;

		playerController.transform.parent = null;
		//playerController.model.SetActive(true);
		playerController.animator.SetBool("isSitting", false);


		beingDriven = false;
    }

	public void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.GetComponent<EntityStats>())
		{
			playerController.GetComponent<PlayerAttack>().Attack(other.gameObject.GetComponent<EntityStats>(), GetComponent<Rigidbody>().velocity.magnitude * GetComponent<Rigidbody>().mass, false);
		}

		if (other.transform.GetComponent<Terrain>() && !beingDriven)
			GetComponent<Rigidbody>().isKinematic = true;

	}
}
