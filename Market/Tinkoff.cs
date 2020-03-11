using Newtonsoft.Json;
using PSVClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Market
{

    public partial class ThisAddIn
    {

        // универсальная процедура для запроса данных из api Тинькофф Инвестиции
        // результат в формате json
        static async Task<T> GetAsync<T>(string Token, string Uri)
        {
            // инициализация HTTP
            var cl = new HttpClient();
            cl.BaseAddress = new Uri(Uri);

            int _TimeoutSec = 90;
            cl.Timeout = new TimeSpan(0, 0, _TimeoutSec);

            string _ContentType = "application/json";
            cl.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_ContentType));

            var _CredentialBase64 = Token;
            cl.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer {0}", _CredentialBase64));


            // GET-запрос
            T result = default(T);

            HttpResponseMessage response = await cl.GetAsync(Uri);

            if (response.IsSuccessStatusCode)
            {
                var jsonAsString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<T>(jsonAsString);
            }
            return result;
        }

    }

}
