using Discord.Commands;
using System.Threading.Tasks;
using System;

public class CommandContextTypeReader : TypeReader
{
    public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
    {
        if (context is CommandContext commandContext)
        {
            return Task.FromResult(TypeReaderResult.FromSuccess(commandContext));
        }
        else
        {
            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Invalid command context."));
        }
    }
}