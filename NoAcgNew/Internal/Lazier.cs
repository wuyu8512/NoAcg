using Microsoft.Extensions.DependencyInjection;
using System;

namespace NoAcgNew
{
    internal class Lazier<T> : Lazy<T> where T : class
    {
        public Lazier(IServiceProvider provider) : base(() => ActivatorUtilities.GetServiceOrCreateInstance<T>(provider))
        {
        }
    }
}
