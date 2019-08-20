public interface IEventHandler
{
	void SubscribeEvent(object eventPublisher, PublisherSubscribedEventArgs publisherSubscribedEventArgs);
	void UnSubScribeEvent();
}