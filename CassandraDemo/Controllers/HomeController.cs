using Cassandra;
using CassandraDemo.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CassandraDemo.Controllers
{
    [RoutePrefix("posts")]
    public class HomeController : Controller
    {
        private readonly PostsContext _context;

        public HomeController()
            : this(new PostsContext())
        {
        }

        public HomeController(PostsContext context)
        {
            _context = context;
        }
        
        [Route()]
        public ActionResult Index()
        {
            var posts = _context.GetPosts();
            return View(posts);
        }

        [Route("{id}")]
        public ActionResult Post(string id)
        {
            var post = _context.GetPostById(id);
            return View(post);
        }

        [Route("{id}")]
        [HttpPost]
        public ActionResult Update(Post post)
        {
            TempData["PostId"] = post.Id;
            TempData["Action"] = PostAction.Updated;
            return RedirectToAction("Index");
        }

        [Route()]
        [HttpPost]
        public ActionResult Add([Bind(Exclude="Id")]Post post)
        {
            var id = _context.Add(post);

            TempData["PostId"] = id;
            TempData["Action"] = PostAction.Added;

            return RedirectToAction("Index");
        }

        [Route("{id}/delete")]
        public ActionResult Delete(string id)
        {
            _context.Delete(id);

            TempData["PostId"] = id;
            TempData["Action"] = PostAction.Deleted;

            return RedirectToAction("Index");
        }

    }
}