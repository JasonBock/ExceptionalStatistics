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

		#region Statements
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

		public override void VisitGlobalStatement(GlobalStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitGlobalStatement(node);
		}

		public override void VisitGotoStatement(GotoStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitGotoStatement(node);
		}

		public override void VisitIfStatement(IfStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitIfStatement(node);
		}

		public override void VisitLabeledStatement(LabeledStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitLabeledStatement(node);
		}

		public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitLocalDeclarationStatement(node);
		}

		public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitLocalFunctionStatement(node);
		}

		public override void VisitLockStatement(LockStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitLockStatement(node);
		}

		public override void VisitReturnStatement(ReturnStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitReturnStatement(node);
		}

		public override void VisitSwitchStatement(SwitchStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitSwitchStatement(node);
		}

		public override void VisitThrowStatement(ThrowStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitThrowStatement(node);
		}

		public override void VisitTryStatement(TryStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitTryStatement(node);
		}

		public override void VisitUnsafeStatement(UnsafeStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitUnsafeStatement(node);
		}

		public override void VisitUsingStatement(UsingStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitUsingStatement(node);
		}

		public override void VisitWhileStatement(WhileStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitWhileStatement(node);
		}

		public override void VisitYieldStatement(YieldStatementSyntax node)
		{
			this.StatementsCount++;
			base.VisitYieldStatement(node);
		}
		#endregion

		#region Expressions
		public override void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitAnonymousMethodExpression(node);
		}

		public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitAnonymousObjectCreationExpression(node);
		}

		public override void VisitArrayCreationExpression(ArrayCreationExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitArrayCreationExpression(node);
		}

		public override void VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitArrowExpressionClause(node);
		}

		public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitAssignmentExpression(node);
		}

		public override void VisitAwaitExpression(AwaitExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitAwaitExpression(node);
		}

		public override void VisitBaseExpression(BaseExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitBaseExpression(node);
		}

		public override void VisitBinaryExpression(BinaryExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitBinaryExpression(node);
		}

		public override void VisitCastExpression(CastExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitCastExpression(node);
		}

		public override void VisitCheckedExpression(CheckedExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitCheckedExpression(node);
		}

		public override void VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitConditionalAccessExpression(node);
		}

		public override void VisitConditionalExpression(ConditionalExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitConditionalExpression(node);
		}

		public override void VisitDeclarationExpression(DeclarationExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitDeclarationExpression(node);
		}

		public override void VisitDefaultExpression(DefaultExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitDefaultExpression(node);
		}

		public override void VisitElementAccessExpression(ElementAccessExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitElementAccessExpression(node);
		}

		public override void VisitElementBindingExpression(ElementBindingExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitElementBindingExpression(node);
		}

		public override void VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitImplicitArrayCreationExpression(node);
		}

		public override void VisitImplicitObjectCreationExpression(ImplicitObjectCreationExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitImplicitObjectCreationExpression(node);
		}

		public override void VisitImplicitStackAllocArrayCreationExpression(ImplicitStackAllocArrayCreationExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitImplicitStackAllocArrayCreationExpression(node);
		}

		public override void VisitInitializerExpression(InitializerExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitInitializerExpression(node);
		}

		public override void VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitInterpolatedStringExpression(node);
		}

		public override void VisitInvocationExpression(InvocationExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitInvocationExpression(node);
		}

		public override void VisitIsPatternExpression(IsPatternExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitIsPatternExpression(node);
		}

		public override void VisitLiteralExpression(LiteralExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitLiteralExpression(node);
		}

		public override void VisitMakeRefExpression(MakeRefExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitMakeRefExpression(node);
		}

		public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitMemberAccessExpression(node);
		}

		public override void VisitMemberBindingExpression(MemberBindingExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitMemberBindingExpression(node);
		}

		public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitObjectCreationExpression(node);
		}

		public override void VisitOmittedArraySizeExpression(OmittedArraySizeExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitOmittedArraySizeExpression(node);
		}

		public override void VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitParenthesizedExpression(node);
		}

		public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitParenthesizedLambdaExpression(node);
		}

		public override void VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitPostfixUnaryExpression(node);
		}

		public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitPrefixUnaryExpression(node);
		}

		public override void VisitQueryExpression(QueryExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitQueryExpression(node);
		}

		public override void VisitRangeExpression(RangeExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitRangeExpression(node);
		}

		public override void VisitRefExpression(RefExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitRefExpression(node);
		}

		public override void VisitRefTypeExpression(RefTypeExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitRefTypeExpression(node);
		}

		public override void VisitRefValueExpression(RefValueExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitRefValueExpression(node);
		}

		public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitSimpleLambdaExpression(node);
		}

		public override void VisitSizeOfExpression(SizeOfExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitSizeOfExpression(node);
		}

		public override void VisitStackAllocArrayCreationExpression(StackAllocArrayCreationExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitStackAllocArrayCreationExpression(node);
		}

		public override void VisitSwitchExpression(SwitchExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitSwitchExpression(node);
		}

		public override void VisitSwitchExpressionArm(SwitchExpressionArmSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitSwitchExpressionArm(node);
		}

		public override void VisitThisExpression(ThisExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitThisExpression(node);
		}

		public override void VisitThrowExpression(ThrowExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitThrowExpression(node);
		}

		public override void VisitTupleExpression(TupleExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitTupleExpression(node);
		}

		public override void VisitTypeOfExpression(TypeOfExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitTypeOfExpression(node);
		}

		public override void VisitWithExpression(WithExpressionSyntax node)
		{
			this.ExpressionsCount++;
			base.VisitWithExpression(node);
		}
		#endregion

		public override void VisitCatchClause(CatchClauseSyntax node)
		{
			// If the block has no nodes,
			// and either it doesn't have a CatchDeclarationSyntax child
			// or the name of the CatchDeclarationSyntax child
			// is either "Exception" or "System.Exception"

			// TODO: I may have to try and "get" a SemanticModel
			// to determine exactly what the exception type is.

			// TODO: I want to capture where this is happening
			// So, I may have to recursively look at the parents
			// and look for a node that is of type MemberDeclarationSyntax
			// (maybe I can be specific and say MethodDeclarationSyntax)
			// Maybe I just keep a running List<CatchClauseSyntax> and
			// provide an ImmutableArray<CatchClauseSyntax> as a getter
			// that I can look at later.

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