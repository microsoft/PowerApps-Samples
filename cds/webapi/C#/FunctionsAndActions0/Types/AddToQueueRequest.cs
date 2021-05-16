namespace PowerApps.Samples
{
    public class AddToQueueRequest<T> where T : IEntity
    {
        public T Target { get; set; }
        public Queue SourceQueue { get; set; }
        public QueueItem QueueItemProperties { get; set; }
    }
}
