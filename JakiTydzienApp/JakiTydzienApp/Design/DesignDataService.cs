using System;
using JakiTydzienApp.Model;

namespace JakiTydzienApp.Design
{
    public class DesignDataService : IDataService
    {
        public void GetData(Action<Data, Exception> callback)
        {
            var item = new Data
            {
                tydzien = "PRZYKŁADOWY",
                details = "Tutaj są dodatkowe informacje",
                expires = 0,
                niedziela = "PRZYKŁADOWA",
            };
            callback(item, null);
        }
    }
}