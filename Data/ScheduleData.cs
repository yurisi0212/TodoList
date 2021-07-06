using System;

namespace TodoList.Data {
    class ScheduleData {

        /// <summary>
        /// ファイルに保存されている行数
        /// </summary>
        public int index { get; set; }

        /// <summary>
        /// Todoのタイトル
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// Todoの時間
        /// </summary>
        public DateTime dateTime { get; set; }

        /// <summary>
        /// Todoの内容
        /// </summary>
        public string contents { get; set; }

        /// <summary>
        /// Todoの状況
        /// </summary>
        public bool complete { get; set; }
    }
}
