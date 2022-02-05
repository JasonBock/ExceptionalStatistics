using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExceptionalStatistics.Core;

public sealed class StatisticsGatherer
{
	public StatisticsGatherer(string code) =>
		(this.ExpressionsCount, this.StatementsCount, this.BadCatchBlocksCount) =
			new StatisticsWalker(SyntaxFactory.ParseCompilationUnit(code));

	public uint BadCatchBlocksCount { get; }
	public uint ExpressionsCount { get; }
	public uint StatementsCount { get; }

	private sealed class StatisticsWalker
		: CSharpSyntaxWalker
	{
		public StatisticsWalker(CompilationUnitSyntax unit) => this.VisitCompilationUnit(unit);

		public void Deconstruct(out uint expressionsCount, out uint statementsCount, out uint badCatchBlocksCount) =>
			(expressionsCount, statementsCount, badCatchBlocksCount) =
				(this.ExpressionsCount, this.StatementsCount, this.BadCatchBlocksCount);

		public override void VisitExpressionStatement(ExpressionStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitExpressionStatement(node);
		}

		public override void VisitBreakStatement(BreakStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitBreakStatement(node);
		}

		public override void VisitCheckedStatement(CheckedStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitCheckedStatement(node);
		}

		public override void VisitContinueStatement(ContinueStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitContinueStatement(node);
		}

		public override void VisitDoStatement(DoStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitDoStatement(node);
		}

		public override void VisitEmptyStatement(EmptyStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitEmptyStatement(node);
		}

		public override void VisitFixedStatement(FixedStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitFixedStatement(node);
		}

		public override void VisitForEachStatement(ForEachStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitForEachStatement(node);
		}

		public override void VisitForEachVariableStatement(ForEachVariableStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitForEachVariableStatement(node);
		}

		public override void VisitForStatement(ForStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitForStatement(node);
		}

		public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitParenthesizedLambdaExpression(node);
		}

		public override void VisitCatchClause(CatchClauseSyntax node)
		{
			// If the block has no nodes,
			// and either it doesn't have a CatchDeclarationSyntax child
			// or the name of the CatchDeclarationSyntax child
			// is either "Exception" or "System.Exception"
			// TODO: I may have to try and "get" a SemanticModel
			// to determine exactly what the exception type is.
			if (!node.Block.DescendantNodes(_ => true).Any())
			{
				var catchDeclaration = node.DescendantNodes().OfType<CatchDeclarationSyntax>().SingleOrDefault();

				if (catchDeclaration is null ||
					catchDeclaration.Type.ToString() == "Exception" ||
					catchDeclaration.Type.ToString() == "System.Exception")
				{
					this.BadCatchBlocksCount++;
				}
			}

			base.VisitCatchClause(node);
		}

		public uint BadCatchBlocksCount { get; private set; }
		public uint ExpressionsCount { get; private set; }
		public uint StatementsCount { get; private set; }
	}
}