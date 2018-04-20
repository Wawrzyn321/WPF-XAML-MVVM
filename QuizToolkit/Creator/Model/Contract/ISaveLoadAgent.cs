using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Creator.Model.Contract
{
    interface ISaveLoadAgent
    {
        void Save(QuestionsSet questionsSet);
        QuestionsSet Load();
    }
}
