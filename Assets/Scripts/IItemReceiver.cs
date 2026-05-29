public interface IItemReceiver
{
    // Returns true if the item was successfully accepted
    bool TryReceiveItem(ResourceType item);
}