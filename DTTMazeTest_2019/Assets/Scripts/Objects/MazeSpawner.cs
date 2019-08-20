using UnityEngine;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;

public class MazeSpawner : MonoBehaviour, IEventHandler, IEventPublisher
{
	#region Variables
	public Func<Vector2Int, Task<List<Vector2Int>>> CalculateMaze;
	public event EventHandler MazeGenerationStarted;
	public event EventHandler MazeGenerationEnded;

	private const string CELL_MATERIAL_Path = "SurfaceMaterials/StarFruit";
	private const string FLOOR_MATERIAL_PATH = "SurfaceMaterials/Carpet";
	private const string MAZE_SPAWNER_NAME = "MazeSpawnPoint";
	private GameObject mazeSpawnPoint;
	private bool isMazeGenerating;
	#endregion

	#region Initialization
	//Has to subscribe to the eventsubject during the Awake phase of the Unity script lifecycle to receive publisher subscribed event.
	private void Awake()
	{
		StaticReferences.EventSubject.PublisherSubscribed += SubscribeEvent;
	}

	private void Start()
	{
		StaticReferences.EventSubject.Subscribe(this);
	}
	#endregion

	#region EventSystem Setup
	public void SubscribeEvent(object eventPublisher, PublisherSubscribedEventArgs publisherSubscribedEventArgs)
	{
		//SubScribe to the targeted eventPublisher with the same type
		if(publisherSubscribedEventArgs.Publisher.GetType() == typeof(MazeConfigurator))
		{
			MazeConfigurator mazeConfigurator = (MazeConfigurator)publisherSubscribedEventArgs.Publisher;
			mazeConfigurator.SpawnMazeEvent += SpawnMaze;
		}
	}

	public void UnSubScribeEvent()
	{
		StaticReferences.EventSubject.PublisherSubscribed -= SubscribeEvent;

		foreach(IEventPublisher eventPublisher in StaticReferences.EventSubject.EventPublishers)
		{
			if(eventPublisher.GetType() == typeof(MazeConfigurator))
			{
				MazeConfigurator mazeConfigurator = (MazeConfigurator)eventPublisher;
				mazeConfigurator.SpawnMazeEvent -= SpawnMaze;
			}
		}
	}

	public void UnSubscribeFromSubject()
	{
		StaticReferences.EventSubject.UnSubscribe(this);
	}

	private void OnDestroy()
	{
		UnSubscribeFromSubject();
		UnSubScribeEvent();
	}

	#endregion

	#region Functionality
	private async void SpawnMaze(object eventPublisher, SpawnMazeEventArgs spawnMazeEventArgs)
	{
		if(!isMazeGenerating)
		{
			isMazeGenerating = !isMazeGenerating;
			MazeGenerationStarted?.Invoke(this, new EventArgs());
			//SetUpMazeSpawnPoint
			SetUpMazeSpawnPoint();
			SpawnMazeWalls(spawnMazeEventArgs.MazeDimensions, mazeSpawnPoint.transform);
			SpawnMazeGround(spawnMazeEventArgs.MazeDimensions, mazeSpawnPoint.transform);

			DetermineCalculationAlgortihm(spawnMazeEventArgs.MazeSpawnAlgorithmType);
			List<Vector2Int> calculatedCellPositions = await CalculateMaze(spawnMazeEventArgs.MazeDimensions);
			StartCoroutine(SpawnCells(calculatedCellPositions, mazeSpawnPoint.transform));
		}
	}

	private void SetUpMazeSpawnPoint()
	{
		if(mazeSpawnPoint != null)
		{
			Destroy(mazeSpawnPoint);
		}
		mazeSpawnPoint = new GameObject();
		mazeSpawnPoint.gameObject.name = MAZE_SPAWNER_NAME;
		mazeSpawnPoint.transform.SetParent(transform);
	}

	private IEnumerator SpawnCells(List<Vector2Int> cellPositions, Transform parent)
	{
		foreach(Vector2 cellToSpawn in cellPositions)
		{
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = new Vector3(cellToSpawn.x, 0, cellToSpawn.y);
			//Given a colour so that the white + white combo doesnt burn your eyes out.
			cube.GetComponent<MeshRenderer>().material = Resources.Load(CELL_MATERIAL_Path) as Material;
			cube.transform.SetParent(parent);
			yield return new WaitForFixedUpdate();
		}
		isMazeGenerating = !isMazeGenerating;
		MazeGenerationEnded?.Invoke(this, new EventArgs());
	}

	private void DetermineCalculationAlgortihm(MazeSpawnAlgorithmType mazeSpawnAlgorithmType)
	{
		switch(mazeSpawnAlgorithmType)
		{
			case MazeSpawnAlgorithmType.BackTrackingRecursive:
				CalculateMaze = MazeCalculatingAlgorithms.CalculateRecursiveBacktrackingMaze;
				break;
		}
	}

	private void SpawnMazeWalls(Vector2Int mazeDimensions, Transform parent)
	{
		int mazeWidth = mazeDimensions.x;
		int mazeHeight = mazeDimensions.y;
		Vector3[] wallPositions = new Vector3[] { new Vector3(mazeWidth / 2f - 0.5f, 0, mazeHeight),
			new Vector3(mazeWidth / 2f - 0.5f, 0, -1f), new Vector3(-1, 0, mazeHeight / 2f - 0.5f),
			new Vector3(mazeWidth, 0, mazeHeight / 2f - 0.5f)};

		Vector3[] wallScales = new Vector3[] {new Vector3(mazeWidth + 2, 1, 1), new Vector3(mazeWidth + 2, 1, 1),
		 new Vector3(1, 1, mazeHeight + 2),new Vector3(1, 1, mazeHeight + 2)};

		for(int i = 0; i < 4; i++)
		{
			GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
			wall.transform.position = wallPositions[i];
			wall.transform.localScale = wallScales[i];
			wall.transform.SetParent(parent);
			wall.GetComponent<MeshRenderer>().material = Resources.Load(CELL_MATERIAL_Path) as Material;
		}
	}

	private void SpawnMazeGround(Vector2Int mazeDimensions, Transform parent)
	{
		int mazeWidth = mazeDimensions.x;
		int mazeHeight = mazeDimensions.y;
		GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
		floor.transform.position = new Vector3(mazeWidth / 2f - 0.5f, -1, mazeHeight / 2f - 0.2f);
		floor.transform.localScale = new Vector3(mazeWidth + 5f, 1, mazeHeight + 5f);
		floor.transform.SetParent(parent);
		floor.GetComponent<MeshRenderer>().material = Resources.Load(FLOOR_MATERIAL_PATH) as Material;
	}
	#endregion
}