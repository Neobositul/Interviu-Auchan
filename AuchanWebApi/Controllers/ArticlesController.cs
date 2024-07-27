using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AuchanWebApi.Models;
using AuchanWebApi.Data;

namespace AuchanWebApi.Controllers
{
    [Route("api/articles")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly ApiContext _context;

        public ArticlesController(ApiContext context)
        {
            _context = context;
        }

        // Get all articles
        [HttpGet]
        public JsonResult Get()
        {
            var articlesInDb = _context.Articles.ToList();
            if (articlesInDb.Count > 0 ) 
                return new JsonResult(Ok(articlesInDb));

            return new JsonResult(NotFound());
        }

        // Get article by id
        [HttpGet("{id}")]
        public JsonResult GetById(int id)
        {
            var articleInDb = _context.Articles.Find(id);

            if (articleInDb == null)
                return new JsonResult(NotFound());

            return new JsonResult(Ok(articleInDb));
        }

        // Create new article
        [HttpPost]
        public JsonResult Post(Article article) 
        {
            var articleInDb = _context.Articles.Find(article.Id);

            if (articleInDb == null)
            {
                _context.Articles.Add(article);
                _context.SaveChanges();
                
                return new JsonResult(Ok(article));
            }

            return new JsonResult(Conflict("Article id already exists"));
        }

        // Edit article by id
        [HttpPut("{id}")]
        public JsonResult Put(int id, Article newArticle)
        {
            var articleInDb = _context.Articles.Find(id);

            if (articleInDb == null)
                return new JsonResult(NotFound("Article not fond"));

            _context.Articles.Remove(articleInDb);
            _context.Articles.Add(newArticle);
            _context.SaveChanges();

            return new JsonResult(Ok(newArticle));
        }

        // Delete article by id
        [HttpDelete("{id}")]
        public JsonResult DeleteById(int id)
        {
            var articleInDb = _context.Articles.Find(id);

            if (articleInDb == null)
                return new JsonResult(NotFound("Article not found"));

            _context.Articles.Remove(articleInDb);
            _context.SaveChanges();

            return new JsonResult(NoContent());
        }
    }
}
