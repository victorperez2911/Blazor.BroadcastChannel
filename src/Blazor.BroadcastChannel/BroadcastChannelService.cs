﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazor.BroadcastChannel
{
    internal class BroadcastChannelService : IBroadcastChannelService
    {
        private readonly Lazy<ValueTask<IJSObjectReference>> _moduleTask;

        public BroadcastChannelService(IJSRuntime jsRuntime, NavigationManager NavManager)
        {

            string path = new Uri(NavManager.BaseUri).AbsolutePath + "_content/Blazor.BroadcastChannel/Blazor.BroadcastChannel.js";

            //_moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", "/_content/Blazor.BroadcastChannel/Blazor.BroadcastChannel.js"));
            _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", path));
        }

        public async Task<IBroadcastChannel> CreateOrJoinAsync(string channelName)
        {
            IJSObjectReference module = await _moduleTask.Value;

            BroadcastChannel channel = await module.InvokeAsync<BroadcastChannel>("createOrJoin", Guid.NewGuid().ToString(), channelName);
            await channel.InitializeAsync(this);

            return channel;
        }

        public async Task CloseAsync(string connectionId)
        {
            IJSObjectReference module = await _moduleTask.Value;

            await module.InvokeVoidAsync("close", connectionId);
        }

        public async Task PostMessageAsync<TValue>(string connectionId, TValue data)
        {
            IJSObjectReference module = await _moduleTask.Value;

            await module.InvokeVoidAsync("postMessage", connectionId, data);
        }

        public async Task AddMessageEventListener(BroadcastChannel channel)
        {
            IJSObjectReference module = await _moduleTask.Value;

            await module.InvokeVoidAsync("addMessageEventListener", channel.Id, DotNetObjectReference.Create(channel));
        }
    }
}
