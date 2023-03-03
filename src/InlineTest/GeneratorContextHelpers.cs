using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Sungaila.InlineTest
{
    internal static class GeneratorContextHelpers
    {
        public static string GetFullyQualifiedMetadataName(this ISymbol namedTypeSymbol)
        {
            return namedTypeSymbol.ToDisplayString(new SymbolDisplayFormat(
                globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                memberOptions: SymbolDisplayMemberOptions.IncludeContainingType,
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters));
        }

        public static Microsoft.CodeAnalysis.TypeInfo GetTypeInfo(this SyntaxNode source, GeneratorExecutionContext context)
        {
            return context.Compilation.GetSemanticModel(source.SyntaxTree, true).GetTypeInfo(source, context.CancellationToken);
        }

        public static ISymbol GetDeclaredSymbol(this SyntaxNode source, GeneratorExecutionContext context)
        {
            return context.Compilation.GetSemanticModel(source.SyntaxTree, true).GetDeclaredSymbol(source) ?? throw new InvalidOperationException();
        }

        public static bool ConversionExists(this SyntaxNode source, GeneratorExecutionContext context, ITypeSymbol destination)
        {
            var sourceTypeInfo = source.GetTypeInfo(context);
            var result = context.Compilation.ClassifyConversion(sourceTypeInfo.ConvertedType!, destination).Exists;

            if (!result && sourceTypeInfo.ConvertedType is ITypeSymbol namedTypeSymbol)
            {
                result = namedTypeSymbol.MetadataName == destination.MetadataName;
            }

            return result;
        }

        public static string InvokeMethod(this MethodDeclarationSyntax methodDef, GeneratorExecutionContext context, bool withSemicolon, bool ignoreAsync)
        {
            var paramWithoutTypeList = string.Join(", ", methodDef.ParameterList.Parameters.Select(p => p.Identifier.ValueText.Trim()));
            bool isAsyncMethod = methodDef.IsAsync() && !ignoreAsync;

            if (methodDef.Modifiers.Any(m => m.ValueText == "static"))
            {
                return $"{(isAsyncMethod ? "await " : string.Empty)}{methodDef.GetDeclaredSymbol(context).GetFullyQualifiedMetadataName()}({paramWithoutTypeList}){(withSemicolon ? ";" : string.Empty)}";
            }

            if (methodDef.FirstAncestorOrSelf<ClassDeclarationSyntax>() is not ClassDeclarationSyntax classDef)
                throw new InvalidOperationException();

            return $"{(isAsyncMethod ? "await " : string.Empty)}new {classDef.GetDeclaredSymbol(context).GetFullyQualifiedMetadataName()}().{methodDef.Identifier.ValueText}({paramWithoutTypeList}){(withSemicolon ? ";" : string.Empty)}";
        }

        public static string InvokeMethodWithResult(this MethodDeclarationSyntax methodDef, GeneratorExecutionContext context)
        {
            return $"var result = {methodDef.InvokeMethod(context, true, false)}";
        }

        public static void GetArgumentDisplay(this AttributeSyntax attribute, out string argumentList, out List<string> argumentListDisplay)
        {
            if (attribute.FirstAncestorOrSelf<MethodDeclarationSyntax>() is not MethodDeclarationSyntax methodDef)
                throw new InvalidOperationException();

            var args = attribute.ArgumentList?.Arguments.Where(a => a.NameEquals == null);
            argumentList = string.Join(", ", args?.Select(p => p.Expression.GetText().ToString().Trim()) ?? new string[] { });

            argumentListDisplay = new List<string>();

            if (args == null)
                return;

            for (int i = 0; i < args.Count() && i < methodDef.ParameterList.Parameters.Count; i++)
            {
                argumentListDisplay.Add($"{methodDef.ParameterList.Parameters.ElementAt(i).Identifier.ValueText.Trim()}: {args.ElementAt(i).Expression.GetText().ToString().Replace("\"", "\\\"").Trim()}");
            }
        }

        public static string GetDisplayName(this MethodDeclarationSyntax methodDef, List<string> argumentListDisplay, string suffix)
        {
            return $"{methodDef.Identifier.ValueText}({string.Join(", ", argumentListDisplay)}){suffix.Replace("\"", "\\\"")}";
        }

        public static string ModifyForAsync(this string expectedType, MethodDeclarationSyntax methodDef)
        {
            if (!methodDef.Modifiers.Any(m => m.ValueText == "async"))
                return expectedType;

            if (expectedType.StartsWith("global::System.Threading.Tasks.Task<"))
                return expectedType.Substring("global::System.Threading.Tasks.Task<".Length, expectedType.Length - "global::System.Threading.Tasks.Task<".Length - 1);

            if (expectedType.StartsWith("global::System.Threading.Tasks.ValueTask<"))
                return expectedType.Substring("global::System.Threading.Tasks.ValueTask<".Length, expectedType.Length - "global::System.Threading.Tasks.ValueTask<".Length - 1);

            throw new InvalidOperationException();
        }

        public static bool IsAsync(this MethodDeclarationSyntax methodDef)
        {
            return methodDef.Modifiers.Any(m => m.ValueText == "async");
        }

        public static string GetTestMethodModifiers(this MethodDeclarationSyntax methodDef)
        {
            return methodDef.IsAsync() ? "public async global::System.Threading.Tasks.Task" : "public void";
        }
    }
}