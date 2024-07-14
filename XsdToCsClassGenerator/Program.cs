using System.Diagnostics;

namespace XsdToCsClassGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string xsdDirectory;

            // Verifica se o diretório foi passado como argumento
            if (args.Length == 0)
            {
                Console.WriteLine("Por favor, forneça o caminho do diretório contendo os arquivos XSD.");
                Console.Write("Digite o caminho do diretório: ");
                xsdDirectory = Console.ReadLine();

                // Verifica se o usuário forneceu algum valor, se não, retorna.
                if (string.IsNullOrEmpty(xsdDirectory))
                {
                    Console.WriteLine("Nenhum caminho fornecido. O programa será encerrado.");
                    return;
                }
            }
            else
            {
                xsdDirectory = args[0];
            }

            // Verifica se o diretório existe
            if (!Directory.Exists(xsdDirectory))
            {
                Console.WriteLine($"O diretório {xsdDirectory} não existe.");
                return;
            }

            // Obtém todos os arquivos XSD do diretório
            var xsdFiles = Directory.GetFiles(xsdDirectory, "*.xsd");

            // Verifica se há arquivos XSD no diretório
            if (xsdFiles.Length == 0)
            {
                Console.WriteLine($"Nenhum arquivo XSD encontrado no diretório {xsdDirectory}.");
                return;
            }

            // Itera sobre cada arquivo XSD e chama xsd.exe para gerar as classes C#
            foreach (var xsdFile in xsdFiles)
            {
                Console.WriteLine($"Processando {xsdFile}...");
                GenerateCsClassFromXsd(xsdFile);
            }

            Console.WriteLine("Processamento concluído.");
        }

        /// <summary>
        /// Gera uma classe C# a partir de um arquivo XSD usando xsd.exe.
        /// </summary>
        /// <param name="xsdFile">O caminho do arquivo XSD.</param>
        static void GenerateCsClassFromXsd(string xsdFile)
        {
            // Caminho para o xsd.exe (ajuste conforme necessário)
            var xsdExePath = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\xsd.exe";

            // Verifica se xsd.exe existe
            if (!File.Exists(xsdExePath))
            {
                Console.WriteLine($"xsd.exe não encontrado no caminho {xsdExePath}.");
                return;
            }

            // Configura o processo para chamar xsd.exe
            var processInfo = new ProcessStartInfo
            {
                FileName = xsdExePath,
                Arguments = $"\"{xsdFile}\" /c",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Inicia o processo e captura a saída
            using (var process = Process.Start(processInfo))
            {
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                // Exibe a saída e erros (se houver)
                Console.WriteLine(output);
                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Erro ao processar {xsdFile}:\n{error}");
                }
            }
        }
    }
}
