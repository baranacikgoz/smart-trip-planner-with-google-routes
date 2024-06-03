namespace SmartTripPlanner.GoogleRoutes.Exceptions;

public class GoogleRoutesException : Exception
{
    public GoogleRoutesException(string message, string sentBody, string receivedBody) : base(message)
    {
        SentBody = sentBody;
        ReceivedBody = receivedBody;
    }

    public string SentBody { get; private set; }
    public string ReceivedBody { get; private set; }
}
