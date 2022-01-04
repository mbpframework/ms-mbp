using Grpc.Core;
using Grpc.Core.Interceptors;
using Mbp.Core.User;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Mbp.Net.gRPC
{
    /// <summary>
    /// gRPC客户端拦截器
    /// </summary>
    public class GrpcClientInterceptor : Interceptor
    {
        private readonly IServiceCollection _services;

        public GrpcClientInterceptor(IServiceCollection services)
        {
            _services = services;
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            AddCallerMetadata(ref context);

            return base.AsyncUnaryCall(request, context, continuation);
        }

        private void AddCallerMetadata<TRequest, TResponse>(ref ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
        {
            var headers = context.Options.Headers;

            // Call doesn't have a headers collection to add to.
            // Need to create a new context with headers for the call.
            if (headers == null)
            {
                headers = new Metadata();
                var options = context.Options.WithHeaders(headers);
                context = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, options);
            }

            var currentUser = _services.BuildServiceProvider().GetService<ICurrentUser>();

            if (!string.IsNullOrEmpty(currentUser.AccessToken))
            {
                headers.Add("Authorization", $"Bearer {currentUser.AccessToken}");
            }

            headers.Add("caller-user", Environment.UserName);
            headers.Add("caller-machine", Environment.MachineName);
            headers.Add("caller-os", Environment.OSVersion.ToString());
        }
    }
}
