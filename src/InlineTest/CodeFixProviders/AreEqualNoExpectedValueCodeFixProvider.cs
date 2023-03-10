using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sungaila.InlineTest.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AreEqualNoExpectedValueCodeFixProvider)), Shared]
    internal class AreEqualNoExpectedValueCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(InlineTestAnalyzer.AreEqualNoExpectedValueId);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
#if FALSE && DEBUG
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Launch();
            }
#endif
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.Single();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var declaration = root!.FindToken(diagnosticSpan.Start).Parent!.FirstAncestorOrSelf<AttributeSyntax>();

            context.RegisterCodeFix(
                CodeAction.Create(
                    "Add Expected parameter",
                    c => AddExpectedParameterAsync(context.Document, declaration!, c),
                    "Add Expected parameter"
                    ),
                diagnostic);
        }

        private async Task<Document> AddExpectedParameterAsync(Document document, AttributeSyntax attributeSyntax, CancellationToken cancellationToken)
        {
            var newArgument = SyntaxFactory.AttributeArgument(
                SyntaxFactory.NameEquals(nameof(AreEqualAttribute.Expected)),
                null,
                SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression)
                );

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            SyntaxNode newRoot;

            if (attributeSyntax.ArgumentList == null)
            {
                newRoot = oldRoot!.ReplaceNode(attributeSyntax,
                    attributeSyntax.WithArgumentList(SyntaxFactory.AttributeArgumentList(SyntaxFactory.SeparatedList(new[] { newArgument }))));
            }
            else
            {
                newRoot = oldRoot!.ReplaceNode(attributeSyntax, attributeSyntax.AddArgumentListArguments(newArgument));
            }

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
