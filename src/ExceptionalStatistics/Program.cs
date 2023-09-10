using ExceptionalStatistics.Core;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System.Collections.Immutable;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics;
using System.Numerics;

var solutionOption = new Option<FileInfo?>("--sln",
	description: "The solution file to parse.",
	parseArgument: result =>
	{
		if (result.Tokens.Count > 0)
		{
			var solutionFile = result.Tokens[0].Value;

			if (string.IsNullOrWhiteSpace(solutionFile))
			{
				result.ErrorMessage = "No solution file was provided.";
				return null;
			}
			else
			{
				var solutionFileInfo = new FileInfo(solutionFile);

				if (!solutionFileInfo.Exists)
				{
					result.ErrorMessage = $"The solution file {solutionFile} does not exist.";
					return null;
				}
				else
				{
					return solutionFileInfo;
				}
			}
		}
		else
		{
			result.ErrorMessage = "No solution file was provided.";
			return null;
		}
	});

var directoryOption = new Option<DirectoryInfo?>("--dir",
	description: "The directory to recursively search for .cs files.",
	parseArgument: result =>
	{
		if (result.Tokens.Count > 0)
		{
			var directory = result.Tokens[0].Value;

			if (string.IsNullOrWhiteSpace(directory))
			{
				result.ErrorMessage = "No directory was provided.";
				return null;
			}
			else
			{
				var directoryInfo = new DirectoryInfo(directory);

				if (!directoryInfo.Exists)
				{
					result.ErrorMessage = $"The directory {directory} does not exist.";
					return null;
				}
				else
				{
					return directoryInfo;
				}
			}
		}
		else
		{
			result.ErrorMessage = "No directory was provided.";
			return null;
		}
	});


var rootCommand = new RootCommand
{
	solutionOption,
	directoryOption
};

rootCommand.SetHandler(async (solutionOptionValue, directoryOptionValue) =>
{
	var stopwatch = Stopwatch.StartNew();

	var statistics = new Statistics();

	if (solutionOptionValue is not null)
	{
		MSBuildLocator.RegisterDefaults();

		// TODO: Would be nice if we did a async consumer/producer model here.
		using (var workspace = MSBuildWorkspace.Create())
		{
			var solution = await workspace.OpenSolutionAsync(solutionOptionValue.FullName).ConfigureAwait(false);

			foreach (var project in solution.Projects)
			{
				foreach (var document in project.Documents)
				{
					var documentPath = document.FilePath!;
					if (Path.GetExtension(documentPath) == ".cs")
					{
						var code = await File.ReadAllTextAsync(documentPath).ConfigureAwait(false);
						statistics += (new FileInfo(documentPath), new StatisticsGatherer(code));
					}
				}
			}
		}

		await PrintStatisticsAsync(statistics).ConfigureAwait(false);
	}
	else if (directoryOptionValue is not null)
	{
		var forLock = new object();

		await Parallel.ForEachAsync(Directory.EnumerateFiles(directoryOptionValue.FullName, "*.cs", SearchOption.AllDirectories),
			async (file, token) =>
			{
				var code = await File.ReadAllTextAsync(file, token).ConfigureAwait(false);

				lock (forLock)
				{
					statistics += (new FileInfo(file), new StatisticsGatherer(code));
				}
			}).ConfigureAwait(false);

		await PrintStatisticsAsync(statistics).ConfigureAwait(false);
	}
	else
	{
		Console.WriteLine("ERROR: A solution file or directory was not provided.");
	}

	stopwatch.Stop();

	await Console.Out.WriteLineAsync($"Total time: {stopwatch.Elapsed}").ConfigureAwait(false);
},
solutionOption, directoryOption);

await rootCommand.InvokeAsync(args).ConfigureAwait(false);

