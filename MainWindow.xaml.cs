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

        private DataFile dataFile = new DataFile();

        private List<ListBoxItem> title =  new List<ListBoxItem>();
        private void Menu_Loaded(object sender, RoutedEventArgs e) {
            if (!dataFile.folderExists()) label.Content = "初回起動ですのでフォルダを作成しました";
            dataFile.makeFile();
            date_picker.SelectedDate = DateTime.Today;           
        }

        public MainWindow() {
            InitializeComponent();
        }

        public void add_sd(object sender, RoutedEventArgs e) {
            newItem();
        }

        public void delete_sd(object sender, RoutedEventArgs e) {
            deleteItem();
        }

        private void delete_button(object sender, RoutedEventArgs e) {
            deleteItem();
        }

        private void newItem() {
            ListBoxItem listBoxItem = new ListBoxItem();
            listBoxItem.Content = "新規作成";
            listBox.Items.Add(listBoxItem);
            title.Add(listBoxItem);
            dataFile.addData("新規作成", DateTime.Now, "詳細", false);
        }

        private void deleteItem() {
            if (listBox.SelectedIndex == -1) {
                label.Content = "予定が存在しません";
                return;
            }
            title.RemoveAt(listBox.SelectedIndex);
            listBox.Items.RemoveAt(listBox.SelectedIndex);
        }

        public void exit_click(object sender, RoutedEventArgs e) {
            Environment.Exit(0);
        }

        public void Listbox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (listBox.SelectedItem == null) return;
            try {
                label.Content = ((sender as System.Windows.Controls.ListBox).SelectedItem as ListBoxItem).Content.ToString();
            } catch (System.NullReferenceException) {
                label.Content = listBox.SelectedItem.ToString();
            }

            textBox_title.Text = title[listBox.SelectedIndex].Content.ToString();
        }

        private void Date_Changed(object sender, SelectionChangedEventArgs e) {
            //label.Content = date_picker.Text.ToString();
        }

        private bool isTitleWordCount(string title) {
            if (title.Length < 52) return true;
            return false;
        }

        public void LoadCsvByDate(string date) {

        }

        private void New_button(object sender, RoutedEventArgs e) {
            newItem();
        }


        private void Lost_Focus_textBox_title(object sender, EventArgs e) {
            label.Content = textBox_title.Text.ToString();
            if (listBox.SelectedItem == null) {
                textBox_title.Text = "タイトル";
                label.Content = "予定を選択または新規作成してください";
                return;
            }
            title[listBox.SelectedIndex].Content= textBox_title.Text.ToString();
        }

        private void Lost_Focus_textBox(object sender, EventArgs e) {
            label.Content = textBox.Text.ToString();
            if (listBox.SelectedItem == null) {
                textBox.Text = "詳細";
                label.Content = "予定を選択または新規作成してください";
            }
        }

        private void Date_Change_todo(object sender, SelectionChangedEventArgs e) {
            label.Content = date_picker.Text.ToString();
        }

        private void DateTime_Change_contents(object sender, RoutedEventArgs e) {
            if (datetimePicker.Value == null) return;
            label.Content = datetimePicker.Value.Value.ToString("yyyy/MM/dd");
            if (listBox.SelectedItem == null) {
                datetimePicker.Value = null;
                label.Content = "予定を選択または新規作成してください";
                return;
            }
            if (datetimePicker.Value.Value.ToString("yyyy/MM/dd") != date_picker.SelectedDate.Value.ToString("yyyy/MM/dd")) {
                //listboxから消す処理
                label.Content = "none";
            }

        }

    }
}
