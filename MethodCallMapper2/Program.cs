using MethodCallMapper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection;

namespace MethodCallMapper2
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourceCodeDirectory = @"C:\Users\marci\source\repos\ImageToBase64Converter\MethodCallMapper";

            var classFiles = Directory.GetFiles(sourceCodeDirectory, "*.cs", SearchOption.AllDirectories);
            var syntaxTrees = classFiles.Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file))).ToList();

            var compilation = CSharpCompilation.Create("MethodCallMapper")
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddSyntaxTrees(syntaxTrees);

            var targetType = typeof(SampleClass);

            // Obter todos os métodos da classe alvo
            var methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var method in methods)
            {
                Console.WriteLine($"Método: {method.Name}");
                var methodCalls = GetMethodCalls(compilation, targetType, method);
                foreach (var call in methodCalls)
                {
                    Console.WriteLine($"  Chama: {call}");
                }
            }
        }

        static List<string> GetMethodCalls(CSharpCompilation compilation, Type targetType, MethodInfo method)
        {
            var methodCalls = new List<string>();
            var syntaxTree = compilation.SyntaxTrees.First(tree =>
                tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Any(cd => cd.Identifier.Text == targetType.Name));

            var methodDeclaration = syntaxTree.GetRoot().DescendantNodes()
                .OfType<MethodDeclarationSyntax>().First(md => md.Identifier.Text == method.Name);

            var invocationExpressions = methodDeclaration.DescendantNodes().OfType<InvocationExpressionSyntax>();
            foreach (var invocation in invocationExpressions)
            {
                var expression = invocation.Expression as IdentifierNameSyntax;
                if (expression != null)
                {
                    methodCalls.Add(expression.Identifier.Text);
                }
                else
                {
                    var memberAccessExpression = invocation.Expression as MemberAccessExpressionSyntax;
                    if (memberAccessExpression != null)
                    {
                        var identifier = memberAccessExpression.Name as IdentifierNameSyntax;
                        if (identifier != null)
                        {
                            methodCalls.Add(identifier.Identifier.Text);
                        }
                    }
                }
            }

            var objectCreations = methodDeclaration.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
            foreach (var creation in objectCreations)
            {
                var className = creation.Type.ToString();
                var classMethods = GetClassMethods(compilation, className);
                methodCalls.AddRange(classMethods);
            }

            return methodCalls;
        }

        static List<string> GetClassMethods(CSharpCompilation compilation, string className)
        {
            var classMethods = new List<string>();

            var classSyntaxTree = compilation.SyntaxTrees.FirstOrDefault(tree =>
                tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Any(cd => cd.Identifier.Text == className));

            if (classSyntaxTree == null)
            {
                return classMethods;
            }

            var classDeclaration = classSyntaxTree.GetRoot().DescendantNodes()
                .OfType<ClassDeclarationSyntax>().First(cd => cd.Identifier.Text == className);

            var methods = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>();

            foreach (var method in methods)
            {
                var invocationExpressions = method.DescendantNodes().OfType<InvocationExpressionSyntax>();
                foreach (var invocation in invocationExpressions)
                {
                    var expression = invocation.Expression as IdentifierNameSyntax;
                    if (expression != null)
                    {
                        classMethods.Add(expression.Identifier.Text);
                    }
                    else
                    {
                        var memberAccessExpression = invocation.Expression as MemberAccessExpressionSyntax;
                        if (memberAccessExpression != null)
                        {
                            var identifier = memberAccessExpression.Name as IdentifierNameSyntax;
                            if (identifier != null)
                            {
                                classMethods.Add(identifier.Identifier.Text);
                            }
                        }
                    }
                }
            }

            return classMethods;
        }
    
}
}

