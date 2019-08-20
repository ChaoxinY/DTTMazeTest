using System;
using UnityEngine;

public class SpawnMazeEventArgs : EventArgs
{
	public Vector2 MazeDimensions;
	public MazeSpawnAlgorithmType MazeSpawnAlgorithmType;
	public SpawnMazeEventArgs(Vector2 mazeDimensions, MazeSpawnAlgorithmType mazeSpawnAlgorithmType)
	{
		MazeDimensions = mazeDimensions;
		MazeSpawnAlgorithmType = mazeSpawnAlgorithmType;
	}
}

public enum MazeSpawnAlgorithmType
{
	BackTrackingRecursive = 0
}

public class MazeConfigurator : MonoBehaviour, IEventPublisher
{
	public event EventHandler<SpawnMazeEventArgs> SpawnMazeEvent;
	public Vector2 mazeDimensions;
	public MazeSpawnAlgorithmType mazeSpawnAlgorithmType;

	#region Initialization
	private void Start()
	{
		StaticReferences.EventSubject.Subscribe(this);
	}
	#endregion

	#region EventSystem Setup
	public void UnSubscribeFromSubject()
	{
		StaticReferences.EventSubject.UnSubscribe(this);
	}

	private void OnDestroy()
	{
		UnSubscribeFromSubject();
	}
	#endregion

	#region Functionality
	public void SpawnMaze()
	{
		SpawnMazeEvent?.Invoke(this, new SpawnMazeEventArgs(mazeDimensions, mazeSpawnAlgorithmType));
	}
	//set maze value with methods here
	#endregion
}

