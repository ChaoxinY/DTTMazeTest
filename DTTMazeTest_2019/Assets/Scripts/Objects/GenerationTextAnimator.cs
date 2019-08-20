using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class GenerationTextAnimator : MonoBehaviour, IEventHandler
{
	#region Variables
	public Text displayText;
	private bool generationFinished;
	#endregion

	#region Initialization
	private void Awake()
	{
		displayText.gameObject.SetActive(false);
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
			mazeSpawner.MazeGenerationStarted += OnMazeGenerating;
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
				mazeSpawner.MazeGenerationStarted -= OnMazeGenerating;
				mazeSpawner.MazeGenerationStarted -= OnMazeGenerationFinished;
			}
		}
	}

	private void OnDestroy()
	{
		UnSubScribeEvent();
	}
	#endregion

	#region Functionality
	private void OnMazeGenerating(object sender, EventArgs eventArgs)
	{
		generationFinished = false;
		StartCoroutine(PlayTextAnimation());
	}

	private IEnumerator PlayTextAnimation()
	{	
		displayText.gameObject.SetActive(true);
		displayText.color = Color.white;
		int state = 0;
		while(generationFinished == false)
		{
			string displayText = "Generating";
			for(int i = 0; i < state; i++)
			{
				displayText += " .";
			}
			this.displayText.text = displayText;
			yield return new WaitForSeconds(0.5f);
			if(state == 3)
			{
				state =0;
			}
			else
			{
				state++;
			}		
		}
	}

	private void OnMazeGenerationFinished(object sender, EventArgs eventArgs)
	{
		generationFinished = true;
		displayText.color = Color.green;
		displayText.text = "Done!";
	}

	#endregion
}