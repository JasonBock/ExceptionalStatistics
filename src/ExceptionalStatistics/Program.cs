using ExceptionalStatistics.Core;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Diagnostics;

var stopwatch = Stopwatch.StartNew();

var statistics = new Statistics();
var fileCount = 0;

// "M:\\repos\\roslyn"
// "C:\\Users\\jason\\source\\repos\\ExceptionalStatistics\\src"

var forLock = new object();

await Parallel.ForEachAsync(Directory.EnumerateFiles("M:\\repos\\roslyn", "*.cs", SearchOption.AllDirectories),
	async (file, token) =>
	{
		Interlocked.Increment(ref fileCount);
		var code = await File.ReadAllTextAsync(file, token).ConfigureAwait(false);
		lock (forLock)
		{
			statistics += new StatisticsGatherer(new FileInfo(file), code);
		}
	}).ConfigureAwait(false);

//foreach (var file in Directory.EnumerateFiles("C:\\Users\\jason\\source\\repos\\ExceptionalStatistics\\src", "*.cs", SearchOption.AllDirectories))
//{
//	fileCount++;
//	//await Console.Out.WriteLineAsync($"Processing {file}...").ConfigureAwait(false);
//	statistics += new StatisticsGatherer(await File.ReadAllTextAsync(file).ConfigureAwait(false));
//}

await Console.Out.WriteLineAsync($"Total number of files: {fileCount}").ConfigureAwait(false);
await Console.Out.WriteLineAsync($"Number of statements: {statistics.StatementsCount}").ConfigureAwait(false);
await Console.Out.WriteLineAsync($"Number of expressions: {statistics.ExpressionsCount}").ConfigureAwait(false);
await Console.Out.WriteLineAsync($"Number of bad catch blocks: {statistics.BadCatchBlocksCount}").ConfigureAwait(false);

foreach (var (file, catchClauses) in statistics.BadCatchClauses)
{
	await Console.Out.WriteLineAsync($"File: {file.FullName}").ConfigureAwait(false);

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
					 $"\t{(namespaceDeclaration is not null ? namespaceDeclaration.Name + "." : "global::")}{typeDeclaration.Identifier.Text}.{declarationNode.Identifier.Text}()").ConfigureAwait(false);
				var catchClauseSpan = catchClause.GetLocation().GetMappedLineSpan();
				await Console.Out.WriteLineAsync($"\tStart line location: {catchClauseSpan.StartLinePosition.Line + 1}").ConfigureAwait(false);
				await Console.Out.WriteLineAsync($"\tEnd line location: {catchClauseSpan.StartLinePosition.Line + 1}").ConfigureAwait(false);
			}
		}
	}
}

stopwatch.Stop();

await Console.Out.WriteLineAsync($"Total time: {stopwatch.Elapsed}").ConfigureAwait(false);

internal sealed class Statistics
{
	internal Statistics() { }

	public static Statistics operator +(Statistics left, StatisticsGatherer right) =>
		new(left.BadCatchBlocksCount + right.BadCatchBlocksCount,
			left.ExpressionsCount + right.ExpressionsCount,
			left.StatementsCount + right.StatementsCount,
			right.BadCatchClauses.Length > 0 ? left.BadCatchClauses.Add(right.File!, right.BadCatchClauses) : left.BadCatchClauses);

	private Statistics(uint badCatchBlocksCount, uint expressionsCount, uint statementsCount,
		ImmutableDictionary<FileInfo, ImmutableArray<CatchClauseSyntax>> badCatchClauses) =>
		(this.BadCatchBlocksCount, this.ExpressionsCount, this.StatementsCount, this.BadCatchClauses) =
			(badCatchBlocksCount, expressionsCount, statementsCount, badCatchClauses);

	internal ImmutableDictionary<FileInfo, ImmutableArray<CatchClauseSyntax>> BadCatchClauses { get; } = ImmutableDictionary<FileInfo, ImmutableArray<CatchClauseSyntax>>.Empty;
	internal uint BadCatchBlocksCount { get; }
	internal uint ExpressionsCount { get; }
	internal uint StatementsCount { get; }
}