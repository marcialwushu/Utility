using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace MethodCallMapper
{
    class Program
    {
        static void Main(string[] args)
        {
            // Tipo da classe que queremos analisar
            var targetType = typeof(SampleClass);

            // Obtemos todos os métodos públicos de instância definidos diretamente na classe alvo
            var methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var method in methods)
            {
                Console.WriteLine($"Método: {method.Name}");
                var methodCalls = GetMethodCalls(targetType, method);
                foreach (var call in methodCalls)
                {
                    Console.WriteLine($"  Chama: {call}");
                }
            }
        }

        /// <summary>
        /// Obtém as chamadas de método dentro de um método específico da classe alvo.
        /// </summary>
        /// <param name="targetType">O tipo da classe que contém o método.</param>
        /// <param name="method">O método a ser analisado.</param>
        /// <returns>Uma lista de nomes de métodos chamados.</returns>
        static List<string> GetMethodCalls(Type targetType, MethodInfo method)
        {
            List<string> methodCalls = new List<string>();

            // Caminho para o assembly que contém a classe alvo
            var assemblyPath = targetType.Assembly.Location;

            // Carrega o assembly usando Mono.Cecil
            var assembly = AssemblyDefinition.ReadAssembly(assemblyPath);
            var typeDefinition = assembly.MainModule.GetType(targetType.FullName);

            // Encontra a definição do método que corresponde ao método fornecido
            var methodDefinition = typeDefinition.Methods.First(m => m.Name == method.Name);

            // Percorre as instruções do corpo do método
            foreach (var instruction in methodDefinition.Body.Instructions)
            {
                // Verifica se a instrução é uma chamada de método (chamada normal ou virtual)
                if (instruction.OpCode == Mono.Cecil.Cil.OpCodes.Call || instruction.OpCode == Mono.Cecil.Cil.OpCodes.Callvirt)
                {
                    // Obtém a referência do método chamado
                    var methodReference = (MethodReference)instruction.Operand;
                    methodCalls.Add(methodReference.Name);
                }
            }

            return methodCalls;
        }
    
}
}