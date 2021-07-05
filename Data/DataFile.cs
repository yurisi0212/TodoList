using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace TodoList.Data {
    class DataFile {

        private string path = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\TodoList";
        private string csvPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\TodoList\todo.csv";

        private List<List<string>> data = new List<List<string>>();
        public bool folderExists () {
            return Directory.Exists(path);
        }

        public void makeFile() {
            Directory.CreateDirectory(path);
            File.Create(csvPath).Close();
        }

        public string getPath() {
            return path;
        }

        public void Parsor() {
            using (var parser = new TextFieldParser(path)) {
                parser.Delimiters = new string[] { "," };
                int count = 0;
                data.Add(new List<string>());
                while (!parser.EndOfData) {
                    var fields = parser.ReadFields();
                    foreach (var s in fields) {
                        data[count].Add(s);
                    }
                }
            }
        }

        public void addData(string title,DateTime date,string contents,bool complete) {
            Encoding enc = Encoding.GetEncoding("utf-8");
            StreamWriter writer = new StreamWriter(csvPath, true, enc);
            writer.WriteLine(title + "," + date.ToString() + "," + contents + "," + complete.ToString());
            writer.Close();
        }

        public void deleteData() {

        }

        public List<List<string>> getDataByDateTime(DateTime dateTime) {
            List<List<string>> datetimeData = new List<List<string>>();
            datetimeData.Add(new List<string>());
            for (int i = 0; i < data.Count; i++) {
                if(DateTime.ParseExact("yyyy/MM/dd", data[i][1], null).ToString() == dateTime.ToString("yyyy/MM/dd")) {
                    foreach(string addData in data[i]) {
                        datetimeData[i].Add(addData);
                    } 
               }

            }
            return datetimeData;
        }
    }
}
