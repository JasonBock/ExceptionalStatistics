# ExceptionalStatistics

Using Roslyn to gather statistics on bad exception handling - i.e. empty "catch-all" blocks.

## Getting Started

Install the ExceptionalStatistics tool from NuGet - that's it.

### Prerequisites

The ExceptionalStatistics tool targets .NET 7.

## Usage

You can look for bad exceptional handlers by either providing the location of a solution file:

```
ExceptionalStatistics --sln "C:\MyDirectory\MySolutionFile.sln"
```

Or, you give a directory as the starting point - ExceptionalStatistics will enumerate all files within that directory and subdirectories for C# files:

```
ExceptionalStatistics --dir "C:\MyDirectory"
```

In either case, you should see output something like this:

```
Total number of files: 12704
Number of statements: 903409
Number of expressions: 3900845
Number of bad catch blocks: 43
Number of empty catch blocks with filters: 64

Bad Catch Clauses

        File: M:\repos\roslyn2\src\VisualStudio\Core\Def\EditAndContinue\EditAndContinueFeedbackDiagnosticFileProvider.cs
                Microsoft.VisualStudio.LanguageServices.EditAndContinue.EditAndContinueFeedbackDiagnosticFileProvider.OnFeedbackSemaphoreDeleted()
                Start line location: 134
                End line location: 134
        File: M:\repos\roslyn2\src\Workspaces\Remote\Core\BrokeredServiceConnection.cs
                Microsoft.CodeAnalysis.Remote.BrokeredServiceConnection.OnUnexpectedException()
                Start line location: 410
                End line location: 410
```

## Feedback

If you run into any issues, please add them [here](https://github.com/JasonBock/ExceptionalStatistics/issues).