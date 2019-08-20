using UnityEngine;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;

public class MazeSpawner : MonoBehaviour,  IEventHandler
{
	public Func<Vector2, Task<List<Vector2>>> CalculateMaze;

	#region Initialization
	//Has to subscribe to the eventsubject during the Awake phase of the Unity script lifecycle to receive publisher subscribed event.
	private void Awake()
	{
		StaticReferences.EventSubject.PublisherSubscribed += SubscribeEvent;
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

	private void OnDestroy()
	{
		UnSubScribeEvent();
	}

	#endregion

	#region Functionality
	private async void SpawnMaze(object eventPublisher, SpawnMazeEventArgs spawnMazeEventArgs)
	{
		DetermineCalculationAlgortihm(spawnMazeEventArgs.MazeSpawnAlgorithmType);
		List<Vector2> calculatedCellPositions = await CalculateMaze(spawnMazeEventArgs.MazeDimensions);
		StartCoroutine(SpawnCells(calculatedCellPositions, transform));
	}

	private IEnumerator SpawnCells(List<Vector2> cellPositions, Transform parent)
	{
		foreach(Vector2 cellToSpawn in cellPositions)
		{
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = new Vector3(cellToSpawn.x, 0, cellToSpawn.y);
			//Given a colour so that the white + white combo doesnt burn your eyes out.
			cube.GetComponent<MeshRenderer>().material = Resources.Load("SurfaceMaterials/Peach") as Material;
			cube.transform.SetParent(parent);
			yield return new WaitForFixedUpdate();
		}
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
	#endregion
}

