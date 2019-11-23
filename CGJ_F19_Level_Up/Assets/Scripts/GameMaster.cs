﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
	[SerializeField] private AiController aiController;
	[SerializeField] private UpgradeManager upgradeManager;
	public Player player;
	[SerializeField] private int startSpawnAmount = 10;
	[SerializeField] private int currentSpawnAmount = 10;
	[SerializeField] private float exponentialGrowthRate = 2f;


	public int PlayerScore { get; set; }

	private UIMaster _uiMaster;

	#region Singleton
	public static GameMaster Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null)
			Destroy(this);

		Instance = this;
		_uiMaster = GetComponent<UIMaster>();
	}
	#endregion

	private void FixedUpdate()
	{
		if (aiController.AllEnemiesKilled())
		{
			aiController.StartWave(.1f, currentSpawnAmount);
			NewSpawnAmount();
		}
	}

	public void PlayerDead()
	{
		_uiMaster.GameOverUI();
	}

	public UpgradeManager GetUpgradeManager()
	{
		return upgradeManager;
	}

	private void NewSpawnAmount()
	{
		currentSpawnAmount = (int)Mathf.Pow(currentSpawnAmount, exponentialGrowthRate);
	}
}