using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Store.Client
{
    class Program
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        // URL de la API local:
        private static string BaseApiUrl = "https://localhost:7234/api/";

        static async Task Main(string[] args)
        {

            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Store API Client";

            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("**************** STORE API CLIENT ****************");
                Console.WriteLine("1. Loggearse");
                Console.WriteLine("2. Ver productos");
                Console.WriteLine("3. Salir");
                Console.Write("Seleccione una opcion: ");
                string option = Console.ReadLine() ?? string.Empty;

                switch (option)
                {
                    case "1":
                        await LoginAsync();
                        break;

                    case "2":
                        await GetProductsAsync();
                        break;

                    case "3":
                        exit = true;
                        Console.WriteLine("Saliendo...");
                        break;

                    default:
                        Console.WriteLine("Opcion invalida. Intente nuevamente.");
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nPresione una tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }

        static async Task LoginAsync()
        {
            Console.Clear();
            Console.WriteLine("**************** LOGIN ****************");
            Console.Write("Email: ");
            string email = Console.ReadLine() ?? string.Empty;

            Console.Write("Password: ");
            string password = ReadPassword();

            var loginData = new
            {
                Email = email,
                Password = password
            };

            try
            {
                var response = await _httpClient.PostAsync(
                    BaseApiUrl + "Auth/login",
                    new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json")
                );

                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en el cliente haciendo login: " + ex.Message);
            }
        }

        static async Task GetProductsAsync()
        {
            Console.Clear();
            Console.WriteLine("**************** LISTA DE PRODUCTOS ****************");

            try
            {
                Console.Write("Ingrese parte del nombre del producto (o deje vacio para ver todos): ");
                string? searchName = Console.ReadLine()?.Trim();

                // No se envia token para este endpoint
                _httpClient.DefaultRequestHeaders.Authorization = null;

                string endpoint = string.IsNullOrEmpty(searchName)
                    ? BaseApiUrl + "Product"
                    : BaseApiUrl + "Product?name=" + Uri.EscapeDataString(searchName);

                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al obtener productos: " + content);
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                dynamic products = JsonConvert.DeserializeObject(json);

                if (products?.data == null || products.data.Count == 0)
                {
                    Console.WriteLine("\nNo se encontraron productos.");
                    return;
                }

                Console.WriteLine("\nProductos disponibles:\n");

                foreach (var item in products.data)
                {
                    Console.WriteLine($"ID: {item.id}");
                    Console.WriteLine($"Nombre: {item.name}");
                    Console.WriteLine($"Stock: {item.stock}");
                    Console.WriteLine($"Precio: {item.unitPrice}");
                    Console.WriteLine(new string('-', 35));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en el cliente recuperando productos: " + ex.Message);
            }

            Console.WriteLine("\nPresione una tecla para volver al menu principal...");
            Console.ReadKey();
        }



        // Para ocultar la contrasena al escribir
        static string ReadPassword()
        {
            StringBuilder password = new StringBuilder();
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password.Append(key.KeyChar);
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password.ToString();
        }
    }
}
