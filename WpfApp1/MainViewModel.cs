using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApp1
{
    public class RelayCommand : ICommand
    {
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }

    class MainViewModel : INotifyPropertyChanged
    {
        private int _currentPage = 1;
        private int _pageSize = 10;

        private string _message = "test";

        private List<ArticleModel> _articles = new List<ArticleModel>();
        public ObservableCollection<ArticleModel> DisplayItems { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Message
        {
            get { return _message; }
            set 
            { 
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public int CurrentPage
        {
            get { return _currentPage; } 
            set 
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
                UpdatePage();
            } 
        }

        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand ReloadCommand { get; }

        public async void GetItems()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync("https://localhost:7017/api/articles");

                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    string aux = await response.Content.ReadAsStringAsync();
                    dynamic values = JObject.Parse(aux);

                    if (values != null && values.value != null)
                    {
                        _articles.Clear();
                        foreach (JObject item in values.value)
                        {
                            int id = (int)item.GetValue("id");
                            string title = (string)item.GetValue("title");
                            string content = (string)item.GetValue("content");
                            DateTime publishedDate = (DateTime)item.GetValue("publishedDate");

                            _articles.Add(new ArticleModel(id, title, content, publishedDate));
                        }
                    }
                    Message = "Articles loaded";
                    
                }
                else
                    Message = $"Server error code {response.StatusCode}";
            }
            UpdatePage();
        }

        public MainViewModel()
        {
            DisplayItems = new ObservableCollection<ArticleModel>();

            NextPageCommand = new RelayCommand(_ => CurrentPage++, _ => CurrentPage < ((double)_articles.Count / _pageSize));
            PreviousPageCommand = new RelayCommand(_ => CurrentPage--, _ => CurrentPage > 1);
            ReloadCommand = new RelayCommand(_ => GetItems(), _ => true);

            GetItems();
        }

        public void UpdatePage()
        {
            DisplayItems.Clear();
            var items = _articles.Skip((_currentPage - 1) * _pageSize). Take(_pageSize);

            foreach(var item in items)
                DisplayItems.Add(item);
        }
    }
}
