using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RC_SpeechToText.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC_SpeechToText.Tests
{
    public class DbContext
    {
        public static DbContextOptions<SearchAVContext> CreateNewContextOptions()
        {
            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<SearchAVContext>();
            builder.UseInMemoryDatabase()
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }
    }
}
