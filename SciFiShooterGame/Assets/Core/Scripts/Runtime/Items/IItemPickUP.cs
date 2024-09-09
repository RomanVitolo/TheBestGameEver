namespace Core.Scripts.Runtime.Items
{
    public interface IItemPickUP<in T>
    {
        public void PickUpObject(T itemId);    
    }
}