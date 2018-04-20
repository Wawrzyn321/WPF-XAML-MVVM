using Creator.Model.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business;
using Microsoft.Win32;
using System.IO;

namespace Creator.Model.Implementation
{
    class ModalSaveLoadAgent : ISaveLoadAgent
    {
        public ModalSaveLoadAgent()
        {
            
        }

        public QuestionsSet Load()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();

            ISerializer serializer;
            switch (Path.GetExtension(ofd.SafeFileName))
            {
                case ".xml":
                    serializer = new XmlSerializer();
                    break;
                case ".json":
                    serializer = new JsonSerializer();
                    break;
                case ".dat":
                    serializer = new BinarySerializer();
                    break;
                case "":
                    throw new Exception("Null path");
                default:
                    throw new InvalidExtensionException("It ain't gonna work mate. Try with .xml, .json or .dat.");
            }

            SaveLoadManager mgr = new SaveLoadManager(serializer);
            return mgr.Load(ofd.FileName);
        }

        public void Save(QuestionsSet questionsSet)
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.Filter = "JSON (*.json)|*.json| XML (*.xml)|*.xml";

            ofd.ShowDialog();

            ISerializer serializer = null;

            switch (Path.GetExtension(ofd.SafeFileName))
            {
                case ".json":
                    serializer = new JsonSerializer();
                    break;
                case ".xml":
                    serializer = new XmlSerializer();
                    break;
                default:
                    throw new InvalidExtensionException("Only .json and .xml are supported!");
            }

            SaveLoadManager mgr = new SaveLoadManager(serializer);
            mgr.Save(questionsSet, ofd.FileName.Replace(ofd.SafeFileName, ""), ofd.SafeFileName);
        }
    }
}
