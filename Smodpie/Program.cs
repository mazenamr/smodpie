using Smodpie.Config;
using Smodpie.Counter;
using Smodpie.Model;
using Smodpie.Model.NGram;
using Smodpie.Parser;
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
            Dictionary<string, List<string>> trainingFiles = Utils.SZZ.GetTrainingLines(path);

            IParser parser = new WordParser();
            var tokenizer = new Tokenizer();

            var model = new NGramModel();

            int i = 1;
            foreach (var (file, lines) in trainingFiles)
            {
                Console.WriteLine($"Training {i++}: {file}");
                var parsed = parser.ParseLines(lines.ToArray());

                if (parsed == null)
                    continue;

                foreach (var line in parsed)
                {
                    tokenizer.Store(line);
                    var tokens = tokenizer.GetIndices(line).ToArray();
                    model.LearnTokens(tokens);
                }
            }

            TrieCounter counter = (model.Counter as TrieCounter)!;
            Console.WriteLine($"Learned {counter._root.C} tokens");
            Console.WriteLine("Saving model...");

            Utils.IO.SaveCounterToFile(counter, Constant.COUNTER_PATH);
            Utils.IO.SaveTokenizerToFile(tokenizer, Constant.TOKENS_PATH);
        });
    }

    static async Task Rank(string newCommit, string dir, string counterPath, string tokensPath, int n)
    {
        double invlog2 = -1.0 / Math.Log(2);

        await Task.Run(() =>
        {
            IParser parser = new WordParser();

            var counter = Utils.IO.LoadCounterFromFile(counterPath);
            var tokenizer = Utils.IO.LoadTokenizerFromFile(tokensPath);

            IModel model = new NGramModel(Constant.DEFAULT_NGRAM_ORDER, counter);

            var fileNames = Utils.SZZ.GetFileNames(newCommit);

            var fileEntropies = new List<double>[fileNames.Count];

            for (int i = 0; i < fileNames.Count; i++)
            {
                fileEntropies[i] = new List<double>();

                var filePath = Path.Join(dir, fileNames[i]);
                var parsed = parser.ParseFile(filePath);
                if (parsed == null)
                    continue;

                for (int j = 0; j < parsed.Length; j++)
                {
                    var tokens = tokenizer.GetIndices(parsed[j]).ToArray();
                    if (tokens.Length != 0)
                    {
                        var modelled = model.ModelTokens(tokens);
                        var probabilities = modelled.
                            Select(x => x.Probability * x.Confidence + (1 - x.Confidence) / tokenizer._tokens.Count).ToArray();
                        var entropies = probabilities.
                            Select(x => x * Math.Log(x) * invlog2).ToArray();
                        var lineEntropy = entropies.Average() + entropies.Max();
                        fileEntropies[i].Add(lineEntropy);
                    }
                    else
                        fileEntropies[i].Add(0);
                }
            }

            // get the top n lines ranked by entropy
            var topLines = new List<(string, double)>();
            for (int i = 0; i < fileNames.Count; i++)
                for (int j = 0; j < fileEntropies[i].Count; j++)
                    topLines.Add(($"{fileNames[i]}:{j + 1}", fileEntropies[i][j]));

            topLines.Sort((x, y) => y.Item2.CompareTo(x.Item2));

            var limit = Math.Min(n, topLines.Count);
            for (int i = 0; i < limit; i++)
                Console.WriteLine(topLines[i].Item1);
        });
    }
}
