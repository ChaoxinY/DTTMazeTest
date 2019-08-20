using System;
using System.Collections.Generic;

public class PublisherSubscribedEventArgs : EventArgs
{
	public IEventPublisher Publisher;
	public PublisherSubscribedEventArgs(IEventPublisher publisher)
	{
		Publisher = publisher;
	}
}

public class EventSubject
{
	//IEventhandlers should subscribe to this event first during the awake phase. If a IEventPublisher subscribes to this subject during the start phase, 
	//The IEventhandlers will be notified via this event. 
	public event EventHandler<PublisherSubscribedEventArgs> PublisherSubscribed;

	public List<IEventPublisher> EventPublishers { get; private set; } = new List<IEventPublisher>();

	public void Subscribe(IEventPublisher item)
	{
		EventPublishers.Add(item);
		PublisherSubscribed(this, new PublisherSubscribedEventArgs(item));
	}

	public void UnSubscribe(IEventPublisher item)
	{
		EventPublishers.Remove(item);
	}
}
