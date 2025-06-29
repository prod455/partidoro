﻿using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Partidoro.Application.Cli
{
    public sealed class TypeRegistrar : ITypeRegistrar
    {
        private readonly IServiceCollection _builder;

        public TypeRegistrar(IServiceCollection builder)
        {
            _builder = builder;
        }

        public ITypeResolver Build()
        {
            return new TypeResolver(_builder.BuildServiceProvider());
        }

        public void Register(Type service, Type implementation)
        {
            _builder.AddSingleton(service, implementation);
        }

        public void RegisterInstance(Type service, object implementation)
        {
            _builder.AddSingleton(service, implementation);
        }

        public void RegisterLazy(Type service, Func<object> func)
        {
            _builder.AddSingleton(service, (provider) => func());
        }
    }

    public sealed class TypeResolver : ITypeResolver, IDisposable
    {
        private readonly IServiceProvider _provider;

        public TypeResolver(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public object? Resolve(Type? type)
        {
            if (type == null)
            {
                return null;
            }
            // Resolve a service from the DI container
            return _provider.GetService(type);
        }

        public void Dispose()
        {
            // If the provider can be disposed, do it
            if (_provider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
