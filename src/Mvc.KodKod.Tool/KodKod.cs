using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;

namespace Mvc.KodKod.Tool
{
    class KodKod
    {
        public KodKodOptions Options { get; set; }

        public void Execute()
        {
            var assemblyName = new AssemblyName(Options.ApplicationName);
            var assembly = Assembly.Load(assemblyName);
            var serviceProvider = GetServiceProvider(assembly);

            var apiDescriptorProvider = serviceProvider.GetRequiredService<IApiDescriptionGroupCollectionProvider>();
            var apiDescriptions = apiDescriptorProvider.ApiDescriptionGroups.Items;

            var document = NSwaggerizer.GetSwaggerDocument(apiDescriptorProvider.ApiDescriptionGroups, Options);
            File.WriteAllText(Options.OutputPath, document.ToJson());
        }

        private IServiceProvider GetServiceProvider(Assembly applicationAssembly)
        {
            var services = new ServiceCollection();

            var hostingEnvironment = new HostingEnvironment
            {
                ApplicationName = Options.ApplicationName,
            };
            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore.Mvc.KodKod");

            services
                .AddSingleton<IHostingEnvironment>(hostingEnvironment)
                .AddSingleton<DiagnosticSource>(diagnosticSource)
                .AddLogging()
                .AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();

            var mvcBuilder = services.AddMvc();

            var configureType = applicationAssembly
                .GetExportedTypes()
                .FirstOrDefault(typeof(IDesignTimeMvcBuilderConfiguration).IsAssignableFrom);

            if (configureType != null)
            {
                var configureInstance = (IDesignTimeMvcBuilderConfiguration)Activator.CreateInstance(configureType);
                configureInstance.ConfigureMvc(mvcBuilder);
            }

            return mvcBuilder.Services.BuildServiceProvider();
        }
    }
}
