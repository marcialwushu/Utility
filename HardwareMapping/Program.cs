using System.Diagnostics;
using System.Text.Json;

namespace HardwareMapping
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Script PowerShell embutido
            string powerShellScript = @"
                        function Get-HardwareInfo {
                        $cpu = Get-WmiObject Win32_Processor | Select-Object Name, Manufacturer, Version, ProcessorId, NumberOfCores, NumberOfLogicalProcessors, MaxClockSpeed
                        $ram = Get-WmiObject Win32_PhysicalMemory | Select-Object Manufacturer, PartNumber, SerialNumber, Capacity, Speed, FormFactor
                        $disk = Get-WmiObject Win32_DiskDrive | Select-Object Model, Manufacturer, SerialNumber, Size, InterfaceType

                        $hardwareInfo = @()

                        $cpu | ForEach-Object {
                            $hardwareInfo += [PSCustomObject]@{
                                Type = 'CPU'
                                Manufacturer = $_.Manufacturer
                                Model = $_.Name
                                SerialNumber = $_.ProcessorId
                                Version = $_.Version
                                PartNumber = ''
                                Description = 'CPU'
                                Properties = @{
                                    Cores = $_.NumberOfCores
                                    Threads = $_.NumberOfLogicalProcessors
                                    Architecture = 'x86_64'
                                    BaseClock = $_.MaxClockSpeed
                                    MaxClock = $_.MaxClockSpeed
                                    Cache = ''
                                }
                                ExternalReferences = @()
                            }
                        }

                        $ram | ForEach-Object {
                            $hardwareInfo += [PSCustomObject]@{
                                Type = 'RAM'
                                Manufacturer = $_.Manufacturer
                                Model = $_.PartNumber
                                SerialNumber = $_.SerialNumber
                                Version = ''
                                PartNumber = $_.PartNumber
                                Description = 'Memory'
                                Properties = @{
                                    Capacity = [math]::round($_.Capacity / 1GB, 2)
                                    Type = 'DDR4'
                                    Speed = $_.Speed
                                    FormFactor = $_.FormFactor
                                }
                                ExternalReferences = @()
                            }
                        }

                        $disk | ForEach-Object {
                            $hardwareInfo += [PSCustomObject]@{
                                Type = 'Disk'
                                Manufacturer = $_.Manufacturer
                                Model = $_.Model
                                SerialNumber = $_.SerialNumber
                                Version = ''
                                PartNumber = ''
                                Description = 'Storage'
                                Properties = @{
                                    Capacity = [math]::round($_.Size / 1GB, 2)
                                    Type = 'SSD'
                                    Interface = $_.InterfaceType
                                }
                                ExternalReferences = @()
                            }
                        }

                        $hardwareInfo | ConvertTo-Json
                    }

                    Get-HardwareInfo
                        ";

            string powerShellOutput = await ExecutePowerShellScript(powerShellScript);

            if (!string.IsNullOrWhiteSpace(powerShellOutput))
            {
                // Gera o arquivo JSON com as especificações do hardware
                var hardwareComponents = JsonSerializer.Deserialize<List<HardwareComponent>>(powerShellOutput);
                foreach (var component in hardwareComponents)
                {
                    if (component.Type == "CPU")
                    {
                        component.Properties["BaseClock"] = $"{component.Properties["BaseClock"]} MHz";
                        component.Properties["MaxClock"] = $"{component.Properties["MaxClock"]} MHz";
                    }
                    if (component.Type == "RAM" || component.Type == "Disk")
                    {
                        component.Properties["Capacity"] = $"{component.Properties["Capacity"]} GB";
                        if (component.Type == "RAM")
                        {
                            component.Properties["Speed"] = $"{component.Properties["Speed"]} MHz";
                        }
                    }
                }
                string jsonOutput = JsonSerializer.Serialize(hardwareComponents, new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText("HardwareInfo.json", jsonOutput);

                Console.WriteLine("Arquivo HardwareInfo.json gerado com sucesso!");
            }
            else
            {
                Console.WriteLine("Falha ao obter informações de hardware.");
            }
        }

        static async Task<string> ExecutePowerShellScript(string script)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-ExecutionPolicy Bypass -Command \"{script}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();

                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Console.WriteLine("Erro ao executar o script PowerShell:");
                    Console.WriteLine(error);
                }

                return output;
            }
        }

        public class HardwareComponent
        {
            public string Type { get; set; }
            public string Manufacturer { get; set; }
            public string Model { get; set; }
            public string SerialNumber { get; set; }
            public string Version { get; set; }
            public string PartNumber { get; set; }
            public string Description { get; set; }
            public Dictionary<string, object> Properties { get; set; }
            public List<ExternalReference> ExternalReferences { get; set; }
        }

        public class ExternalReference
        {
            public string Type { get; set; }
            public string Url { get; set; }
        }

    }
}
