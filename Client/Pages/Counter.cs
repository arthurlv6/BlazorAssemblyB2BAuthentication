﻿using System;
using System.Threading;
using System.Threading.Tasks;
using BlazorTest.Client.Services;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BlazorTest.Client.Pages
{
    public class Model
    {
        public string Token { get; set; }
    }

    public partial class Counter
    {
        [Inject]
        public GrpcChannel Channel { get; set; }

        public int currentCount = 0;
        public CancellationTokenSource cts;
        public Model Model = new Model();
        public string Error;

        [Inject]
        public GrpcBearerTokenProvider GrpcBearerTokenProvider { get; set; }

        public async Task IncrementCount()
        {
            cts = new CancellationTokenSource();

            try
            {
                Model.Token = await GrpcBearerTokenProvider.GetTokenAsync(Program.Scope);
            }
            catch (AccessTokenNotAvailableException a)
            {
                a.Redirect();
            }

            //var headers = new Metadata
            //{
            //    { "Authorization", $"Bearer {Model.Token}" }
            //};

            //var client = new Count.Counter.CounterClient(Channel);
            //var call = client.StartCounter(new CounterRequest { Start = currentCount }, headers, cancellationToken: cts.Token);

            //try
            //{
            //    Error = string.Empty;
            //    await foreach (var message in call.ResponseStream.ReadAllAsync())
            //    {
            //        currentCount = message.Count;
            //        StateHasChanged();
            //    }
            //}
            //catch (RpcException rpcException) when (rpcException.StatusCode == StatusCode.Cancelled)
            //{
            //    // Ignore exception from cancellation
            //    Error = rpcException.Message;
            //}
            //catch (RpcException rpcException) when (rpcException.StatusCode == StatusCode.Unauthenticated)
            //{
            //    Error = rpcException.Message;

            //}
            //catch (Exception exception)
            //{
            //    Error = exception.Message;
            //}
        }

        public void StopCount()
        {
            cts?.Cancel();
            cts = null;
        }
    }
}
