using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using TodoList.Data;

namespace TodoList {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {

        /// <summary>
        /// csvの管理クラス
        /// </summary>
        private DataFile dataFile = new DataFile();

        /// <summary>
        /// コンボで選択されている値
        /// </summary>
        private int comboSelect = -1;

        /// <summary>
        /// コンボボックスのアイテムを格納するList
        /// </summary>
        private List<ComboBoxItem> title = new List<ComboBoxItem>();

        /// <summary>
        /// スケジュールデータのオブジェクトを保存するList
        /// </summary>
        private List<ScheduleData> viewItem = new List<ScheduleData>();


        public MainWindow() {
            InitializeComponent();
        }

        private void Menu_Loaded(object sender, RoutedEventArgs e) {
            if (!dataFile.fileExists()) {
                label.Content = "初回起動ですのでフォルダを作成しました";
                dataFile.makeFile();
            }
            dataFile.Parse();
            combo.Items.Add(new ComboBoxItem().Name = "test");
            date_picker.SelectedDate = DateTime.Today;
        }

        public void add_sd(object sender, RoutedEventArgs e) {
            newItem();
        }

        public void delete_sd(object sender, RoutedEventArgs e) {
            deleteItem();
        }

        public void exit_click(object sender, RoutedEventArgs e) {
            Environment.Exit(0);
        }

        private void New_button(object sender, RoutedEventArgs e) {
            newItem();
        }

        private void Date_Change_todo(object sender, SelectionChangedEventArgs e) {
            changeViewItem(date_picker.SelectedDate.ToString(), check.IsChecked.Value);
        }

        public void Combo_SelectionChanged(object sender, EventArgs e) {
            changeSelect();
        }



        private void Complete_button_name_Click(object sender, RoutedEventArgs e) {
            int select = combo.SelectedIndex;
            if (select == -1) {
                label.Content = "予定が存在しません";
                return;
            }
            bool vi = viewItem[select].complete ? viewItem[select].complete = false : viewItem[select].complete = true;
            string msg = viewItem[select].complete ? "完了しました" : "完了をキャンセルしました";
            label.Content = "[" + viewItem[select].title + "]を" + msg;
            dataFile.changeData(viewItem[select]);
            dataFile.Parse();
            changeViewItem(date_picker.SelectedDate.ToString(), check.IsChecked.Value);

            if (Boolean.Parse(check.IsChecked.ToString())) {
                combo.SelectedIndex = select;
                changeSelect();
                return;
            }
            resetForm();
        }

        private void Lost_Focus_textBox_title(object sender, EventArgs e) {
            if (combo.SelectedItem == null) {
                textBox_title.Text = "タイトル";
                label.Content = "予定を選択または新規作成してください";
                return;
            }

        }

        private void DateTime_Change_contents(object sender, RoutedEventArgs e) {
            if (datetimePicker.Value == null) return;
            if (combo.SelectedItem == null) {
                datetimePicker.Value = null;
                label.Content = "予定を選択または新規作成してください";
                return;
            }

            changeViewItem(date_picker.SelectedDate.ToString(), check.IsChecked.Value);
            fixesIndex();
        }

        private void Lost_Focus_textBox(object sender, EventArgs e) {
            if (combo.SelectedItem == null) {
                textBox.Text = "詳細";
                label.Content = "予定を選択または新規作成してください";
                return;
            }

        }

        private void delete_button(object sender, RoutedEventArgs e) {
            deleteItem();
        }

        private void save_button(object sender, RoutedEventArgs e) {
            if (combo.SelectedIndex == -1) return;
            int select = combo.SelectedIndex;
            if (viewItem[select].title.ToString() != textBox_title.Text.ToString()) {
                title[select].Content = textBox_title.Text.ToString();
                viewItem[select].title = textBox_title.Text.ToString();
            }
            if (viewItem[select].contents.ToString() != textBox.Text.ToString()) {
                string str = textBox.Text.ToString().Replace("\r\n", "%esc%").Replace("\n", "%esc%").Replace("\r", "%esc%");
                viewItem[select].contents = str;
            }
            if (viewItem[select].dateTime != datetimePicker.Value) {
                viewItem[select].dateTime = DateTime.Parse(datetimePicker.Value.Value.ToString());
            }
            label.Content = "[" + viewItem[select].title + "]の変更を保存しました！";
            dataFile.changeData(viewItem[select]);
            dataFile.Parse();
            changeViewItem(date_picker.SelectedDate.ToString(), check.IsChecked.Value);
            resetForm();
            comboSelect = select;
            combo.SelectedIndex = select;
            changeSelect();

        }

