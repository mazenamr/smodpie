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
        var trainingPathOption = new Option<DirectoryInfo>(
            new[] { "--path" },
            "JSON file containing training data")
        { IsRequired = true };

        var trainCommand = new Command("train", "Train the model")
        {
            trainingPathOption
        };

        var newCommitJsonOption = new Option<DirectoryInfo>(
            new[] { "--new-commit" },
            "JSON file containing new commit data ")
        { IsRequired = true };

        var dirOption = new Option<DirectoryInfo>(
            new[] { "--dir" },
            "Project directory")
        { IsRequired = true };

        var counterOption = new Option<DirectoryInfo>(
            new[] { "--counter" },
            "Counter JSON file")
        { IsRequired = true };

        var tokensOption = new Option<DirectoryInfo>(
            new[] { "--tokens" },
            "Tokens JSON file")
        { IsRequired = true };

        var nOption = new Option<int>(
            new[] { "-n" },
            "Number of lines to return")
        { IsRequired = true };

        var rankCommand = new Command("rank", "Rank the lines based on the model")
        {
            newCommitJsonOption,
            dirOption,
            counterOption,
            tokensOption,
            nOption
        };

        trainCommand.SetHandler(async (path) =>
        {
            await Train(path.FullName);
        }, trainingPathOption);

        rankCommand.SetHandler(async (newCommit, dir, counter, tokens, n) =>
        {
            await Rank(newCommit.FullName, dir.FullName, counter.FullName, tokens.FullName, n);
        }, newCommitJsonOption, dirOption, counterOption, tokensOption, nOption);

        var rootCommand = new RootCommand("Smodpie: a tool for modeling source code")
        {
            trainCommand,
            rankCommand
        };

        return rootCommand;
    }

    static async Task Train(string path)
    {
        await Task.Run(() =>
        {
            Console.WriteLine($"Clean Lines Json: {path}");
        });
    }

    static async Task Rank(string newCommit, string dir, string counter, string tokens, int n)
    {
        await Task.Run(() =>
        {
            Console.WriteLine($"New-Commit: {newCommit}");
            Console.WriteLine($"Directory: {dir}");
            Console.WriteLine($"Counter: {counter}");
            Console.WriteLine($"Tokens: {tokens}");
            Console.WriteLine($"N: {n}");
        });
    }
}
