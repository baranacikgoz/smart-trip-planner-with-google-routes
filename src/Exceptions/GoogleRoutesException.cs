namespace SmartTripPlanner.WebAPI.Exceptions;

public class GoogleRoutesException : System.Exception
{
    public GoogleRoutesException(string message, string sentBody, string receivedBody) : base(message)
    {
        SentBody = sentBody;
        ReceivedBody = receivedBody;
    }

    public string SentBody { get; private set; }
    public string ReceivedBody { get; private set; }

    public static async Task ThrowIfNotSuccess(HttpResponseMessage response, string sentBody)
    {
        if (!response.IsSuccessStatusCode)
        {
            var receivedBody = await response.Content.ReadAsStringAsync();
            throw new GoogleRoutesException(
                $"Google Routes API request failed with status code {response.StatusCode}.",
                sentBody,
                receivedBody
            );
        }
    }
}
