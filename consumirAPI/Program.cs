using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MyApiConsumer
{
    class Nome
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; }
    }

    class Program
    {
        static readonly HttpClient client = new HttpClient();
        static readonly string baseUrl = "https://apiriiccardods.azurewebsites.net/api/Nomes";

        static async Task Main(string[] args)
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("Escolha uma opção:");
                Console.WriteLine("1. Listar Nomes");
                Console.WriteLine("2. Obter Nome por ID");
                Console.WriteLine("3. Criar Nome");
                Console.WriteLine("4. Atualizar Nome");
                Console.WriteLine("5. Deletar Nome");
                Console.WriteLine("6. Sair");
                Console.Write("\nOpção: ");


                switch (Console.ReadLine())
                {
                    case "1":
                        await ListarNomes();
                        break;
                    case "2":
                        await ObterNomePorId();
                        break;
                    case "3":
                        await CriarNome();
                        break;
                    case "4":
                        await AtualizarNome();
                        break;
                    case "5":
                        await DeletarNome();
                        break;
                    case "6":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Opção inválida. Tente novamente.");
                        break;
                }

                if (running)
                {
                    Console.WriteLine("\nPressione qualquer tecla para continuar..");
                    Console.ReadKey();
                }
            }
        }

        static async Task ListarNomes()
        {
            Console.WriteLine("Buscando lista de nomes...");
            try
            {
                HttpResponseMessage response = await client.GetAsync(baseUrl);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var nomes = JsonConvert.DeserializeObject<List<Nome>>(responseBody);

                Console.WriteLine("Nomes encontrados:");
                nomes.Reverse(); // Isso inverte a ordem dos elementos na lista
                foreach (var nome in nomes)
                {
                    Console.WriteLine($"Id: {nome.Id}, Nome Completo: {nome.NomeCompleto}");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nExceção ao fazer a requisição GET:");
                Console.WriteLine($"Mensagem: {e.Message}");
            }
        }

        static async Task ObterNomePorId()
        {
            Console.Write("Digite o ID do nome: ");
            string id = Console.ReadLine();
            Console.WriteLine($"Buscando nome com ID {id}...");

            try
            {
                HttpResponseMessage response = await client.GetAsync($"{baseUrl}/{id}");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var nome = JsonConvert.DeserializeObject<Nome>(responseBody);

                Console.WriteLine($"Nome encontrado: Id: {nome.Id}, Nome Completo: {nome.NomeCompleto}");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nExceção ao fazer a requisição GET por ID:");
                Console.WriteLine($"Mensagem: {e.Message}");
            }
        }

        static async Task CriarNome()
        {
            Console.Write("Digite o novo nome completo: ");
            string novoNomeCompleto = Console.ReadLine();
            var data = new StringContent(JsonConvert.SerializeObject(new { NomeCompleto = novoNomeCompleto }), Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(baseUrl, data);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Nome criado com sucesso!");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nExceção ao fazer a requisição POST:");
                Console.WriteLine($"Mensagem: {e.Message}");
            }
        }

        static async Task AtualizarNome()
        {
            Console.Write("Digite o ID do nome para atualizar: ");
            string id = Console.ReadLine();
            Console.Write("Digite o novo nome completo: ");
            string novoNomeCompleto = Console.ReadLine();

            // Observe que estamos incluindo o "id" no corpo da requisição JSON,
            // como mostrado na sua captura de tela do Swagger.
            var nomeObject = new { id = int.Parse(id), nomeCompleto = novoNomeCompleto };
            string json = JsonConvert.SerializeObject(nomeObject);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PutAsync($"{baseUrl}/{id}", data);
                if (!response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Erro ao atualizar nome: {response.StatusCode}");
                    Console.WriteLine($"Detalhes do erro: {responseContent}");
                }
                else
                {
                    Console.WriteLine("Nome atualizado com sucesso!");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nExceção ao fazer a requisição PUT:");
                Console.WriteLine($"Mensagem: {e.Message}");
            }
        }


        static async Task DeletarNome()
        {
            Console.Write("Digite o ID do nome para deletar: ");
            string id = Console.ReadLine();

            try
            {
                HttpResponseMessage response = await client.DeleteAsync($"{baseUrl}/{id}");
                response.EnsureSuccessStatusCode();
                Console.WriteLine("Nome deletado com sucesso!");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nExceção ao fazer a requisição DELETE:");
                Console.WriteLine($"Mensagem: {e.Message}");
            }
        }
    }
}
