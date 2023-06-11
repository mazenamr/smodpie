using System.CommandLine;

namespace Smodpie;

internal class Program
{
    static Task<int> Main(string[] args)
    {
        var rootCommand = CreateRootCommand();

        // Parse the incoming args and invoke the handler
        return rootCommand.InvokeAsync(args);
    }

    private static RootCommand CreateRootCommand()
    {
        var trainOption = new Option<string>(
            new[] { "-tr", "--train" },
            "The path of the training files"
        )
        { IsRequired = true };

        var testOption = new Option<string>(
            new[] { "-te", "--test" },
            "The path of the test files"
        );

        var parserOption = new Option<string>(
            new[] { "-p", "--parser" },
            () => "char",
            "The parser to use [char, word]"
        );

        var rootCommand = new RootCommand
            {
                trainOption,
                testOption,
                parserOption
            };

        rootCommand.Description = "Smodpie: a tool for modeling source code";

        rootCommand.SetHandler(async (train, test, parser) =>
        {
            await Train(train, parser);
            if (test != null)
                await Test(test, parser);
        }, trainOption, testOption, parserOption);

        return rootCommand;
    }

    static async Task Train(string trainPath, string parser)
    {
        await Task.Run(() =>
        {
            Console.WriteLine($"Training files: {trainPath}");
            Console.WriteLine($"Parser: {parser}");
        });
    }

    static async Task Test(string testPath, string parser)
    {
        await Task.Run(() =>
        {
            Console.WriteLine($"Test files: {testPath}");
            Console.WriteLine($"Parser: {parser}");
        });
    }
}
