using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace Sungaila.InlineTest
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class InlineTestAnalyzer : DiagnosticAnalyzer
    {
        public const string ParameterCountExceededId = "IT0000";

        private static readonly DiagnosticDescriptor ParameterCountExceededRule = new(
                                            ParameterCountExceededId,
                                            "Method exceeds 15 parameters limit for inline tests",
                                            "Method exceeds 15 parameters limit for inline tests",
                                            "Method",
                                            DiagnosticSeverity.Error,
                                            true,
                                            "A method cannot have more than 15 parameters for inline tests. This is a technical limitation because DataRowAttribute supports 16 parameters only (one parameter is reserved for the expected value).",
                                            "https://github.com/sungaila/InlineTest"
                                        );

        public const string ParameterCountMismatchId = "IT0001";

        private static readonly DiagnosticDescriptor ParameterCountMismatchRule = new(
                                            ParameterCountMismatchId,
                                            "Method parameter count mismatch",
                                            "Method parameter count mismatch",
                                            "Method",
                                            DiagnosticSeverity.Error,
                                            true,
                                            "The method parameter count and the inline test parameter count must match. Please note that optional parameters do not count.",
                                            "https://github.com/sungaila/InlineTest"
                                        );

        public const string ParameterTypesMismatchId = "IT0002";

        private static readonly DiagnosticDescriptor ParameterTypesMismatchRule = new(
                                            ParameterTypesMismatchId,
                                            "Method parameter types mismatch",
                                            "Method parameter types mismatch",
                                            "Method",
                                            DiagnosticSeverity.Error,
                                            true,
                                            "The method parameter types and the inline test parameter types must match.",
                                            "https://github.com/sungaila/InlineTest"
                                        );

        public const string AreEqualNoReturnTypeId = "IT0003";

        private static readonly DiagnosticDescriptor AreEqualNoReturnTypeRule = new(
                                            AreEqualNoReturnTypeId,
                                            "Method has no return type but compares values",
                                            "Method has no return type but compares values",
                                            "Method",
                                            DiagnosticSeverity.Warning,
                                            true,
                                            $"Inline tests cannot compare expected and actual values if a method has no return type.",
                                            "https://github.com/sungaila/InlineTest"
                                        );

        public const string AreEqualNoExpectedValueId = "IT0004";

        private static readonly DiagnosticDescriptor AreEqualNoExpectedValueRule = new(
                                            AreEqualNoExpectedValueId,
                                            $"{nameof(AreEqualAttribute)} missing {nameof(AreEqualAttribute.Expected)} property",
                                            $"{nameof(AreEqualAttribute)} missing {nameof(AreEqualAttribute.Expected)} property",
                                            "Style",
                                            DiagnosticSeverity.Warning,
                                            true,
                                            $"{nameof(AreEqualAttribute)} should set the {nameof(AreEqualAttribute.Expected)} property explicitly. Otherwise default is used as fallback.",
                                            "https://github.com/sungaila/InlineTest"
                                        );

        public const string AreNotEqualNoNotExpectedValueId = "IT0005";

        private static readonly DiagnosticDescriptor AreNotEqualNoNotExpectedValueRule = new(
                                            AreNotEqualNoNotExpectedValueId,
                                            $"{nameof(AreNotEqualAttribute)} missing {nameof(AreNotEqualAttribute.NotExpected)} property",
                                            $"{nameof(AreNotEqualAttribute)} missing {nameof(AreNotEqualAttribute.NotExpected)} property",
                                            "Style",
                                            DiagnosticSeverity.Warning,
                                            true,
                                            $"{nameof(AreNotEqualAttribute)} should set the {nameof(AreNotEqualAttribute.NotExpected)} property explicitly. Otherwise default is used as fallback.",
                                            "https://github.com/sungaila/InlineTest"
                                        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
            ParameterCountExceededRule,
            ParameterCountMismatchRule,
            ParameterTypesMismatchRule,
            AreEqualNoReturnTypeRule,
            AreEqualNoExpectedValueRule,
            AreNotEqualNoNotExpectedValueRule);

        public override void Initialize(AnalysisContext context)
        {
#if FALSE && DEBUG
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Launch();
            }
#endif
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
        }

        private void AnalyzeMethod(SymbolAnalysisContext context)
        {
            if (context.Symbol is not IMethodSymbol methodSymbol)
                return;

            if (methodSymbol.Parameters.Length > 15)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        ParameterCountExceededRule,
                        methodSymbol.Locations.First(l => l.IsInSource),
                        methodSymbol.Name));
            }

            foreach (var attr in methodSymbol.GetAttributes().Where(a => a.AttributeClass?.BaseType?.GetFullyQualifiedMetadataNameWithoutTypeParameters() == typeof(InlineTestAttributeBase).FullName))
            {
                var attrParamCount = attr.ConstructorArguments.FirstOrDefault().Values.Count();

                if (attrParamCount < methodSymbol.Parameters.Count(p => !p.IsOptional) ||
                    attrParamCount > methodSymbol.Parameters.Length)
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            ParameterCountMismatchRule,
                            attr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation(),
                            methodSymbol.Name));
                }

                for (int i = 0; i < methodSymbol.Parameters.Length && i < attrParamCount; i++)
                {
                    // TODO
                }
            }

            foreach (var areEqualAttr in methodSymbol.GetAttributes().Where(a => a.AttributeClass?.GetFullyQualifiedMetadataNameWithoutTypeParameters() == typeof(AreEqualAttribute).FullName))
            {
                if (!areEqualAttr.NamedArguments.Any(a => a.Key == nameof(AreEqualAttribute.Expected)))
                {
                    context.ReportDiagnostic(Diagnostic.Create(AreEqualNoExpectedValueRule,
                        areEqualAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation(),
                        methodSymbol.Name));
                }

                if (methodSymbol.ReturnsVoid)
                {
                    context.ReportDiagnostic(Diagnostic.Create(AreEqualNoReturnTypeRule,
                        areEqualAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation(),
                        methodSymbol.Name));
                }
            }

            foreach (var areNotEqualAttr in methodSymbol.GetAttributes().Where(a => a.AttributeClass?.GetFullyQualifiedMetadataNameWithoutTypeParameters() == typeof(AreNotEqualAttribute).FullName))
            {
                if (!areNotEqualAttr.NamedArguments.Any(a => a.Key == nameof(AreNotEqualAttribute.NotExpected)))
                {
                    context.ReportDiagnostic(Diagnostic.Create(AreNotEqualNoNotExpectedValueRule,
                        areNotEqualAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation(),
                        methodSymbol.Name));
                }

                if (methodSymbol.ReturnsVoid)
                {
                    context.ReportDiagnostic(Diagnostic.Create(AreEqualNoReturnTypeRule,
                        areNotEqualAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation(),
                        methodSymbol.Name));
                }
            }

            foreach (var isTrueAttr in methodSymbol.GetAttributes().Where(a => a.AttributeClass?.GetFullyQualifiedMetadataNameWithoutTypeParameters() == typeof(IsTrueAttribute).FullName))
            {
                if (methodSymbol.ReturnsVoid)
                {
                    context.ReportDiagnostic(Diagnostic.Create(AreEqualNoReturnTypeRule,
                        isTrueAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation(),
                        methodSymbol.Name));
                }
            }

            foreach (var isFalseAttr in methodSymbol.GetAttributes().Where(a => a.AttributeClass?.GetFullyQualifiedMetadataNameWithoutTypeParameters() == typeof(IsFalseAttribute).FullName))
            {
                if (methodSymbol.ReturnsVoid)
                {
                    context.ReportDiagnostic(Diagnostic.Create(AreEqualNoReturnTypeRule,
                        isFalseAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation(),
                        methodSymbol.Name));
                }
            }

            foreach (var isNullAttr in methodSymbol.GetAttributes().Where(a => a.AttributeClass?.GetFullyQualifiedMetadataNameWithoutTypeParameters() == typeof(IsNullAttribute).FullName))
            {
                if (methodSymbol.ReturnsVoid)
                {
                    context.ReportDiagnostic(Diagnostic.Create(AreEqualNoReturnTypeRule,
                        isNullAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation(),
                        methodSymbol.Name));
                }
            }

            foreach (var isNotNullAttr in methodSymbol.GetAttributes().Where(a => a.AttributeClass?.GetFullyQualifiedMetadataNameWithoutTypeParameters() == typeof(IsNotNullAttribute).FullName))
            {
                if (methodSymbol.ReturnsVoid)
                {
                    context.ReportDiagnostic(Diagnostic.Create(AreEqualNoReturnTypeRule,
                        isNotNullAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation(),
                        methodSymbol.Name));
                }
            }

            foreach (var isInstanceOfTypeAttr in methodSymbol.GetAttributes().Where(a => $"{a.AttributeClass?.GetFullyQualifiedMetadataNameWithoutTypeParameters()}`1" == typeof(IsInstanceOfTypeAttribute<>).FullName))
            {
                if (methodSymbol.ReturnsVoid)
                {
                    context.ReportDiagnostic(Diagnostic.Create(AreEqualNoReturnTypeRule,
                        isInstanceOfTypeAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation(),
                        methodSymbol.Name));
                }
            }

            foreach (var isNotInstanceOfTypeAttr in methodSymbol.GetAttributes().Where(a => $"{a.AttributeClass?.GetFullyQualifiedMetadataNameWithoutTypeParameters()}`1" == typeof(IsNotInstanceOfTypeAttribute<>).FullName))
            {
                if (methodSymbol.ReturnsVoid)
                {
                    context.ReportDiagnostic(Diagnostic.Create(AreEqualNoReturnTypeRule,
                        isNotInstanceOfTypeAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation(),
                        methodSymbol.Name));
                }
            }

            foreach (var throwsExceptionAttr in methodSymbol.GetAttributes().Where(a => $"{a.AttributeClass?.GetFullyQualifiedMetadataNameWithoutTypeParameters()}`1" == typeof(ThrowsExceptionAttribute<>).FullName))
            {
                // NOP
            }
        }
    }
}