﻿using log4net;
using Microsoft.Owin.Hosting;
using Ninject;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.OwinHost;
using Owin;
using Swashbuckle.Application;
using System;
using System.Net.Http.Formatting;
using System.Web.Http;
using Zapp.Config;

namespace Zapp.Rest
{
    /// <summary>
    /// Represents a implementation of <see cref="IRestService"/> for the Owin NuGet package.
    /// </summary>
    public class OwinRestService : IDisposable, IRestService
    {
        private readonly IKernel kernel;

        private readonly ILog logService;
        private readonly IConfigStore configStore;

        private IDisposable owinInstance;

        /// <summary>
        /// Initializes a new <see cref="OwinRestService"/>.
        /// </summary>
        /// <param name="kernel">Ninject kernel instance.</param>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="configStore">Configuration storage instance.</param>
        public OwinRestService(
            IKernel kernel,
            ILog logService,
            IConfigStore configStore)
        {
            this.kernel = kernel;

            this.logService = logService;
            this.configStore = configStore;
        }

        /// <summary>
        /// Starts the current instance of <see cref="OwinRestService"/>.
        /// </summary>
        public void Listen()
        {
            var opts = new StartOptions
            {
                Port = configStore.Value.Rest.Port
            };

            owinInstance = WebApp.Start(opts, Startup);

            logService.Info($"listening on port: {opts.Port}");
        }

        private void Startup(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());

            config
                .EnableSwagger(c => c.SingleApiVersion("v1", "Zapp"))
                .EnableSwaggerUi();

            app.UseNinjectMiddleware(() => kernel).UseNinjectWebApi(config);
        }

        /// <summary>
        /// Releases all resourced used by the <see cref="OwinRestService"/> instance.
        /// </summary>
        public void Dispose()
        {
            owinInstance?.Dispose();
            owinInstance = null;
        }
    }
}
