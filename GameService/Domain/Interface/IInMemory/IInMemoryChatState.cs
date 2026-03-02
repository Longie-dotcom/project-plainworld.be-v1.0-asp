using Domain.Aggregate;

namespace Domain.Interface.IInMemory
{
    public interface IInMemoryChatState
    {
        void Add(
            Chat chat);

        void Clear();
    }
}
