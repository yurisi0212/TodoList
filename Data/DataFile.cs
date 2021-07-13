using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TodoList.Data {
    class DataFile {

        /// <summary>
        /// フォルダへのパス
        /// </summary>
        private string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\yurisi\TodoList";

        /// <summary>
        /// csvファイルへのパス
        /// </summary>
        private string csvPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\yurisi\TodoList\todo.data";

        /// <summary>
        /// ScheduleDataのオブジェクトを保持するList
        /// </summary>
        private List<ScheduleData> data = new List<ScheduleData>();

        public bool fileExists() {
            return File.Exists(csvPath);
        }

        public void makeFile() {
            Directory.CreateDirectory(path);
            File.Create(csvPath).Close();
        }

        public void deleteFile() {
            File.Delete(csvPath);
        }

        public void Parse() {
            data = new List<ScheduleData>();
            using (var parser = new TextFieldParser(csvPath)) {
                parser.Delimiters = new string[] { "," };
                int indexCount = 0;
                while (!parser.EndOfData) {
                    var fields = parser.ReadFields();
                    int count = 0;
                    ScheduleData scheduleData = new ScheduleData();
                    foreach (var s in fields) {
                        if (count == 0) {
                            scheduleData.title = s;
                            count++;
                            continue;
                        } else if (count == 1) {
                            scheduleData.dateTime = DateTime.Parse(s);
                            count++;
                            continue;
                        } else if (count == 2) {
                            scheduleData.contents = s;
                            count++;
                            continue;
                        }
                        scheduleData.complete = Boolean.Parse(s);
                    }
                    scheduleData.index = indexCount;
                    data.Add(scheduleData);
                    
                    indexCount++;
                }
            }
        }

        public void addData(ScheduleData sd) {
            Encoding enc = Encoding.GetEncoding("utf-8");
            StreamWriter writer = new StreamWriter(csvPath, true, enc);
            writer.WriteLine(sd.title + "," + sd.dateTime.ToString() + "," + sd.contents + "," + sd.complete.ToString());
            writer.Close();
            Parse();
        }

        public void deleteData(int index) {
            Parse();
            int count = 0;
            string line;
            if (!File.Exists(csvPath + ".data")) File.Create(csvPath + ".data").Close();
            Encoding enc = Encoding.GetEncoding("utf-8");
            StreamWriter writer = new StreamWriter(csvPath + ".data", true, enc);
            StreamReader reader = new StreamReader(csvPath);
            using (reader) {
                using (writer) {
                    while (reader.Peek() >= 0) {
                        line = reader.ReadLine();
                        if (index == count) {
                            count++;
                            continue;
                        }
                        writer.WriteLine(line);
                        count++;
                    }
                }
            }
            File.Delete(csvPath);
            File.Move(csvPath + ".data", csvPath);
            Parse();
        }

        public void changeData(ScheduleData sd) {
            Parse();
            int count = 0;
            string line;
            if (!File.Exists(csvPath + ".data")) File.Create(csvPath + ".data").Close();
            Encoding enc = Encoding.GetEncoding("utf-8");
            StreamWriter writer = new StreamWriter(csvPath + ".data", true, enc);
            StreamReader reader = new StreamReader(csvPath);
            using (reader) {
                using (writer) {
                    while (reader.Peek() >= 0) {
                        line = reader.ReadLine();
                        if (sd.index == count) {
                            writer.WriteLine(sd.title + "," + sd.dateTime.ToString() + "," + sd.contents + "," + sd.complete.ToString());
                            count++;
                            continue;
                        }
                        writer.WriteLine(line);
                        count++;
                    }
                }
            }
            File.Delete(csvPath);
            File.Move(csvPath + ".data", csvPath);
            Parse();
        }


        public List<ScheduleData> getDataByDateTime(string dateTime,bool complete) {
            DateTime dt = DateTime.Parse(dateTime);
            List<ScheduleData> datetimeData = new List<ScheduleData>();
            if (complete) {
                foreach (ScheduleData sd in data) {
                    if (sd.dateTime.ToString("yyyy/MM/dd") == dt.ToString("yyyy/MM/dd")) datetimeData.Add(sd);
                }
            } else {
                foreach (ScheduleData sd in data) {
                    if (sd.dateTime.ToString("yyyy/MM/dd") == dt.ToString("yyyy/MM/dd")) {
                        if (sd.complete == false) { 
                        datetimeData.Add(sd);
                        }
                    }
                }
            }
            
            datetimeData.Sort((a, b) => a.dateTime.CompareTo(b.dateTime));
            return datetimeData;
        }


    }
}
