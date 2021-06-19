using BlazorSignalRChat.Client.HttpManagers;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MudBlazor.Services;
using MudBlazor;

namespace BlazorSignalRChat.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");


            builder.Services.AddHttpClient("BlazorSignalRChat.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorSignalRChat.ServerAPI"));

            builder.Services.AddApiAuthorization();

            builder.Services.AddTransient<IChatManager, ChatManager>();
            //builder.Services.AddScoped<Blazorise.Snackbar.Snackbar>(); ... не хочет работать
            builder.Services.AddMudServices(c => 
            {
                c.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight; // ... короче будет(надеюсь) всплывающее сообщение справа внизу
            });
            //builder.Services.AddMudServices(config =>
            //{
            //    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;


            //    config.SnackbarConfiguration.PreventDuplicates = false;
            //    config.SnackbarConfiguration.NewestOnTop = false;
            //    config.SnackbarConfiguration.ShowCloseIcon = true;
            //    config.SnackbarConfiguration.VisibleStateDuration = 10000;
            //    config.SnackbarConfiguration.HideTransitionDuration = 500;
            //    config.SnackbarConfiguration.ShowTransitionDuration = 500;
            //    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            //});

            await builder.Build().RunAsync();
        }
    }
}
