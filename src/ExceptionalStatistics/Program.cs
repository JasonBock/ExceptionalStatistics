using ExceptionalStatistics.Core;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Numerics;

var stopwatch = Stopwatch.StartNew();

var statistics = new Statistics();
var fileCount = 0;

// "M:\\repos\\roslyn"
// "C:\\Users\\jason\\source\\repos\\ExceptionalStatistics\\src"

var forLock = new object();

await Parallel.ForEachAsync(Directory.EnumerateFiles("M:\\repos\\roslyn2", "*.cs", SearchOption.AllDirectories),
	async (file, token) =>
	{
		Interlocked.Increment(ref fileCount);
		var code = await File.ReadAllTextAsync(file, token).ConfigureAwait(false);
		lock (forLock)
		{
			statistics += (new FileInfo(file), new StatisticsGatherer(code));
		}
	}).ConfigureAwait(false);

await Console.Out.WriteLineAsync($"Total number of files: {fileCount}").ConfigureAwait(false);
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

stopwatch.Stop();

await Console.Out.WriteLineAsync($"Total time: {stopwatch.Elapsed}").ConfigureAwait(false);

internal sealed class Statistics
	: IAdditionOperators<Statistics, (FileInfo file, StatisticsGatherer gatherer), Statistics>
{
	internal Statistics() { }

	public static Statistics operator +(Statistics left, (FileInfo file, StatisticsGatherer gatherer) right) =>
		new(left.ExpressionsCount + right.gatherer.ExpressionsCount,
			left.StatementsCount + right.gatherer.StatementsCount,
			right.gatherer.BadCatchClauses.Length > 0 ? 
				left.BadCatchClauses.Add(right.file!, right.gatherer.BadCatchClauses) : 
				left.BadCatchClauses,
			right.gatherer.EmptyCatchBlockWithFilterClauses.Length > 0 ? 
				left.EmptyCatchBlockWithFilterClauses.Add(right.file, right.gatherer.EmptyCatchBlockWithFilterClauses) : 
			left.EmptyCatchBlockWithFilterClauses);

	private Statistics(uint expressionsCount, uint statementsCount,
		ImmutableDictionary<FileInfo, ImmutableArray<CatchClauseSyntax>> badCatchClauses,
		ImmutableDictionary<FileInfo, ImmutableArray<CatchClauseSyntax>> emptyCatchBlockWithFilterClauses) =>
		(this.ExpressionsCount, this.StatementsCount, this.BadCatchClauses, this.EmptyCatchBlockWithFilterClauses) =
			(expressionsCount, statementsCount, badCatchClauses, emptyCatchBlockWithFilterClauses);

	internal ImmutableDictionary<FileInfo, ImmutableArray<CatchClauseSyntax>> BadCatchClauses { get; } = ImmutableDictionary<FileInfo, ImmutableArray<CatchClauseSyntax>>.Empty;
	internal ImmutableDictionary<FileInfo, ImmutableArray<CatchClauseSyntax>> EmptyCatchBlockWithFilterClauses { get; } = ImmutableDictionary<FileInfo, ImmutableArray<CatchClauseSyntax>>.Empty;
	internal uint ExpressionsCount { get; }
	internal uint StatementsCount { get; }
}