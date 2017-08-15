namespace AccidentalFish.Foundations.Resources.Azure.EventHubs
{
    /// <summary>
    /// Interface for an event hub seriailzer. The default serializer uses Json.
    /// </summary>
    public interface IEventHubSerializer
    {
        /// <summary>
        /// Return a serialized verison of an item
        /// </summary>
        /// <param name="item">The  item to serialize</param>
        /// <returns>Serialized item</returns>
        string Serialize<T>(T item);
    }
}
