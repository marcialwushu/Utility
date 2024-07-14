namespace ImageToBase64Converter
{
    class Program
    {
        static void Main(string[] args)
        {
            string imagesDirectory = @"C:\Users\marci\Pictures"; // Altere para o caminho do seu diretório de imagens
            string outputDirectory = @"C:\Users\marci\Pictures\TXT"; // Altere para o caminho onde deseja salvar os arquivos txt

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var imageFiles = Directory.GetFiles(imagesDirectory, "*.*", SearchOption.AllDirectories)
                                      .Where(s => s.EndsWith(".jpg") || s.EndsWith(".jpeg") || s.EndsWith(".png") || s.EndsWith(".bmp") || s.EndsWith(".gif"));

            foreach (var imageFile in imageFiles)
            {
                try
                {
                    string base64String = ConvertImageToBase64(imageFile);
                    string outputFileName = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(imageFile) + ".txt");

                    File.WriteAllText(outputFileName, base64String);

                    Console.WriteLine($"Converted and saved: {imageFile}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to convert: {imageFile}. Error: {ex.Message}");
                }
            }

            Console.WriteLine("Conversion completed.");

        }

        static string ConvertImageToBase64(string imagePath)
        {
            byte[] imageBytes = File.ReadAllBytes(imagePath);
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }
    }
}