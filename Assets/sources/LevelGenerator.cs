using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour {

	private const int OBSTACLES_POOL_SIZE = 25;
	private const int LEVEL_GENERATION_DEPTH = 10;

	[SerializeField]
	private Obstacle obstaclePrefab = null;

	private LinkedList<Obstacle> obstaclesPool = new LinkedList<Obstacle>();

	private Obstacle lastObstacle = null;

	public Obstacle.OnMoveDelegate OnMoveObstacle;

	private void Awake() {
		for (int i = 0; i < OBSTACLES_POOL_SIZE; i++) {
			AddObstacleToPool();
		}
	}

	private void AddObstacleToPool() {
		Obstacle obstacle = Instantiate(obstaclePrefab, Vector3.zero, Quaternion.identity) as Obstacle;
		obstaclesPool.AddLast(obstacle);
		obstacle.Initialize();
		obstacle.gameObject.SetActive(false);
	}

	private Obstacle GetObstacleFromPool() {
		Obstacle result = null;

		while (result == null) {
			if (obstaclesPool.Count == 0) {
				AddObstacleToPool();
			}

			result = obstaclesPool.Last.Value;
			obstaclesPool.RemoveLast();

			if (result == null) {
				continue;
			}

			result.OnReturnToPool += ReturnToPool;
			result.OnMove += MoveObstacle;
			result.gameObject.SetActive(true);
			result.SetupForVisualize();
		}

		return result;
	}

	private void MoveObstacle(Obstacle obstacle) {
		if (OnMoveObstacle != null) {
			OnMoveObstacle(obstacle);
		}
	}

	private void ReturnToPool(Obstacle obstacle) {
		if (obstacle == null) {
			return;
		}

		obstaclesPool.AddFirst(obstacle);
		obstacle.gameObject.SetActive(false);
		obstacle.OnReturnToPool -= ReturnToPool;
		obstacle.OnMove -= MoveObstacle;
	}

	private float[] pseudoRandomSideSeed = new float[] {
		-1f, 1f, 
	};
	private int index = 0;

	private void GenerateLevel() {
		for (int i = 0; i < LEVEL_GENERATION_DEPTH; i++) {
			Vector3 pos = new Vector3((lastObstacle == null ? Game.ScreenSizeWorld.x : lastObstacle.transform.position.x) + Config.ObstaclesDistance, 0f);

			float offset = Random.Range(Config.ObstaclesHeightMin, Config.ObstaclesHeightMax);
			float side = pseudoRandomSideSeed[index++];
			if (index >= pseudoRandomSideSeed.Length) {
				index = 0;
			}

			pos.y = (Game.ScreenSizeWorld.y + offset) * side;

			Obstacle obstacle = GetObstacleFromPool();
			obstacle.transform.position = pos;

			lastObstacle = obstacle;
		}
	}

	private void Update() {
		if (Game.StartTimer > 2f) {
			return;
		}

		if (lastObstacle == null || lastObstacle.transform.position.x <= Game.ScreenSizeWorld.x) {
			GenerateLevel();
		}
	}
}
