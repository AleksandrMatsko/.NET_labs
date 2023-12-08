using SharedTransitLibrary;

namespace Colosseum.RabbitMQ;

public class CardIndexToldHandler : ICardIndexToldHandler
{
    private readonly IDictionary<Guid, IList<CardIndexTold>> _messages = 
        new Dictionary<Guid, IList<CardIndexTold>>();
    private readonly IDictionary<Guid, TaskCompletionSource<IList<CardIndexTold>>> _waitedMessages = new 
        Dictionary<Guid, TaskCompletionSource<IList<CardIndexTold>>>();

    private readonly object _lock = new();
    
    public void Handle(CardIndexTold message)
    {
        lock (_lock)
        {
            var ok = _messages.TryGetValue(message.ExperimentId, out var msgList);
            if (!ok)
            {
                _messages[message.ExperimentId] = new List<CardIndexTold> { message };
                return;
            }
            msgList!.Add(message);
            ok = _waitedMessages.TryGetValue(message.ExperimentId, out var t);
            if (ok)
            {
                t!.SetResult(msgList);
                _waitedMessages.Remove(message.ExperimentId);
                _messages.Remove(message.ExperimentId);
            }
            else
            {
                _messages[message.ExperimentId] = msgList;
            }
        }
    }

    public async Task<IList<CardIndexTold>> GetMessage(Guid experimentId)
    {
        Task<IList<CardIndexTold>> t;
        lock (_lock)
        {
            var ok = _messages.TryGetValue(experimentId, out var msgList);
            if (ok && msgList?.Count == 2)
            {
                _messages.Remove(experimentId);
                t = Task.FromResult(msgList!);
            }
            else
            {
                var tcs = new TaskCompletionSource<IList<CardIndexTold>>();
                _waitedMessages.Add(experimentId, tcs);
                t = tcs.Task;    
            }
        }

        return await t;
    }
}
