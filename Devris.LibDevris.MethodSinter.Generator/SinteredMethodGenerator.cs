﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Devris.LibDevris.MethodSinter.Generator
{
    [Generator]
    public class SinteredMethodGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var sinteredMethodProvider =
                context.SyntaxProvider.ForAttributeWithMetadataName("Devris.LibDevris.MethodSinter.SinteredMethodAttribute",
                    predicate: static (s, _) => s is MethodDeclarationSyntax mds && mds.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)),
                    transform: static (gac, _) => new SinteredMethodData
                    {
                        AttributeIdentifier = (string)gac.Attributes.Single().ConstructorArguments.Single().Value,
                        Ancestor = new SinteredMethodData.AncestorData
                        {
                            Usings = ((MethodDeclarationSyntax)gac.TargetNode).FirstAncestorOrSelf<CompilationUnitSyntax>().Usings
                                .Select(u => u.ToString()).Where(s => string.IsNullOrWhiteSpace(s) == false)
                                .ToImmutableArray(),
                            NamespaceName = ((MethodDeclarationSyntax)gac.TargetNode).FirstAncestorOrSelf<NamespaceDeclarationSyntax>().Name.ToString(),
                            ClassName = ((MethodDeclarationSyntax)gac.TargetNode).FirstAncestorOrSelf<ClassDeclarationSyntax>().Identifier.Text,
                        },
                        Method = new SinteredMethodData.MethodData
                        {
                            Modifiers = ((MethodDeclarationSyntax)gac.TargetNode).Modifiers.ToString(),
                            MethodName = ((MethodDeclarationSyntax)gac.TargetNode).Identifier.Text,
                            Parameters = ((MethodDeclarationSyntax)gac.TargetNode).ParameterList.Parameters
                                .Select(p => new SinteredMethodData.MethodData.ParameterData
                                {
                                    Attributes = p.AttributeLists.ToString(),
                                    Modifier = p.Modifiers.ToString(),
                                    Type = p.Type.ToString(),
                                    ParameterName = p.Identifier.Text
                                }).ToImmutableArray(),
                        },
                    });

            var methodFragmentProvider =
                context.SyntaxProvider
                    .ForAttributeWithMetadataName("Devris.LibDevris.MethodSinter.MethodFragmentAttribute",
                        predicate: static (s, _) => s is MethodDeclarationSyntax,
                        transform: static (gac, _) => (
                            AttributeIdentifier: (string)gac.Attributes.Single().ConstructorArguments.Single().Value,
                            MethodName: ((MethodDeclarationSyntax)gac.TargetNode).Identifier.Text))
                    .Collect();

            var targetProvider = sinteredMethodProvider.Combine(methodFragmentProvider).Select((ps, _) =>
            {
                ps.Left.FragmentNames = ps.Right
                    .Where(t => t.AttributeIdentifier == ps.Left.AttributeIdentifier)
                    .Select(t => t.MethodName)
                    .ToImmutableArray();

                return ps.Left;
            });

            context.RegisterSourceOutput(targetProvider, GenerateSintered);
        }

        private void GenerateSintered(SourceProductionContext context, SinteredMethodData source)
        {
            var inputParameters = source.Method.Parameters
                .Select(p => p.Modifier switch
                    {
                        "params" or "" => p.ParameterName,
                        "ref readonly" => $"in {p.ParameterName}",
                        var mod => $"{mod} {p.ParameterName}"
                    });

            context.AddSource($"{source.Ancestor.ClassName}.{source.AttributeIdentifier}.g.cs",
$@"// <auto-generated />
{string.Join("\r\n", source.Ancestor.Usings)}

namespace {source.Ancestor.NamespaceName}
{{
    partial class {source.Ancestor.ClassName}
    {{
        {source.Method.Modifiers} void {source.Method.MethodName}({string.Join(", ", source.Method.Parameters)})
        {{
            {string.Join("\r\n            ", source.FragmentNames.Select(f => $"{f}({string.Join(",", inputParameters)});"))}
        }}
    }}
}}
");
        }

        struct SinteredMethodData
        {
            public string AttributeIdentifier { get; set; }

            public AncestorData Ancestor { get; set; }
            public struct AncestorData
            {
                public IEnumerable<string> Usings { get; set; }

                public string NamespaceName { get; set; }

                public string ClassName { get; set; }
            }

            public MethodData Method { get; set; }
            public struct MethodData
            {
                public string Modifiers { get; set; }

                public string MethodName { get; set; }

                public IEnumerable<ParameterData> Parameters { get; set; }
                public struct ParameterData
                {
                    public string Attributes { get; set; }

                    public string Modifier { get; set; }

                    public string Type { get; set; }

                    public string ParameterName { get; set; }

                    public override string ToString()
                    {
                        return string.Join(" ", new[] { Attributes, Modifier, Type, ParameterName }.Where(s => string.IsNullOrWhiteSpace(s) == false));
                    }
                }
            }

            public IEnumerable<string> FragmentNames { get; set; }
        }
    }
}

