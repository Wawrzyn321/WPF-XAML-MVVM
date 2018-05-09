using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace JakiTydzienApp.Model
{
    /// <summary>
    /// Real-world data service. Pulls data from jakitydzien.pl and fills
    /// the Data item.
    /// </summary>
    public class DataService : IDataService
    {
        //invoke async method
        public void GetData(Action<Data, Exception> callback)
        {
             DoFoo(callback);
        }

        //get data and return to caller
        private async void DoFoo(Action<Data, Exception> callback)
        {
            await RetreiveDataFromWebsite().ContinueWith(task =>
            {
                try
                {
                    callback(task.Result, null);
                }
                catch (Exception err)
                {
                    callback(null, err);
                }
            });
        }

        //execute http request
        private async Task<Data> RetreiveDataFromWebsite()
        {
            const string request = "https://jakitydzien.pl/api/?type=json&api_key=cb01cde96fa2e2ddd437656a92c2da98&include_sunday_type=true";
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(request))
            using (HttpContent content = response.Content)
            {
                string str = await content.ReadAsStringAsync();
                return fastJSON.JSON.ToObject<Data>(str);
            }
        }

    }
}