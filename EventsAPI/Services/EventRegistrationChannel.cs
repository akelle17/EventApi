using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace EventsAPI.Services
{
    public class EventRegistrationChannel
    {
        private const int MaxMessagesInChannel = 100;
        private readonly ILogger<EventRegistrationChannel> _logger;
        private readonly Channel<EventRegistrationChannelRequest> _channel;

        public EventRegistrationChannel(ILogger<EventRegistrationChannel> logger)
        {
            _logger = logger;
            var options = new BoundedChannelOptions(MaxMessagesInChannel)
            {
                SingleReader = true,
                SingleWriter = false
            };

            _channel = Channel.CreateBounded<EventRegistrationChannelRequest>(options);
        }

        public async Task<bool> AddRegistration(EventRegistrationChannelRequest request, CancellationToken ct = default)
        {
            while(await _channel.Writer.WaitToWriteAsync(ct) && !ct.IsCancellationRequested)
            {
                if(_channel.Writer.TryWrite(request))
                {
                    return true;
                }
            }
            return false;
        }

        // dequeue the stuff.
        public IAsyncEnumerable<EventRegistrationChannelRequest> ReadAllAsync(CancellationToken ct = default) =>
            _channel.Reader.ReadAllAsync();
    }

    public record EventRegistrationChannelRequest(int RegistrationId);
}