static async Task PrintStatisticsAsync(Statistics statistics)
{
	await Console.Out.WriteLineAsync($"Total number of files: {statistics.FileCount}").ConfigureAwait(false);
	await Console.Out.WriteLineAsync($"Number of statements: {statistics.StatementsCount}").ConfigureAwait(false);
	await Console.Out.WriteLineAsync($"Number of expressions: {statistics.ExpressionsCount}").ConfigureAwait(false);
	await Console.Out.WriteLineAsync($"Number of bad catch blocks: {statistics.BadCatchClauses.Count}").ConfigureAwait(false);
	await Console.Out.WriteLineAsync($"Number of empty catch blocks with filters: {statistics.EmptyCatchBlockWithFilterClauses.Count}").ConfigureAwait(false);

	await Console.Out.WriteLineAsync().ConfigureAwait(false);
	await Console.Out.WriteLineAsync("Bad Catch Clauses").ConfigureAwait(false);
	await Console.Out.WriteLineAsync().ConfigureAwait(false);

	foreach (var (file, catchClauses) in statistics.BadCatchClauses)
	{
		await Console.Out.WriteLineAsync($"\tFile: {file.FullName}").ConfigureAwait(false);

		foreach (var catchClause in catchClauses)
		{
			var declarationNode = catchClause.Ancestors(true).OfType<MethodDeclarationSyntax>()
				.FirstOrDefault();

			if (declarationNode is not null)
			{
				var typeDeclaration = declarationNode.Ancestors(true).OfType<TypeDeclarationSyntax>().FirstOrDefault();

				if (typeDeclaration is not null)
				{
					var namespaceDeclaration = typeDeclaration.Ancestors(true).OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();

					await Console.Out.WriteLineAsync(
						 $"\t\t{(namespaceDeclaration is not null ? namespaceDeclaration.Name + "." : "global::")}{typeDeclaration.Identifier.Text}.{declarationNode.Identifier.Text}()").ConfigureAwait(false);
					var catchClauseSpan = catchClause.GetLocation().GetMappedLineSpan();
					await Console.Out.WriteLineAsync($"\t\tStart line location: {catchClauseSpan.StartLinePosition.Line + 1}").ConfigureAwait(false);
					await Console.Out.WriteLineAsync($"\t\tEnd line location: {catchClauseSpan.StartLinePosition.Line + 1}").ConfigureAwait(false);
				}
			}
		}
	}

	await Console.Out.WriteLineAsync().ConfigureAwait(false);
	await Console.Out.WriteLineAsync("Empty Catch Block With Filter Clauses").ConfigureAwait(false);
	await Console.Out.WriteLineAsync().ConfigureAwait(false);

	foreach (var (file, catchClauses) in statistics.EmptyCatchBlockWithFilterClauses)
	{
		await Console.Out.WriteLineAsync($"\tFile: {file.FullName}").ConfigureAwait(false);

		foreach (var catchClause in catchClauses)
		{
			var declarationNode = catchClause.Ancestors(true).OfType<MethodDeclarationSyntax>()
				.FirstOrDefault();

			if (declarationNode is not null)
			{
				var typeDeclaration = declarationNode.Ancestors(true).OfType<TypeDeclarationSyntax>().FirstOrDefault();

				if (typeDeclaration is not null)
				{
					var namespaceDeclaration = typeDeclaration.Ancestors(true).OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();

					await Console.Out.WriteLineAsync(
						 $"\t\t{(namespaceDeclaration is not null ? namespaceDeclaration.Name + "." : "global::")}{typeDeclaration.Identifier.Text}.{declarationNode.Identifier.Text}()").ConfigureAwait(false);
					var catchClauseSpan = catchClause.GetLocation().GetMappedLineSpan();
					await Console.Out.WriteLineAsync($"\t\tStart line location: {catchClauseSpan.StartLinePosition.Line + 1}").ConfigureAwait(false);
					await Console.Out.WriteLineAsync($"\t\tEnd line location: {catchClauseSpan.StartLinePosition.Line + 1}").ConfigureAwait(false);
				}
			}
		}
	}
}

internal sealed class Statistics
	: IAdditionOperators<Statistics, (FileInfo file, StatisticsGatherer gatherer), Statistics>
{
	internal Statistics() { }

	public static Statistics operator +(Statistics left, (FileInfo file, StatisticsGatherer gatherer) right) =>
		new(left.ExpressionsCount + right.gatherer.ExpressionsCount,
			left.StatementsCount + right.gatherer.StatementsCount,
			left.FileCount + 1,
			right.gatherer.BadCatchClauses.Length > 0 ?
				left.BadCatchClauses.Add(right.file!, right.gatherer.BadCatchClauses) :
				left.BadCatchClauses,
			right.gatherer.EmptyCatchBlockWithFilterClauses.Length > 0 ?
				left.EmptyCatchBlockWithFilterClauses.Add(right.file, right.gatherer.EmptyCatchBlockWithFilterClauses) :
			left.EmptyCatchBlockWithFilterClauses);

	private Statistics(uint expressionsCount, uint statementsCount, uint fileCount,
		ImmutableDictionary<FileInfo, ImmutableArray<CatchClauseSyntax>> badCatchClauses,
		ImmutableDictionary<FileInfo, ImmutableArray<CatchClauseSyntax>> emptyCatchBlockWithFilterClauses) =>
		(this.ExpressionsCount, this.StatementsCount, this.FileCount, this.BadCatchClauses, this.EmptyCatchBlockWithFilterClauses) =
			(expressionsCount, statementsCount, fileCount, badCatchClauses, emptyCatchBlockWithFilterClauses);

	internal ImmutableDictionary<FileInfo, ImmutableArray<CatchClauseSyntax>> BadCatchClauses { get; } = ImmutableDictionary<FileInfo, ImmutableArray<CatchClauseSyntax>>.Empty;
	internal ImmutableDictionary<FileInfo, ImmutableArray<CatchClauseSyntax>> EmptyCatchBlockWithFilterClauses { get; } = ImmutableDictionary<FileInfo, ImmutableArray<CatchClauseSyntax>>.Empty;
	internal uint ExpressionsCount { get; }
	internal uint FileCount { get; }
	internal uint StatementsCount { get; }
}