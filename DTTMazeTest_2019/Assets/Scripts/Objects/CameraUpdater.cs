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
		if(publisherSubscribedEventArgs.Publisher.GetType() == typeof(MazeConfigurator))
		{
			MazeConfigurator mazeConfigurator = (MazeConfigurator)publisherSubscribedEventArgs.Publisher;
			mazeConfigurator.SpawnMazeEvent += UpdateCameraPosition;
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
				mazeConfigurator.SpawnMazeEvent -= UpdateCameraPosition;
			}
		}
	}

	private void OnDestroy()
	{
		UnSubScribeEvent();
	}
	#endregion

	#region Functionality
	private void UpdateCameraPosition(object sender, SpawnMazeEventArgs spawnMazeEventArgs)
	{
		Vector3 cameraFocusPoint = ToolMethods.CalculateTransformCenterpoint(new Vector3(spawnMazeEventArgs.MazeDimensions.x, mazeSpawnPoint.transform.position.y, spawnMazeEventArgs.MazeDimensions.y));
		ToolMethods.CameraFocusOnPosition(focusCamera, cameraFocusPoint, spawnMazeEventArgs.MazeDimensions);
	}
	#endregion
}

