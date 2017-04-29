using SoTaggy.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoTaggy.Web.Areas.Admin.Controllers
{
    public class ImageController : Controller
    {
        private TagDbContext db = new TagDbContext();

        public ActionResult Index()
        {
            var model = db.Images.ToList();

            return View(model);
        }

        public ActionResult ShowImage(int id = 0)
        {
            var artwork = db.Artworks.SingleOrDefault(img => img.ArtworkId == id);

            if (artwork != null)
            {
                var image = artwork.Images.FirstOrDefault();
                if (image != null)
                {
                    return File(image.ImageData, image.ImageFileType);
                }
            }

            return File("~/images/notfound.jpg", "image/jpg");
        }

        public ActionResult ShowThumnailImage(int id = 0)
        {
            var artwork = db.Artworks.SingleOrDefault(img => img.ArtworkId == id);

            if (artwork != null)
            {
                var image = artwork.Images.FirstOrDefault();
                if (image != null)
                {
                    return File(image.ImageData, image.ImageFileType);
                }
            }

            return File("~/images/notfound.jpg", "image/jpg");
        }

    }
}
