using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Creator.ViewModel.Contract
{
    interface ISwitchToEditor
    {
        void LoadEditorView(QuestionsSet questionSet);
    }
}
