using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Direction = MVVMTest2.Model.DataItem.TranslationDirection;

namespace MVVMTest2.Model
{
    /// <summary>
    /// Concrete IDataService, loading DataItems
    /// from files.
    /// </summary>
    public class LocalFileDataService : IDataService
    {

        public LocalFileDataService()
        {
            Converter = new WordConverter();
        }

        private async Task LoadFromFile(string path)
        {
            Converter.Clear();
            using (var sr = new StreamReader(path, Encoding.Default))
            {
                Converter.Append(await sr.ReadToEndAsync());
            }
        }

        private async Task AppendFromFile(string path)
        {
            using (var sr = new StreamReader(path, Encoding.Default))
            {
                Converter.Append(Environment.NewLine);
                Converter.Append(await sr.ReadToEndAsync());
            }
        }

        #region IDataService Members

        public WordConverter Converter { get; set; }

        public async Task<DataItem> GetData(string path, Direction dir)
        {
            await LoadFromFile(path);
            var words = Converter.GetWordsDictionary(dir);

            return new DataItem(words, dir);
        }

        public async Task<DataItem> GetData(IList<string> paths, Direction dir, Action<int, int> OnProgress)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                OnProgress?.Invoke(i + 1, paths.Count);
                await AppendFromFile(paths[i]);
            }
            var words = Converter.GetWordsDictionary(dir);

            return new DataItem(words, dir);
        }

        #endregion
    }
}