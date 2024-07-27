using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class ArticleModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishedDate { get; set; }

        public ArticleModel(int id, string title, string content, DateTime publishedDate)
        {
            Id = id;
            Title = title;
            Content = content;
            PublishedDate = publishedDate;
        }
    }
}
