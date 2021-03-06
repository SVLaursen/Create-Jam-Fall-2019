﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : LivingEntity
{
	[SerializeField] private float moveSpeedDefault = 5;
	[SerializeField] private float moveSpeed = 5;
	[SerializeField] private GameObject deathSoundObject;

	private Camera _viewCamera;
	private PlayerMotor _motor;
	private GunController _gunController;

	[Header("Upgrades")]
	[SerializeField] private List<Upgrade> upgrades;

	public float MoveSpeed { get; set; }

	protected override void Start()
	{
		base.Start();

		_motor = new PlayerMotor(GetComponent<Rigidbody>());
		_gunController = GetComponent<GunController>();
		_viewCamera = Camera.main;
		MoveSpeed = moveSpeed;
	}

	private void Update()
	{
		// Movement input
		var moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		var moveVelocity = moveInput.normalized * MoveSpeed;
		_motor.SetVelocity(moveVelocity);

		// Look input
		var ray = _viewCamera.ScreenPointToRay(Input.mousePosition);
		var groundPlane = new Plane(Vector3.up, Vector3.up * _gunController.ShootingHeight);

		if (groundPlane.Raycast(ray, out var rayDistance))
		{
			var point = ray.GetPoint(rayDistance);
			//Debug.DrawLine(ray.origin,point,Color.red);
			_motor.LookAt(point);
		}

		// Weapon input
		if (Input.GetMouseButton(0))
			_gunController.OnTriggerHold();

		if (Input.GetMouseButtonUp(0))
			_gunController.OnTriggerRelease();
	}
	private void FixedUpdate() => _motor.Movement();

	public void AddUpgrade(Upgrade u)
	{
		if (u != null)
		{
			upgrades.Add(u);
			ApplyUpgrades();
		}
	}

	public void ApplyUpgrades()
	{
		SetStatsToDefault();

		if (upgrades.Count > 0)
		{
			foreach (Upgrade u in upgrades)
			{
				u.ApplyUpgrade(this, _gunController);
			}
		}
	}

	public void SetStatsToDefault()
	{
		MoveSpeed = moveSpeed;
		_gunController.SetStatsToDefault();
	}

	private void OnDisable()
	{
		GameMaster.Instance.PlayerDead();
		Destroy(Instantiate(deathSoundObject, transform.position, Quaternion.identity), 5f);
	}
}
