using NUnit.Framework;

namespace ExceptionalStatistics.Core.Tests;

public static class StatisticsGathererTests
{
	// TODO: Add tests for passing in a file

	[Test]
	public static void CheckStatementsCount()
	{
		var code =
@"public static class Test
{
	public static int DoSomething()
	{
		var result = 0;

		for(var i = 0; i < 10; i++)
		{
			result += i;
		}

		return result;
	}
}";
		var gatherer = new StatisticsGatherer(code);
		Assert.That(gatherer.StatementsCount, Is.GreaterThan(0));
	}

	[Test]
	public static void CheckExpressionsCount()
	{
		var code =
@"public static class Test
{
	public static int DoSomething()
	{
		var x = () => 3;
		return x();
	}
}";
		var gatherer = new StatisticsGatherer(code);
		Assert.That(gatherer.ExpressionsCount, Is.GreaterThan(0));
	}

	[Test]
	public static void CheckBadCatchBlockCountWhenBlockIsEmptyAndNoCatchDeclarationIsMade()
	{
		var code =
@"public static class Test
{
	public static void DoSomething()
	{
		try
		{
		}
		catch { }
	}
}";
		var gatherer = new StatisticsGatherer(code);
		Assert.That(gatherer.BadCatchBlocksCount, Is.EqualTo(1));
	}

	[Test]
	public static void CheckBadCatchBlockCountWhenBlockIsNotEmptyAndNoCatchDeclarationIsMade()
	{
		var code =
@"public static class Test
{
	public static void DoSomething()
	{
		try
		{
		}
		catch 
		{ 
			Console.WriteLine(""Hi."");
		}
	}
}";
		var gatherer = new StatisticsGatherer(code);
		Assert.That(gatherer.BadCatchBlocksCount, Is.EqualTo(0));
	}

	[Test]
	public static void CheckBadCatchBlockCountWhenBlockIsEmptyAndCatchDeclarationIsMadeForArgumentException()
	{
		var code =
@"using System;

public static class Test
{
	public static void DoSomething()
	{
		try
		{
		}
		catch (ArgumentException) { }
	}
}";
		var gatherer = new StatisticsGatherer(code);
		Assert.That(gatherer.BadCatchBlocksCount, Is.EqualTo(0));
	}

	[Test]
	public static void CheckBadCatchBlockCountWhenBlockIsEmptyAndCatchDeclarationIsMadeForException()
	{
		var code =
@"using System;

public static class Test
{
	public static void DoSomething()
	{
		try
		{
		}
		catch (Exception) { }
	}
}";
		var gatherer = new StatisticsGatherer(code);
		Assert.That(gatherer.BadCatchBlocksCount, Is.EqualTo(1));
	}

	[Test]
	public static void CheckBadCatchBlockCountWhenBlockIsNotEmptyAndCatchDeclarationIsMadeForException()
	{
		var code =
@"using System;

public static class Test
{
	public static void DoSomething()
	{
		try
		{
		}
		catch (Exception) 
		{
			Console.WriteLine(""I'm in the catch block."");
		}
	}
}";
		var gatherer = new StatisticsGatherer(code);
		Assert.That(gatherer.BadCatchBlocksCount, Is.EqualTo(0));
	}

	[Test]
	public static void CheckBadCatchBlockCountWhenBlockIsEmptyAndCatchDeclarationIsMadeForSystemException()
	{
		var code =
@"public static class Test
{
	public static void DoSomething()
	{
		try
		{
		}
		catch (System.Exception) { }
	}
}";
		var gatherer = new StatisticsGatherer(code);
		Assert.That(gatherer.BadCatchBlocksCount, Is.EqualTo(1));
	}
}