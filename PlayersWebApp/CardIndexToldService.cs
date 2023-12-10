using SharedTransitLibrary;

namespace PlayersWebApp;

public class CardIndexToldService
{
    private readonly IDictionary<Guid, CardIndexTold> _messages = new Dictionary<Guid, CardIndexTold>();
    private readonly IDictionary<Guid, TaskCompletionSource<CardIndexTold>> _waitedMessages = new 
        Dictionary<Guid, TaskCompletionSource<CardIndexTold>>();

    public void AddMessage(CardIndexTold msg)
    {
        _messages.Add(msg.ExperimentId, msg);
        var ok = _waitedMessages.TryGetValue(msg.ExperimentId, out var t);
        if (ok)
        {
            t!.SetResult(msg);
            _waitedMessages.Remove(msg.ExperimentId);
        }
    }

    public Task<CardIndexTold> AwaitMessageAsync(Guid experimentId)
    {
        var ok = _messages.TryGetValue(experimentId, out var msg);
        if (ok)
        {
            _messages.Remove(experimentId);
            return Task.FromResult(msg!);
        }

        var tcs = new TaskCompletionSource<CardIndexTold>();
        _waitedMessages.Add(experimentId, tcs);
        return tcs.Task;
    }
}
