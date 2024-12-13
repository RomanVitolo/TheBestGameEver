namespace Core.Scripts.Runtime.Utilities
{
    public delegate void NotifyAction();
    
    internal interface INotifyEvent
    {
        public NotifyAction NotifyItemAction { get; set; }
        public string ItemId { get; } 
    }
}