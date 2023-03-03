using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sungaila.InlineTest
{
    [Generator]
    internal class InlineTestGenerator : ISourceGenerator
    {
        /// <inheritdoc/>
        public void Execute(GeneratorExecutionContext context)
        {
            var testSymbol = context.Compilation.GetTypeByMetadataName(typeof(InlineTestAttributeBase).FullName) ?? throw new InvalidOperationException();
            var areEqualSymbol = context.Compilation.GetTypeByMetadataName(typeof(AreEqualAttribute).FullName) ?? throw new InvalidOperationException();
            var areNotEqualSymbol = context.Compilation.GetTypeByMetadataName(typeof(AreNotEqualAttribute).FullName) ?? throw new InvalidOperationException();
            var isTrueSymbol = context.Compilation.GetTypeByMetadataName(typeof(IsTrueAttribute).FullName) ?? throw new InvalidOperationException();
            var isFalseSymbol = context.Compilation.GetTypeByMetadataName(typeof(IsFalseAttribute).FullName) ?? throw new InvalidOperationException();
            var isNullSymbol = context.Compilation.GetTypeByMetadataName(typeof(IsNullAttribute).FullName) ?? throw new InvalidOperationException();
            var isNotNullSymbol = context.Compilation.GetTypeByMetadataName(typeof(IsNotNullAttribute).FullName) ?? throw new InvalidOperationException();
            var isInstanceOfTypeSymbol = context.Compilation.GetTypeByMetadataName(typeof(IsInstanceOfTypeAttribute<>).GetGenericTypeDefinition().FullName) ?? throw new InvalidOperationException();
            var isNotInstanceOfTypeSymbol = context.Compilation.GetTypeByMetadataName(typeof(IsNotInstanceOfTypeAttribute<>).GetGenericTypeDefinition().FullName) ?? throw new InvalidOperationException();
            var throwsExceptionSymbol = context.Compilation.GetTypeByMetadataName(typeof(ThrowsExceptionAttribute<>).GetGenericTypeDefinition().FullName) ?? throw new InvalidOperationException();

            var attributeDefs = context
                .Compilation
                .SyntaxTrees
                .SelectMany(t => t.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>())
                .SelectMany(m => m.AttributeLists)
                .SelectMany(l => l.Attributes)
                .Where(a => a.ConversionExists(context, testSymbol))
                .Distinct()
                .ToList();

            var classDefs = attributeDefs
                .Select(a => a.FirstAncestorOrSelf<ClassDeclarationSyntax>())
                .OfType<ClassDeclarationSyntax>()
                .Distinct()
                .ToList();

            var methodDefs = attributeDefs
                .Select(a => a.FirstAncestorOrSelf<MethodDeclarationSyntax>())
                .OfType<MethodDeclarationSyntax>()
                .Distinct()
                .ToList();

            var sb = new StringBuilder();

            sb.AppendLine("#nullable enable");
            sb.AppendLine("using global::Microsoft.VisualStudio.TestTools.UnitTesting;");
            sb.AppendLine("using global::System.CodeDom.Compiler;");
            sb.AppendLine();
            sb.AppendLine($"namespace Sungaila.InlineTest.Generated");
            sb.Append("{");

            foreach (var classDef in classDefs)
            {
                if (classDef.FirstAncestorOrSelf<NamespaceDeclarationSyntax>() is not NamespaceDeclarationSyntax namespaceDef)
                    continue;

                var classTypeInfo = classDef.GetDeclaredSymbol(context);

                sb.AppendLine();
                sb.AppendLine("\t/// <summary>");
                sb.AppendLine($"\t/// Test class for <seealso cref=\"{classTypeInfo.GetFullyQualifiedMetadataName()}\"/>.");
                sb.AppendLine("\t/// </summary>");
                sb.AppendLine($"\t[GeneratedCode(\"{typeof(InlineTestGenerator).Assembly.GetName().Name}\", \"{typeof(InlineTestGenerator).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? typeof(InlineTestGenerator).Assembly.GetName().Version.ToString()}\")]");
                sb.AppendLine("\t[TestClass]");
                sb.AppendLine($"\tpublic partial class {classDef.Identifier.ValueText}Tests");
                sb.Append("\t{");

                foreach (var methodDef in methodDefs.Where(m => m.FirstAncestorOrSelf<ClassDeclarationSyntax>((c) => c == classDef) != null))
                {
                    var allAttributes = attributeDefs.Where(a => a.FirstAncestorOrSelf<MethodDeclarationSyntax>((c) => c == methodDef) != null).ToList();
                    var areEqualAttributes = allAttributes.Where(a => a.ConversionExists(context, areEqualSymbol)).ToList();
                    var areNotEqualAttributes = allAttributes.Where(a => a.ConversionExists(context, areNotEqualSymbol)).ToList();
                    var isTrueAttributes = allAttributes.Where(a => a.ConversionExists(context, isTrueSymbol)).ToList();
                    var isFalseAttributes = allAttributes.Where(a => a.ConversionExists(context, isFalseSymbol)).ToList();
                    var isNullAttributes = allAttributes.Where(a => a.ConversionExists(context, isNullSymbol)).ToList();
                    var isNotNullAttributes = allAttributes.Where(a => a.ConversionExists(context, isNotNullSymbol)).ToList();
                    var isInstanceOfTypeAttributes = allAttributes.Where(a => a.ConversionExists(context, isInstanceOfTypeSymbol)).ToList();
                    var isNotInstanceOfTypeAttributes = allAttributes.Where(a => a.ConversionExists(context, isNotInstanceOfTypeSymbol)).ToList();
                    var throwsExceptionAttributes = allAttributes.Where(a => a.ConversionExists(context, throwsExceptionSymbol)).ToList();

                    var paramList = string.Join(", ", methodDef.ParameterList.Parameters.Select(p => p.GetText().ToString().Trim()));
                    var paramWithoutTypeList = string.Join(", ", methodDef.ParameterList.Parameters.Select(p => p.Identifier.ValueText.Trim()));

                    var expectedType = methodDef.ReturnType.GetTypeInfo(context).ConvertedType!.GetFullyQualifiedMetadataName();
                    var methodName = $"{methodDef.Identifier.ValueText}%AttributeKind%";
                    string? methodSignature = null;
                    string? attributeKind = null;

                    if (areEqualAttributes.Any())
                    {
                        attributeKind = nameof(Assert.AreEqual);
                        expectedType = expectedType.ModifyForAsync(methodDef);
                        methodSignature = $"{methodName}({paramList}{(!string.IsNullOrWhiteSpace(paramList) ? ", " : string.Empty)}{expectedType} expected)";

                        sb.AppendLine();

                        bool hasNullability = context.Compilation.GetSemanticModel(methodDef.SyntaxTree, true).GetTypeInfo(areEqualAttributes.First()).ConvertedNullability.FlowState != NullableFlowState.None;

                        if (!hasNullability)
                            sb.AppendLine("#nullable disable");

                        sb.AppendLine("\t\t/// <summary>");
                        sb.AppendLine($"\t\t/// Test method for <seealso cref=\"{methodDef.GetDeclaredSymbol(context)!.GetFullyQualifiedMetadataName()}\"/> using <seealso cref=\"global::Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual{{}}\"/>.");
                        sb.AppendLine("\t\t/// </summary>");
                        sb.AppendLine($"\t\t[TestMethod]");

                        foreach (var areEqualAttr in areEqualAttributes)
                        {
                            var expectedArgument = areEqualAttr.ArgumentList?.Arguments.SingleOrDefault(a => a.NameEquals?.Name?.GetText().ToString().Trim() == nameof(AreEqualAttribute.Expected));
                            var expected = expectedArgument?.Expression.GetText().ToString() ?? $"default({expectedType})";

                            areEqualAttr.GetArgumentDisplay(out var argList, out var argDisplay);
                            sb.AppendLine($"\t\t[DataRow({argList}{(!string.IsNullOrWhiteSpace(argList) ? ", " : string.Empty)}{expected}, DisplayName = \"{methodDef.GetDisplayName(argDisplay, $" is {expected}")}\")]");
                        }

                        sb.AppendLine($"\t\t{methodDef.GetTestMethodModifiers()} {methodSignature.Replace("%AttributeKind%", attributeKind)}");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine($"\t\t\t{methodDef.InvokeMethodWithResult(context)}");
                        sb.AppendLine("\t\t\tAssert.AreEqual(expected, result);");
                        sb.AppendLine("\t\t}");

                        if (!hasNullability)
                            sb.AppendLine("#nullable enable");
                    }

                    if (areNotEqualAttributes.Any())
                    {
                        attributeKind = nameof(Assert.AreNotEqual);
                        expectedType = expectedType.ModifyForAsync(methodDef);
                        methodSignature = $"{methodName}({paramList}{(!string.IsNullOrWhiteSpace(paramList) ? ", " : string.Empty)}{expectedType} notExpected)";

                        sb.AppendLine();

                        bool hasNullability = context.Compilation.GetSemanticModel(methodDef.SyntaxTree, true).GetTypeInfo(areNotEqualAttributes.First()).ConvertedNullability.FlowState != NullableFlowState.None;

                        if (!hasNullability)
                            sb.AppendLine("#nullable disable");

                        sb.AppendLine("\t\t/// <summary>");
                        sb.AppendLine($"\t\t/// Test method for <seealso cref=\"{methodDef.GetDeclaredSymbol(context)!.GetFullyQualifiedMetadataName()}\"/> using <seealso cref=\"global::Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual{{}}\"/>.");
                        sb.AppendLine("\t\t/// </summary>");
                        sb.AppendLine($"\t\t[TestMethod]");

                        foreach (var areNotEqualAttr in areNotEqualAttributes)
                        {
                            var notExpectedArgument = areNotEqualAttr.ArgumentList?.Arguments.SingleOrDefault(a => a.NameEquals?.Name?.GetText().ToString().Trim() == nameof(AreNotEqualAttribute.NotExpected));
                            var notExpected = notExpectedArgument?.Expression.GetText().ToString() ?? $"default({expectedType})";

                            areNotEqualAttr.GetArgumentDisplay(out var argList, out var argDisplay);
                            sb.AppendLine($"\t\t[DataRow({argList}{(!string.IsNullOrWhiteSpace(argList) ? ", " : string.Empty)}{notExpected}, DisplayName = \"{methodDef.GetDisplayName(argDisplay, $" is not {notExpected}")}\")]");
                        }

                        sb.AppendLine($"\t\t{methodDef.GetTestMethodModifiers()} {methodSignature.Replace("%AttributeKind%", attributeKind)}");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine($"\t\t\t{methodDef.InvokeMethodWithResult(context)}");
                        sb.AppendLine("\t\t\tAssert.AreNotEqual(notExpected, result);");
                        sb.AppendLine("\t\t}");

                        if (!hasNullability)
                            sb.AppendLine("#nullable enable");
                    }

                    if (isTrueAttributes.Any())
                    {
                        attributeKind = nameof(Assert.IsTrue);
                        methodSignature = $"{methodName}({paramList})";

                        sb.AppendLine();

                        bool hasNullability = context.Compilation.GetSemanticModel(methodDef.SyntaxTree, true).GetTypeInfo(isTrueAttributes.First()).ConvertedNullability.FlowState != NullableFlowState.None;

                        if (!hasNullability)
                            sb.AppendLine("#nullable disable");

                        sb.AppendLine("\t\t/// <summary>");
                        sb.AppendLine($"\t\t/// Test method for <seealso cref=\"{methodDef.GetDeclaredSymbol(context)!.GetFullyQualifiedMetadataName()}\"/> using <seealso cref=\"global::Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue\"/>.");
                        sb.AppendLine("\t\t/// </summary>");
                        sb.AppendLine($"\t\t[TestMethod]");

                        foreach (var isTrueAttr in isTrueAttributes)
                        {
                            isTrueAttr.GetArgumentDisplay(out var argList, out var argDisplay);
                            sb.AppendLine($"\t\t[DataRow({argList}{(!string.IsNullOrWhiteSpace(argList) ? ", " : string.Empty)}DisplayName = \"{methodDef.GetDisplayName(argDisplay, " is true")}\")]");
                        }

                        sb.AppendLine($"\t\t{methodDef.GetTestMethodModifiers()} {methodSignature.Replace("%AttributeKind%", attributeKind)}");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine($"\t\t\t{methodDef.InvokeMethodWithResult(context)}");
                        sb.AppendLine("\t\t\tAssert.IsTrue(result);");
                        sb.AppendLine("\t\t}");

                        if (!hasNullability)
                            sb.AppendLine("#nullable enable");
                    }

                    if (isFalseAttributes.Any())
                    {
                        attributeKind = nameof(Assert.IsFalse);
                        methodSignature = $"{methodName}({paramList})";

                        sb.AppendLine();

                        bool hasNullability = context.Compilation.GetSemanticModel(methodDef.SyntaxTree, true).GetTypeInfo(isFalseAttributes.First()).ConvertedNullability.FlowState != NullableFlowState.None;

                        if (!hasNullability)
                            sb.AppendLine("#nullable disable");

                        sb.AppendLine("\t\t/// <summary>");
                        sb.AppendLine($"\t\t/// Test method for <seealso cref=\"{methodDef.GetDeclaredSymbol(context)!.GetFullyQualifiedMetadataName()}\"/> using <seealso cref=\"global::Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse\"/>.");
                        sb.AppendLine("\t\t/// </summary>");
                        sb.AppendLine($"\t\t[TestMethod]");

                        foreach (var isFalseAttr in isFalseAttributes)
                        {
                            isFalseAttr.GetArgumentDisplay(out var argList, out var argDisplay);
                            sb.AppendLine($"\t\t[DataRow({argList}{(!string.IsNullOrWhiteSpace(argList) ? ", " : string.Empty)}DisplayName = \"{methodDef.GetDisplayName(argDisplay, " is false")}\")]");
                        }

                        sb.AppendLine($"\t\t{methodDef.GetTestMethodModifiers()} {methodSignature.Replace("%AttributeKind%", attributeKind)}");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine($"\t\t\t{methodDef.InvokeMethodWithResult(context)}");
                        sb.AppendLine("\t\t\tAssert.IsFalse(result);");
                        sb.AppendLine("\t\t}");

                        if (!hasNullability)
                            sb.AppendLine("#nullable enable");
                    }

                    if (isNullAttributes.Any())
                    {
                        attributeKind = nameof(Assert.IsNull);
                        methodSignature = $"{methodName}({paramList})";

                        sb.AppendLine();

                        bool hasNullability = context.Compilation.GetSemanticModel(methodDef.SyntaxTree, true).GetTypeInfo(isNullAttributes.First()).ConvertedNullability.FlowState != NullableFlowState.None;

                        if (!hasNullability)
                            sb.AppendLine("#nullable disable");

                        sb.AppendLine("\t\t/// <summary>");
                        sb.AppendLine($"\t\t/// Test method for <seealso cref=\"{methodDef.GetDeclaredSymbol(context)!.GetFullyQualifiedMetadataName()}\"/> using <seealso cref=\"global::Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNull\"/>.");
                        sb.AppendLine("\t\t/// </summary>");
                        sb.AppendLine($"\t\t[TestMethod]");

                        foreach (var isNullAttr in isNullAttributes)
                        {
                            isNullAttr.GetArgumentDisplay(out var argList, out var argDisplay);
                            sb.AppendLine($"\t\t[DataRow({argList}{(!string.IsNullOrWhiteSpace(argList) ? ", " : string.Empty)}DisplayName = \"{methodDef.GetDisplayName(argDisplay, " is null")}\")]");
                        }

                        sb.AppendLine($"\t\t{methodDef.GetTestMethodModifiers()} {methodSignature.Replace("%AttributeKind%", attributeKind)}");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine($"\t\t\t{methodDef.InvokeMethodWithResult(context)}");
                        sb.AppendLine("\t\t\tAssert.IsNull(result);");
                        sb.AppendLine("\t\t}");

                        if (!hasNullability)
                            sb.AppendLine("#nullable enable");
                    }

                    if (isNotNullAttributes.Any())
                    {
                        attributeKind = nameof(Assert.IsNotNull);
                        methodSignature = $"{methodName}({paramList})";

                        sb.AppendLine();

                        bool hasNullability = context.Compilation.GetSemanticModel(methodDef.SyntaxTree, true).GetTypeInfo(isNotNullAttributes.First()).ConvertedNullability.FlowState != NullableFlowState.None;

                        if (!hasNullability)
                            sb.AppendLine("#nullable disable");

                        sb.AppendLine("\t\t/// <summary>");
                        sb.AppendLine($"\t\t/// Test method for <seealso cref=\"{methodDef.GetDeclaredSymbol(context)!.GetFullyQualifiedMetadataName()}\"/> using <seealso cref=\"global::Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull\"/>.");
                        sb.AppendLine("\t\t/// </summary>");
                        sb.AppendLine($"\t\t[TestMethod]");

                        foreach (var isNotNullAttr in isNotNullAttributes)
                        {
                            isNotNullAttr.GetArgumentDisplay(out var argList, out var argDisplay);
                            sb.AppendLine($"\t\t[DataRow({argList}{(!string.IsNullOrWhiteSpace(argList) ? ", " : string.Empty)}DisplayName = \"{methodDef.GetDisplayName(argDisplay, " is not null")}\")]");
                        }

                        sb.AppendLine($"\t\t{methodDef.GetTestMethodModifiers()} {methodSignature.Replace("%AttributeKind%", attributeKind)}");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine($"\t\t\t{methodDef.InvokeMethodWithResult(context)}");
                        sb.AppendLine("\t\t\tAssert.IsNotNull(result);");
                        sb.AppendLine("\t\t}");

                        if (!hasNullability)
                            sb.AppendLine("#nullable enable");
                    }

                    if (isInstanceOfTypeAttributes.Any())
                    {
                        foreach (var genericType in isInstanceOfTypeAttributes.GroupBy(a => a.GetTypeInfo(context).ConvertedType!, SymbolEqualityComparer.Default))
                        {
                            if (genericType.Key is not INamedTypeSymbol symbol)
                                continue;

                            var fullyQualifiedMetadataName = symbol.TypeArguments.Single().GetFullyQualifiedMetadataName();
                            attributeKind = nameof(Assert.IsInstanceOfType);
                            methodSignature = $"{methodName}_{fullyQualifiedMetadataName.Replace("global::", string.Empty).Replace('.', '_')}({paramList})";

                            sb.AppendLine();

                            bool hasNullability = context.Compilation.GetSemanticModel(methodDef.SyntaxTree, true).GetTypeInfo(isInstanceOfTypeAttributes.First()).ConvertedNullability.FlowState != NullableFlowState.None;

                            if (!hasNullability)
                                sb.AppendLine("#nullable disable");

                            sb.AppendLine("\t\t/// <summary>");
                            sb.AppendLine($"\t\t/// Test method for <seealso cref=\"{methodDef.GetDeclaredSymbol(context)!.GetFullyQualifiedMetadataName()}\"/> using <seealso cref=\"global::Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull\"/>.");
                            sb.AppendLine("\t\t/// </summary>");
                            sb.AppendLine($"\t\t[TestMethod]");

                            foreach (var isInstanceOfTypeAttr in genericType)
                            {
                                isInstanceOfTypeAttr.GetArgumentDisplay(out var argList, out var argDisplay);
                                sb.AppendLine($"\t\t[DataRow({argList}{(!string.IsNullOrWhiteSpace(argList) ? ", " : string.Empty)}DisplayName = \"{methodDef.GetDisplayName(argDisplay, $" is instance of type {symbol.TypeArguments.Single().MetadataName}")}\")]");
                            }

                            sb.AppendLine($"\t\t{methodDef.GetTestMethodModifiers()} {methodSignature.Replace("%AttributeKind%", attributeKind)}");
                            sb.AppendLine("\t\t{");
                            sb.AppendLine($"\t\t\t{methodDef.InvokeMethodWithResult(context)}");
                            sb.AppendLine($"\t\t\tAssert.IsInstanceOfType<{fullyQualifiedMetadataName}>(result);");
                            sb.AppendLine("\t\t}");

                            if (!hasNullability)
                                sb.AppendLine("#nullable enable");
                        }
                    }

                    if (isNotInstanceOfTypeAttributes.Any())
                    {
                        foreach (var genericType in isNotInstanceOfTypeAttributes.GroupBy(a => a.GetTypeInfo(context).ConvertedType!, SymbolEqualityComparer.Default))
                        {
                            if (genericType.Key is not INamedTypeSymbol symbol)
                                continue;

                            var fullyQualifiedMetadataName = symbol.TypeArguments.Single().GetFullyQualifiedMetadataName();
                            attributeKind = nameof(Assert.IsNotInstanceOfType);
                            methodSignature = $"{methodName}_{fullyQualifiedMetadataName.Replace("global::", string.Empty).Replace('.', '_')}({paramList})";

                            sb.AppendLine();

                            bool hasNullability = context.Compilation.GetSemanticModel(methodDef.SyntaxTree, true).GetTypeInfo(isNotInstanceOfTypeAttributes.First()).ConvertedNullability.FlowState != NullableFlowState.None;

                            if (!hasNullability)
                                sb.AppendLine("#nullable disable");

                            sb.AppendLine("\t\t/// <summary>");
                            sb.AppendLine($"\t\t/// Test method for <seealso cref=\"{methodDef.GetDeclaredSymbol(context)!.GetFullyQualifiedMetadataName()}\"/> using <seealso cref=\"global::Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull\"/>.");
                            sb.AppendLine("\t\t/// </summary>");
                            sb.AppendLine($"\t\t[TestMethod]");

                            foreach (var isNotInstanceOfTypeAttr in genericType)
                            {
                                isNotInstanceOfTypeAttr.GetArgumentDisplay(out var argList, out var argDisplay);
                                sb.AppendLine($"\t\t[DataRow({argList}{(!string.IsNullOrWhiteSpace(argList) ? ", " : string.Empty)}DisplayName = \"{methodDef.GetDisplayName(argDisplay, $" is not instance of type {symbol.TypeArguments.Single().MetadataName}")}\")]");
                            }

                            sb.AppendLine($"\t\t{methodDef.GetTestMethodModifiers()} {methodSignature.Replace("%AttributeKind%", attributeKind)}");
                            sb.AppendLine("\t\t{");
                            sb.AppendLine($"\t\t\t{methodDef.InvokeMethodWithResult(context)}");
                            sb.AppendLine($"\t\t\tAssert.IsNotInstanceOfType<{fullyQualifiedMetadataName}>(result);");
                            sb.AppendLine("\t\t}");

                            if (!hasNullability)
                                sb.AppendLine("#nullable enable");
                        }
                    }

                    if (throwsExceptionAttributes.Any())
                    {
                        foreach (var genericType in throwsExceptionAttributes.GroupBy(a => a.GetTypeInfo(context).ConvertedType!, SymbolEqualityComparer.Default))
                        {
                            if (genericType.Key is not INamedTypeSymbol symbol)
                                continue;

                            var fullyQualifiedMetadataName = symbol.TypeArguments.Single().GetFullyQualifiedMetadataName();
                            attributeKind = nameof(Assert.ThrowsException);
                            methodSignature = $"{methodName}_{fullyQualifiedMetadataName.Replace("global::", string.Empty).Replace('.', '_')}({paramList})";

                            sb.AppendLine();

                            bool hasNullability = context.Compilation.GetSemanticModel(methodDef.SyntaxTree, true).GetTypeInfo(throwsExceptionAttributes.First()).ConvertedNullability.FlowState != NullableFlowState.None;

                            if (!hasNullability)
                                sb.AppendLine("#nullable disable");

                            sb.AppendLine("\t\t/// <summary>");
                            sb.AppendLine($"\t\t/// Test method for <seealso cref=\"{methodDef.GetDeclaredSymbol(context)!.GetFullyQualifiedMetadataName()}\"/> using <seealso cref=\"global::Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull\"/>.");
                            sb.AppendLine("\t\t/// </summary>");
                            sb.AppendLine($"\t\t[TestMethod]");

                            foreach (var throwsExceptionAttr in genericType)
                            {
                                throwsExceptionAttr.GetArgumentDisplay(out var argList, out var argDisplay);
                                sb.AppendLine($"\t\t[DataRow({argList}{(!string.IsNullOrWhiteSpace(argList) ? ", " : string.Empty)}DisplayName = \"{methodDef.GetDisplayName(argDisplay, $" throws {symbol.TypeArguments.Single().MetadataName}")}\")]");
                            }

                            sb.AppendLine($"\t\t{methodDef.GetTestMethodModifiers()} {methodSignature.Replace("%AttributeKind%", attributeKind)}");
                            sb.AppendLine("\t\t{");

                            if (methodDef.IsAsync())
                                sb.AppendLine($"\t\t\tawait Assert.ThrowsExceptionAsync<{fullyQualifiedMetadataName}>(() => {methodDef.InvokeMethod(context, false, true)});");
                            else
                                sb.AppendLine($"\t\t\tAssert.ThrowsException<{fullyQualifiedMetadataName}>(() => {methodDef.InvokeMethod(context, false, true)});");

                            sb.AppendLine("\t\t}");

                            if (!hasNullability)
                                sb.AppendLine("#nullable enable");
                        }
                    }
                }

                sb.AppendLine("\t}");
            }

            sb.Append("}");

            context.AddSource($"GeneratedInlineTests.cs", sb.ToString());
        }

        /// <inheritdoc/>
        public void Initialize(GeneratorInitializationContext context)
        {
#if FALSE && DEBUG
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Launch();
            }
#endif
        }
    }
}