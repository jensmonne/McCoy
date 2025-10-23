using System.Reflection;
using Discord;
using Microsoft.Extensions.Hosting;

namespace McCoy.Core;

public class BotHostedService : IHostedService
{
    private readonly BotService _botService;

    public BotHostedService(BotService botService)
    {
        _botService = botService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var token = DotNetEnv.Env.GetString("DISCORD_TOKEN");

        if (string.IsNullOrEmpty(token))
        {
            await Console.Error.WriteLineAsync("No discord token provided.");
        }

        _botService.ConfigureEventHandlers();
        await _botService.Client.LoginAsync(TokenType.Bot, token);
        await _botService.Client.StartAsync();
        await _botService.Interaction.AddModulesAsync(Assembly.GetEntryAssembly(), _botService.Services);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _botService.Client.StopAsync();
        await _botService.Client.LogoutAsync();
        await _botService.Client.DisposeAsync();
    }
}