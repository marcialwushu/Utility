using Newtonsoft.Json;
using RestSharp;
using System.Text;

namespace FirebaseConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Informações do Firebase
            string apiKey = "FIREBASE_ANDROID_API_KEY ";
            string appId = "FIREBASE_ANDROID_APP_ID ";
            string senderId = "FIREBASE_ANDROID_MESSAGING_SENDER_ID";
            string projectId = "FIREBASE_ANDROID_PROJECT_ID";

            string email = "user@example.com"; // Substitua pelo e-mail do usuário
            string password = "userpassword"; // Substitua pela senha do usuário

            await SignInWithPassword(apiKey, email, password);

            // Exemplo: Listar tokens de dispositivos registrados no Firebase Cloud Messaging (FCM)
            string url = $"https://fcm.googleapis.com/v2/projects/{projectId}/androidApps/{appId}/registrations";

            string uri = $"https://firebaseremoteconfig.googleapis.com/v1/projects/{projectId}/remoteConfig";

            string url2 = $"https://fcm.googleapis.com/v1/projects/{projectId}/registrations/{apiKey}";

            using var httpClient = new HttpClient();

            // Configuração do cabeçalho de autorização
            // Aqui você precisará usar o token de acesso, não a API Key diretamente, a menos que seja especificamente permitido pela API do Firebase
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            try
            {
                // Faça a solicitação GET
                HttpResponseMessage response = await httpClient.GetAsync(url2);

                if (response.IsSuccessStatusCode)
                {
                    // Leia a resposta como string
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    // Faz algo com a resposta, aqui estamos apenas imprimindo
                    Console.WriteLine("Resposta bem-sucedida:");
                    Console.WriteLine(jsonResponse);
                }
                else
                {
                    // Lida com resposta de erro
                    Console.WriteLine($"Erro na solicitação: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Captura e exibe exceções
                Console.WriteLine($"Exceção capturada: {ex.Message}");
            }
        }

        public static async Task SignInWithPassword(string apiKey, string email, string password)
        {
            string url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}";

            using (var client = new HttpClient())
            {
                var userData = new
                {
                    email = email,
                    password = password,
                    returnSecureToken = true
                };

                string json = JsonConvert.SerializeObject(userData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);
                string responseJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Autenticação bem-sucedida:");
                    Console.WriteLine(responseJson);
                }
                else
                {
                    Console.WriteLine("Erro na autenticação:");
                    Console.WriteLine(responseJson);
                }
            }
        }
    
}
}