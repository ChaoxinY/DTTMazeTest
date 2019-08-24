using UnityEngine;
using System.Collections;
using System;

public class CameraUpdater: MonoBehaviour, IEventHandler
{
	#region Variables
	[SerializeField]
	private Camera focusCamera = default;
	[SerializeField]
	private Transform mazeSpawnPoint = default;
	#endregion

	#region Initialization
	private void Awake()
	{
		StaticReferences.EventSubject.PublisherSubscribed += SubscribeEvent;
	}
	#endregion

	#region EventSystem Setup
	public void SubscribeEvent(object eventPublisher, PublisherSubscribedEventArgs publisherSubscribedEventArgs)
	{
		//SubScribe to the targeted eventPublisher with the same type
		if(publisherSubscribedEventArgs.Publisher.GetType() == typeof(MazeSpawner))
		{
			MazeSpawner mazeSpawner = (MazeSpawner)publisherSubscribedEventArgs.Publisher;
			mazeSpawner.MazeGenerationStarted += OnMazeGenerationStarted;
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
				mazeSpawner.MazeGenerationStarted -= OnMazeGenerationStarted;
			}
		}
	}

	private void OnDestroy()
	{
		UnSubScribeEvent();
	}
	#endregion

	#region Functionality
	private void OnMazeGenerationStarted(object sender, MazeGenerationEventArgs mazeGenerationEventArgs)
	{
		Vector3 cameraFocusPoint = ToolMethods.CalculateTransformCenterpoint(new Vector3(mazeGenerationEventArgs.MazeDimensions.x, mazeSpawnPoint.transform.position.y, mazeGenerationEventArgs.MazeDimensions.y));
		cameraFocusPoint = ToolMethods.CalculateFocusPosition(cameraFocusPoint, mazeGenerationEventArgs.MazeDimensions);
		focusCamera.transform.position = cameraFocusPoint;
	}
	#endregion
}

