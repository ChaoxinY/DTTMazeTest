using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnMazeEventArgs : EventArgs
{
	public Vector2Int MazeDimensions;
	public MazeSpawnAlgorithmType MazeSpawnAlgorithmType;
	public SpawnMazeEventArgs(Vector2Int mazeDimensions, MazeSpawnAlgorithmType mazeSpawnAlgorithmType)
	{
		MazeDimensions = mazeDimensions;
		MazeSpawnAlgorithmType = mazeSpawnAlgorithmType;
	}
}

public enum MazeSpawnAlgorithmType
{
	BackTrackingRecursive = 0
}

public class MazeConfigurator : MonoBehaviour, IEventPublisher, IEventHandler
{
	#region Variables
	public event EventHandler<SpawnMazeEventArgs> SpawnMazeEvent;
	[SerializeField]
	private InputField mazeWidthInputField = default;
	[SerializeField]
	private InputField mazeHeightInputField = default;
	[SerializeField]
	private Dropdown mazeSpawnAlgorithmSelector = default;
	private Vector2Int mazeDimensions;
	private bool generationFinished;
	private MazeSpawnAlgorithmType mazeSpawnAlgorithmType;
	#endregion

	#region Initialization
	private void Awake()
	{
		StaticReferences.EventSubject.PublisherSubscribed += SubscribeEvent;
	}

	private void Start()
	{
		StaticReferences.EventSubject.Subscribe(this);
		mazeSpawnAlgorithmType = MazeSpawnAlgorithmType.BackTrackingRecursive;
		SetUpmazeSpawnerDropdown();
		generationFinished = true;
	}
	#endregion

	#region EventSystem Setup
	public void SubscribeEvent(object eventPublisher, PublisherSubscribedEventArgs publisherSubscribedEventArgs)
	{
		//SubScribe to the targeted eventPublisher with the same type
		if(publisherSubscribedEventArgs.Publisher.GetType() == typeof(MazeSpawner))
		{
			MazeSpawner mazeSpawner = (MazeSpawner)publisherSubscribedEventArgs.Publisher;
			mazeSpawner.MazeGenerationEnded +=  OnMazeGenerationFinished;
		}
	}

	public void UnSubScribeEvent()
	{
		StaticReferences.EventSubject.PublisherSubscribed -= SubscribeEvent;
		foreach(IEventPublisher eventPublisher in StaticReferences.EventSubject.EventPublishers)
		{
			if(eventPublisher.GetType() == typeof(MazeSpawner))
			{
				MazeSpawner mazeSpawner = (MazeSpawner)eventPublisher;
				mazeSpawner.MazeGenerationStarted -= OnMazeGenerationFinished;
			}
		}
	}
	public void UnSubscribeFromSubject()
	{
		StaticReferences.EventSubject.UnSubscribe(this);
	}

	private void OnDestroy()
	{
		UnSubScribeEvent();
		UnSubscribeFromSubject();
	}
	#endregion

	#region Functionality
	public void SpawnMaze()
	{
		if(generationFinished)
		{
			SpawnMazeEvent?.Invoke(this, new SpawnMazeEventArgs(mazeDimensions, mazeSpawnAlgorithmType));
			generationFinished = false;
		}
	}
	//set maze value with methods here
	public void SetMazeWidth()
	{
		int inputValue = ToolMethods.SetEvenNumberToOdd(ToolMethods.GetIntValueFromInputField(mazeWidthInputField));
		mazeDimensions = new Vector2Int(inputValue, mazeDimensions.y);
	}

	public void SetMazeHeight()
	{
		int inputValue = ToolMethods.SetEvenNumberToOdd(ToolMethods.GetIntValueFromInputField(mazeHeightInputField));
		mazeDimensions = new Vector2Int(mazeDimensions.x, inputValue);
	}

	private void SetUpmazeSpawnerDropdown()
	{
		List<string> mazeSpawnerDropdownOptions = new List<string>();
		foreach(MazeSpawnAlgorithmType type in Enum.GetValues(typeof(MazeSpawnAlgorithmType)))
		{
			mazeSpawnerDropdownOptions.Add(type.ToString());
		}
		//If there are multiple maze spawners availble we want to let the user see and select these options in the dropdown menu
		mazeSpawnAlgorithmSelector.AddOptions(mazeSpawnerDropdownOptions);
		mazeSpawnAlgorithmSelector.onValueChanged.AddListener(delegate { SetCurrentMazeCalculatingAlgorithm(mazeSpawnAlgorithmSelector.value); });
	}

	private void SetCurrentMazeCalculatingAlgorithm(int value)
	{
		mazeSpawnAlgorithmType = (MazeSpawnAlgorithmType)value;
	}

	private void OnMazeGenerationFinished(object sender, EventArgs eventArgs)
	{
		generationFinished = true;
	}
}
#endregion


