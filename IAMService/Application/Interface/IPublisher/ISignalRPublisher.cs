namespace Application.Interface.IPublisher
{
    public interface ISignalRPublisher
    {
        Task PublishEnvelop(SignalREnvelope.SignalREnvelope envelope);
    }
}
