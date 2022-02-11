using ExceptionalStatistics.Core;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

var statistics = new Statistics();
var fileCount = 0;

// "M:\\repos\\roslyn"
// "C:\\Users\\jason\\source\\repos\\ExceptionalStatistics\\src"
foreach (var file in Directory.EnumerateFiles("M:\\repos\\roslyn", "*.cs", SearchOption.AllDirectories))
{
	fileCount++;
	//await Console.Out.WriteLineAsync($"Processing {file}...").ConfigureAwait(false);
	statistics += new StatisticsGatherer(await File.ReadAllTextAsync(file).ConfigureAwait(false));
}

await Console.Out.WriteLineAsync($"Total number of files: {fileCount}").ConfigureAwait(false);
await Console.Out.WriteLineAsync($"Number of statements: {statistics.StatementsCount}").ConfigureAwait(false);
await Console.Out.WriteLineAsync($"Number of expressions: {statistics.ExpressionsCount}").ConfigureAwait(false);
await Console.Out.WriteLineAsync($"Number of bad catch blocks: {statistics.BadCatchBlocksCount}").ConfigureAwait(false);

foreach (var catchClause in statistics.BadCatchClauses)
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
				 $"{(namespaceDeclaration is not null ? namespaceDeclaration.Name + "." : "global::")}{typeDeclaration.Identifier.Text}.{declarationNode.Identifier.Text}()").ConfigureAwait(false);
			var catchClauseSpan = catchClause.GetLocation().GetMappedLineSpan();
			await Console.Out.WriteLineAsync($"Start line location: {catchClauseSpan.StartLinePosition.Line + 1}").ConfigureAwait(false);
			await Console.Out.WriteLineAsync($"End line location: {catchClauseSpan.StartLinePosition.Line + 1}").ConfigureAwait(false);
		}
	}
}

internal static class BadCatchClause
{
	internal static void Foo()
	{
		try
		{

		}
#pragma warning disable CA1031 // Do not catch general exception types
		catch { }
#pragma warning restore CA1031 // Do not catch general exception types
	}
}

internal sealed class Statistics
{
	internal Statistics() { }

	public static Statistics operator +(Statistics left, StatisticsGatherer right) =>
		new(left.BadCatchBlocksCount + right.BadCatchBlocksCount,
			left.ExpressionsCount + right.ExpressionsCount,
			left.StatementsCount + right.StatementsCount,
			left.BadCatchClauses.Concat(right.BadCatchClauses).ToImmutableArray());

	private Statistics(uint badCatchBlocksCount, uint expressionsCount, uint statementsCount, ImmutableArray<CatchClauseSyntax> badCatchClauses) =>
		(this.BadCatchBlocksCount, this.ExpressionsCount, this.StatementsCount, this.BadCatchClauses) =
			(badCatchBlocksCount, expressionsCount, statementsCount, badCatchClauses);

	internal ImmutableArray<CatchClauseSyntax> BadCatchClauses { get; } = ImmutableArray<CatchClauseSyntax>.Empty;
	internal uint BadCatchBlocksCount { get; }
	internal uint ExpressionsCount { get; }
	internal uint StatementsCount { get; }
}