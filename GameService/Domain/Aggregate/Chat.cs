using Domain.DomainException;
using Domain.Enum;

namespace Domain.Aggregate
{
    public class Chat : AggregateBase
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid UserID { get; private set; }
        public string UserName { get; private set; }
        public string Content { get; private set; }
        public ChatType ChatType { get; private set; }
        public DateTime SentAt { get; private set; }
        #endregion

        protected Chat() : base(Guid.Empty) { }

        public Chat(
            Guid id,
            Guid userId,
            string userName,
            ChatType chatType,
            string content) : base(id)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ChatAggregateException(
                    "Message cannot be empty");

            UserID = userId;
            UserName = userName;
            ChatType = chatType;
            Content = content;
            SentAt = DateTime.UtcNow;
        }

        #region Methods
        #endregion

        #region Private Helpers
        #endregion
    }
}
