﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Object = System.Object;
using Random = UnityEngine.Random;

public class AiController : MonoBehaviour
{
	[SerializeField] private Enemy enemy;
	[SerializeField] private Enemy errorEnemy;
	[SerializeField] private Vector3[] spawnLocations;
	[SerializeField] private int enemiesToInstantiate;

	private List<Enemy> _activeEnemies = new List<Enemy>();
	private List<Enemy> _deactivatedEnemies = new List<Enemy>();

	private bool _spawning;
	private Player _player;

	public void StartWave(float spawnTime, int amountToSpawn)
	{
		StartCoroutine(SpawnEvery(spawnTime, amountToSpawn));
	}

	public bool AllEnemiesKilled()
	{
		return _activeEnemies.Count <= 0 && !_spawning;
	}

	private void Awake()
	{
		InstantiateEnemyObjects();
		_player = FindObjectOfType<Player>();
	}

	private void InstantiateEnemyObjects()
	{
		for (var i = 0; i < enemiesToInstantiate; i++)
		{
			var randomNum = Random.Range(0, 100);
			var instantiated = randomNum < 10 ? Instantiate(errorEnemy) : Instantiate(enemy);
			var instanceTransform = instantiated.transform;

			instantiated.gameObject.SetActive(false);
			instanceTransform.position = new Vector3(100, instantiated.transform.position.y, 100);
			instanceTransform.parent = transform;

			_deactivatedEnemies.Add(instantiated);
		}
	}

	private void ActivateEnemy()
	{
		int randomSpawn = Random.Range(0, spawnLocations.Length);

		if (ReferenceEquals(_deactivatedEnemies[0], null)) return;
		var entity = _deactivatedEnemies[0];

		entity.transform.position = spawnLocations[randomSpawn];
		entity.gameObject.SetActive(true);
		_deactivatedEnemies.RemoveAt(0);
		_activeEnemies.Add(entity);
	}

	public void DeactivateEnemy(Enemy enemy)
	{
		Debug.Log("Deactivated Enemy");
		for (var i = 0; i < _activeEnemies.Count; i++)
		{
			if (_activeEnemies[i] != enemy) continue;

			_activeEnemies.RemoveAt(i);
			_deactivatedEnemies.Add(enemy);

			enemy.transform.position = new Vector3(100, enemy.transform.position.y, 100);
			enemy.gameObject.SetActive(false);

			if (AllEnemiesKilled())
			{
				GameMaster.Instance.GetUpgradeManager().MakeUpgradesAvailable();

			}
		}
	}

	private IEnumerator SpawnEvery(float waitTime, int enemiesToSpawn)
	{
		_spawning = true;
		Debug.Log("Spawning");
		while (enemiesToSpawn > 0)
		{
			yield return new WaitForSeconds(waitTime);

			//if(!_player.gameObject.activeSelf)
			//yield break;

			ActivateEnemy();
			enemiesToSpawn--;
		}
		Debug.Log("NotSpawning");
		_spawning = false;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;

		foreach (var spawn in spawnLocations)
			Gizmos.DrawWireSphere(spawn, 1);
	}
}
