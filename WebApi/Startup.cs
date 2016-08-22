using System;
using System.Web.Http;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Owin;
using Owin.WebSocket.Extensions;
using WebApi.Controllers;
using WebApi.Services;

namespace WebApi
{
    public static class Startup
    {
        private static IServiceLocator GetServiceLocator()
        {
            var container = new UnityContainer();

            container.RegisterType<IChatClientService, ChatClientService>();

            container.RegisterType<ChatService.Interfaces.IChatService>(new InjectionFactory(c => {
                return ServiceProxy.Create<ChatService.Interfaces.IChatService>(
                    new Uri("fabric:/Chitchat/ChatService"),
                    new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(0));
            }));

            // Register self as IServiceLocator
            var locator = new UnityServiceLocator(container);
            container.RegisterInstance<IServiceLocator>(locator);

            return locator;
        }

        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public static void ConfigureApp(IAppBuilder appBuilder)
        {
            var serviceLocator = GetServiceLocator();
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            appBuilder.MapWebSocketRoute<ChatController>("/chat", serviceLocator);

            // Web APi
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            appBuilder.UseWebApi(config);

            // Static files
            var physicalFileSystem = new PhysicalFileSystem("wwwroot");
            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                FileSystem = physicalFileSystem
            };
            options.StaticFileOptions.FileSystem = physicalFileSystem;
            options.StaticFileOptions.ServeUnknownFileTypes = true;
            options.DefaultFilesOptions.DefaultFileNames = new[] { "index.html" };
            appBuilder.UseFileServer(options);
        }
    }
}