        private void changeViewItem(string date, bool complete) {
            combo.Items.Clear();
            title.Clear();
            viewItem.Clear();
            List<ScheduleData> data = dataFile.getDataByDateTime(date, complete);
            if (data == null) return;
            foreach (ScheduleData sd in data) {
                ComboBoxItem comboItem = new ComboBoxItem();
                comboItem.Content = sd.title;
                combo.Items.Add(comboItem);
                title.Add(comboItem);
                viewItem.Add(sd);
            }
        }

        private void resetForm() {
            textBox_title.Text = "タイトル";
            datetimePicker.Value = null;
            textBox.Text = "詳細";
            label_complete.Content = "";
        }

        private void newItem() {
            ComboBoxItem ComboBoxItem = new ComboBoxItem();
            ComboBoxItem.Content = "新規作成";
            combo.Items.Add(ComboBoxItem);
            title.Add(ComboBoxItem);
            ScheduleData sd = new ScheduleData();
            sd.title = "新規作成";
            sd.dateTime = DateTime.Now;
            sd.contents = "詳細";
            sd.complete = false;
            viewItem.Add(sd);
            dataFile.addData(sd);
            changeViewItem(date_picker.SelectedDate.ToString(), check.IsChecked.Value);
            label.Content = "[" + sd.title + "]を作成しました！";
            resetForm();
        }

        private void deleteItem() {
            if (combo.SelectedIndex == -1) {
                label.Content = "予定が存在しません";
                return;
            }
            label.Content = "[" + viewItem[combo.SelectedIndex].title + "]を削除しました";
            title.RemoveAt(combo.SelectedIndex);
            dataFile.deleteData(viewItem[combo.SelectedIndex].index);
            viewItem.RemoveAt(combo.SelectedIndex);
            combo.Items.RemoveAt(combo.SelectedIndex);
            dataFile.Parse();
            changeViewItem(date_picker.SelectedDate.ToString(), check.IsChecked.Value);
            textBox_title.Text = "タイトル";
            textBox.Text = "詳細";
            datetimePicker.Value = null;
        }

        private void fixesIndex() {
            if (combo.SelectedIndex == -1) combo.SelectedIndex = comboSelect;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            MessageBoxResult result = MessageBox.Show("データを削除してもよろしいでしょうか\n削除されたデータは元には戻りません", "todoList", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes) {
                dataFile.deleteFile();
                dataFile.makeFile();
                dataFile.Parse();
                changeViewItem(date_picker.SelectedDate.ToString(), check.IsChecked.Value);
                resetForm();
            }
        }

        private void changeSelect() {
            comboSelect = combo.SelectedIndex;
            if (combo.SelectedIndex == -1) {
                combo.SelectedIndex = comboSelect;
                return;
            }
            ScheduleData vi = viewItem[combo.SelectedIndex];
            textBox_title.Text = vi.title;
            datetimePicker.Value = vi.dateTime;
            string str = vi.contents.Replace("%esc%", "\n");
            textBox.Text = str;
            string button_label = vi.complete ? "キャンセル" : "完了！";
            label_complete.Content = vi.complete ? "完了済み！" : "未完了";
            complete_button_name.Content = button_label;
            combo.SelectedIndex = comboSelect;
            fixesIndex();
        }

        private void check_Click(object sender, RoutedEventArgs e) {
            combo.SelectedIndex = -1;
            resetForm();
            changeViewItem(date_picker.SelectedDate.ToString(), check.IsChecked.Value);

        }
    }
}
