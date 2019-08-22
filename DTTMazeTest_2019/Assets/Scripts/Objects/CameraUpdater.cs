using UnityEngine;

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
			mazeSpawner.MazeGenerationEnded += UpdateCameraPosition;
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
				mazeSpawner.MazeGenerationEnded -= UpdateCameraPosition;
			}
		}
	}

	private void OnDestroy()
	{
		UnSubScribeEvent();
	}
	#endregion

	#region Functionality
	private void UpdateCameraPosition(object sender, MazeGenerationEventArgs mazeGenerationEndedEventArgs)
	{
		Vector3 cameraFocusPoint = ToolMethods.CalculateTransformCenterpoint(new Vector3(mazeGenerationEndedEventArgs.MazeDimensions.x, mazeSpawnPoint.transform.position.y, mazeGenerationEndedEventArgs.MazeDimensions.y));
		ToolMethods.CameraFocusOnPosition(focusCamera, cameraFocusPoint, mazeGenerationEndedEventArgs.MazeDimensions);
	}
	#endregion
}

