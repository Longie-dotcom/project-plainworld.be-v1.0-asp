using Domain.Aggregate;
using Domain.Interface.IInMemory;
using System.Collections.Concurrent;

namespace Infrastructure.Temporary
{
    public class InMemoryChatState : IInMemoryChatState
    {
        #region Attributes
        private readonly ConcurrentBag<Chat> chats = new();
        #endregion

        #region Properties
        #endregion

        public InMemoryChatState() { }

        #region Methods
        public void Add(Chat chat)
        {
            if (chat == null)
                return;

            chats.Add(chat);
        }

        public void Clear()
        {
            while (!chats.IsEmpty)
                chats.TryTake(out _);
        }
        #endregion
    }
}
