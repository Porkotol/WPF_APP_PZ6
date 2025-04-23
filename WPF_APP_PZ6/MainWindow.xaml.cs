using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace WPF_APP_PZ6
{
    public partial class MainWindow : Window
    {
        private BindingList<User> _users;
        private readonly string _filePath = "C:\\Users\\Zeuka\\source\\repos\\WPF_APP_PZ6\\WPF_APP_PZ6\\usergrid.json";

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        public class User : INotifyPropertyChanged
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime Birthday { get; set; }
            public string Email { get; set; }
            
            private bool _isActive;
            public bool IsActive
            {
                get => _isActive;
                set
                {
                    _isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                    OnPropertyChanged(nameof(Status));
                    OnPropertyChanged(nameof(Details));
                }
            }
            
            public string Status => IsActive ? "Активний" : "Неактивний";
            public int Age => DateTime.Now.Year - Birthday.Year;
            public string Details => $"{Name} (ID: {Id}) - {Status}";

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void LoadData()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    File.WriteAllText(_filePath, "[]");
                }

                string jsonContent = File.ReadAllText(_filePath);
                _users = new BindingList<User>(JsonConvert.DeserializeObject<List<User>>(jsonContent));
                _users.ListChanged += Users_ListChanged;
                userGrid.ItemsSource = _users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Users_ListChanged(object sender, ListChangedEventArgs e)
        {
            SaveData();
        }

        private void SaveData()
        {
            try
            {
                string json = JsonConvert.SerializeObject(_users, Formatting.Indented);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ShowDetails_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button button && button.Tag is int id)
            {
                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user != null)
                {
                    MessageBox.Show($"Детальна інформація про користувача {user.Name}:\n\n" +
                                  $"Вік: {user.Age} років\n" +
                                  $"Дата народження: {user.Birthday:dd.MM.yyyy}\n" +
                                  $"Статус: {user.Status}",
                                  "Деталі користувача",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                }
            }
        }
    }
}