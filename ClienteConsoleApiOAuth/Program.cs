// See https://aka.ms/new-console-template for more information
using ClienteConsoleApiOAuth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

Console.WriteLine("Api Client OAuth");
Console.WriteLine("Introduzca Apellido");
string apellido = Console.ReadLine();
Console.WriteLine("Password");
string password = Console.ReadLine();
string respuesta = await GetTokenAsync(apellido, password);
Console.WriteLine(respuesta);

Console.WriteLine("Petición a Empleado");
string data = await FindEmpleadoAsync(respuesta);
Console.WriteLine(data);

Console.WriteLine("---------------------------");
Console.WriteLine("Fin de programa");

static async Task<string> GetTokenAsync(string user, string pass)
{
    string urlApi = "https://localhost:7249/";
    LoginModel model = new LoginModel 
    { UserName = user, Password = pass };
    using (HttpClient client = new HttpClient())
    {
        client.BaseAddress = new Uri(urlApi);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add
            (new MediaTypeWithQualityHeaderValue("application/json"));
        //CONVERTIMOS NUESTRO MODEL A JSON
        string jsonModel = JsonConvert.SerializeObject(model);
        StringContent content =
            new StringContent
            (jsonModel, Encoding.UTF8, "application/json");
        string request = "api/auth/login";
        HttpResponseMessage response = await
            client.PostAsync(request, content);
        if (response.IsSuccessStatusCode)
        {
            string data = await
                response.Content.ReadAsStringAsync();
            JObject keys = JObject.Parse(data);
            string token = keys.GetValue("response").ToString();
            return token;
        }
        else
        {
            return "Petición incorrecta: " + response.StatusCode;
        }
    }
}

//CREAMOS UN METODO PARA REALIZAR UNA PETICION CON EL TOKEN
//EN EL HEADER MEDIANTE Authorization
static async Task<string> FindEmpleadoAsync(string token)
{
    string urlApi = "https://localhost:7249/";
    using (HttpClient client = new HttpClient())
    {
        string request = "api/empleados/7839";
        client.BaseAddress = new Uri(urlApi);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add
            (new MediaTypeWithQualityHeaderValue("application/json"));
        //DEBEMOS AÑADIR A NUESTRO HEADER Authorization
        client.DefaultRequestHeaders.Add
            ("Authorization", "bearer " + token);
        HttpResponseMessage response =
            await client.GetAsync(request);
        if (response.IsSuccessStatusCode)
        {
            string data = await response.Content.ReadAsStringAsync();
            return data;
        }
        else
        {
            return "Error de algo: " + response.StatusCode;
        }
    }
}
