using System;
using System.Collections.Generic;
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
        private List<ComboBoxItem> title =  new List<ComboBoxItem>();

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
            combo.Items.Add(new ComboBoxItem().Name="test");
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
            changeViewItem(date_picker.SelectedDate.ToString());
        }

        public void Combo_SelectionChanged(object sender, EventArgs e) {
            comboSelect = combo.SelectedIndex;
            if (combo.SelectedIndex == -1) {
                combo.SelectedIndex = comboSelect;
                return;
            }
            ScheduleData vi = viewItem[combo.SelectedIndex];
            textBox_title.Text = vi.title;
            datetimePicker.Value = vi.dateTime;
            textBox.Text = vi.contents;
            string button_label = vi.complete ? "キャンセル" : "完了！";
            label_complete.Content = vi.complete ? "完了済み！" : "未完了";
            complete_button_name.Content = button_label;
            combo.SelectedIndex = comboSelect;
            fixesIndex();

        }

        

        private void Complete_button_name_Click(object sender, RoutedEventArgs e) {
            if (combo.SelectedIndex == -1) {
                label.Content = "予定が存在しません";
                return;
            }
            bool vi = viewItem[combo.SelectedIndex].complete ? viewItem[combo.SelectedIndex].complete = false : viewItem[combo.SelectedIndex].complete = true;
            string msg = viewItem[combo.SelectedIndex].complete ? "完了しました" : "完了をキャンセルしました";
            label.Content = "["+viewItem[combo.SelectedIndex] .title+"]を"+ msg;
            dataFile.changeData(viewItem[combo.SelectedIndex]);
            dataFile.Parse();
            changeViewItem(date_picker.SelectedDate.ToString());
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
                      
            changeViewItem(date_picker.SelectedDate.ToString());
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
            if (viewItem[combo.SelectedIndex].title.ToString() != textBox_title.Text.ToString()) {
                title[combo.SelectedIndex].Content = textBox_title.Text.ToString();
                viewItem[combo.SelectedIndex].title = textBox_title.Text.ToString();
            }
            if (viewItem[combo.SelectedIndex].contents.ToString() != textBox.Text.ToString()) {
                viewItem[combo.SelectedIndex].contents = textBox.Text.ToString();
            }
            if (viewItem[combo.SelectedIndex].dateTime != datetimePicker.Value) {
                viewItem[combo.SelectedIndex].dateTime = DateTime.Parse(datetimePicker.Value.Value.ToString());
            }
            label.Content = "[" + viewItem[combo.SelectedIndex].title + "]の変更を保存しました！";
            dataFile.changeData(viewItem[combo.SelectedIndex]);
            dataFile.Parse();
            changeViewItem(date_picker.SelectedDate.ToString());
            resetForm();

        }

        private void changeViewItem(string date) {
            combo.Items.Clear();
            title.Clear();
            viewItem.Clear();
            List<ScheduleData> data = dataFile.getDataByDateTime(date);
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
            changeViewItem(date_picker.SelectedDate.ToString());
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
            changeViewItem(date_picker.SelectedDate.ToString());
            textBox_title.Text = "タイトル";
            textBox.Text = "詳細";
            datetimePicker.Value = null;
        }
        
        private void fixesIndex() {
            if (combo.SelectedIndex == -1) combo.SelectedIndex = comboSelect;
        }
    }
}
