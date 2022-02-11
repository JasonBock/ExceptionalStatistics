using ExceptionalStatistics.Core;

var statistics = new Statistics();
var fileCount = 0;

// "M:\\repos\\roslyn"
// "C:\\Users\\jason\\source\\repos\\ExceptionalStatistics\\src"
foreach (var file in Directory.EnumerateFiles("M:\\repos\\roslyn", "*.cs", SearchOption.AllDirectories))
{
	fileCount++;
	await Console.Out.WriteLineAsync($"Processing {file}...").ConfigureAwait(false);
	statistics += new StatisticsGatherer(await File.ReadAllTextAsync(file).ConfigureAwait(false));
}

await Console.Out.WriteLineAsync($"Total number of files: {fileCount}").ConfigureAwait(false);
await Console.Out.WriteLineAsync($"Number of statements: {statistics.StatementsCount}").ConfigureAwait(false);
await Console.Out.WriteLineAsync($"Number of expressions: {statistics.ExpressionsCount}").ConfigureAwait(false);
await Console.Out.WriteLineAsync($"Number of bad catch blocks: {statistics.BadCatchBlocksCount}").ConfigureAwait(false);

internal sealed class Statistics
{
	internal Statistics() { }

	public static Statistics operator +(Statistics left, StatisticsGatherer right) =>
		new(left.BadCatchBlocksCount + right.BadCatchBlocksCount,
			left.ExpressionsCount + right.ExpressionsCount,
			left.StatementsCount + right.StatementsCount);

	private Statistics(uint badCatchBlocksCount, uint expressionsCount, uint statementsCount) =>
		(this.BadCatchBlocksCount, this.ExpressionsCount, this.StatementsCount) =
			(badCatchBlocksCount, expressionsCount, statementsCount);

	internal uint BadCatchBlocksCount { get; }
	internal uint ExpressionsCount { get; }
	internal uint StatementsCount { get; }
}