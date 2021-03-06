﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Direction = EnglishTranslationQuiz.Model.DataItem.TranslationDirection;

namespace EnglishTranslationQuiz.Model
{
    /// <summary>
    /// Interface for data retreiver
    /// </summary>
    public interface IDataService
    {
        //load DataItem from single file
        Task<DataItem> GetData(string path, Direction dir);

        //combine DataItem from multiple files
        Task<DataItem> GetData(IList<string> paths, Direction dir, Action<int, int> OnProgress);
    }
}
