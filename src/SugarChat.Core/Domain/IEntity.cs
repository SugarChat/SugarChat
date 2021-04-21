namespace SugarChat.Core.Domain
{
    public interface IEntity<T> 
    {
        T Id { get; }
    }
};