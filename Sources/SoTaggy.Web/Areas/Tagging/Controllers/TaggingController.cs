using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SoTaggy.Web.Data;
using SoTaggy.Web.Areas.Tagging.Models;
using SoTaggy.Web.Hubs;
using SoTaggy.Web.Areas.Admin.Models;
using System.Text;

namespace SoTaggy.Web.Areas.Tagging.Controllers
{
    public class TaggingController : Controller
    {
        private TagDbContext db = new TagDbContext();

        private SessionControlModel GetSessionControl()
        {
            var sessionControl = (SessionControlModel)Session["SessionControl"];

            if (sessionControl == null)
            {
                var group = db.TaggingGroups.SingleOrDefault(g => g.UserName.Equals(User.Identity.Name));

                if (group == null)
                {
                    ViewBag.Group = "unbekannt";
                    ViewBag.Unit = "unbekannt";
                }
                else
                {
                    // Session anlegen und Startseite zeigen
                    ViewBag.Group = group.Name;
                    ViewBag.Unit = group.Name;
                }

                ViewBag.Session = Session.SessionID;
                
                sessionControl = new SessionControlModel(group);
                Session["SessionControl"] = sessionControl;
            }

            return sessionControl;
        }


        public ActionResult Index()
        {
            Session["SessionControl"] = null;

            ViewBag.TaggingState = TaggingState.Ready;

            return NextImage();
        }

        public ViewResult Start()
        {
            Session["SessionControl"] = null;

            return NextImage();
        }

        //[HttpPost]
        public ViewResult PrevImage()
        {
            var sessionControl = GetSessionControl();

            var idm = sessionControl.GetPrevImage();

            var entry = GetImage(idm.ImageEntryID);
            ViewBag.Copyright = entry.CopyrightOwner.Name;

            var model = new TagModel();
            model.Image = idm;

            ViewBag.TaggingState = TaggingState.InProgress;

            return View("TaggingEditor", model);
        }

        //[HttpPost]
        public ViewResult NextImage()
        {
            // Hol das Objekt aus der Session State raus
            var sessionControl = GetSessionControl();

            // Tamino: hol das aktuelle Bild
            var imageAktuell = GetImage(sessionControl.GetCurrentImage().ImageEntryID);

            var idm = sessionControl.GetNextImage();

            var entry = GetImage(idm.ImageEntryID);
            ViewBag.Copyright = entry.CopyrightOwner.Name;

            var model = new TagModel();
            model.Image = idm;

            // Tamino: Aktualisiere den TaggingState
            //var TaggingState = new TaggingStateModel();
            
            // Tamino: Aktualisiere die Views auf der Statusseite
            TaggingHub.UpdateViews(imageAktuell, entry, HttpContext.User.Identity.Name);

            ViewBag.TaggingState = TaggingState.InProgress;

            return View("TaggingEditor", model);
        }

        #region Tags
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">Der eingegebene Tag</param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult SendTag(TagModel model)
        {
            var sessionControl = GetSessionControl();

            var idm = sessionControl.GetCurrentImage();


            var tag = new ArtworkTag()
            {
                ArtworkId = idm.ImageEntryID,
                Tag = model.Tag,
                TaggingTime = DateTime.Now,
                TaggingGroupId = sessionControl.Group.TaggingGroupId
            };

            db.Taggings.Add(tag);
            db.SaveChanges();

            // Jetzt noch in der Session ablegen
            idm.Tags.Add(tag);

            // Tamino: Aktuellen Status von der DB abfragen. Alle Tags des Bildes in eine Liste schreiben
            ICollection<ArtworkTag> taggings = null;

            taggings = db.Taggings.Include("Artwork").Include("TaggingGroup")
            .Where(t => t.TaggingGroupId == sessionControl.Group.TaggingGroupId && t.ArtworkId == idm.ImageEntryID).ToList();

            // Tamino: Nach anzahl der tags und verischidenen tags auswerten 

            var _image = new TagAnalysisImage{

                    ArtworkID = idm.ImageEntryID,
                    numberTags = 0,
                    numberDifferentTags = 0,

            };

            var tagList = new List<TagList>();

            foreach (ArtworkTag _tag in taggings)
            {
                var tam = new TagList
                {
                    Tag = _tag.Tag,
                    Anzahl = 1,
                };

                    if (tagList.Exists(x => x.Tag == tam.Tag))
                    {
                        tagList.Find(x => x.Tag == tam.Tag).Anzahl++;  
                        _image.numberTags++;
                    }
                    else
                    {
                        tagList.Add(tam);
                        _image.numberTags++;
                        _image.numberDifferentTags++;
                    }
                }

            // Tamino: string für das Update bauen
            var writer = new StringBuilder();
            writer.Append("<table class='table-condensed table-striped col-sm-12'>");
            foreach (TagList tags in tagList)
            {
                writer.Append("<tr><td class='col-sm-6'>" + tags.Tag + 
                    "</td><td class='col-sm-6' style='text-align: right;'>" + tags.Anzahl + 
                    "</td></tr>");
            }
            writer.Append("</table><div id='ImageId' style='display: none;'>" + idm.ImageEntryID + "</div>");

            string[] pageUpdate = new string[] { _image.ArtworkID.ToString(), _image.numberTags.ToString(),
                _image.numberDifferentTags.ToString(), writer.ToString()};

            // Tamino: Tags auf det Statusseite aktualisieren
            
            TaggingHub.UpdateTags(pageUpdate, HttpContext.User.Identity.Name);

            // Das Modell für die Anzeige der Liste aktualisieren
            return PartialView("_TagList", idm);
        }

        [HttpPost]
        public PartialViewResult DeleteTag(int id)
        {
            var sessionControl = GetSessionControl();

            var idm = sessionControl.GetCurrentImage();

            // zuerst den Tag speichern
            var tag = db.Taggings.SingleOrDefault(t => t.ArtworkTagId == id);
             
            if (tag != null)
            {
                db.Taggings.Remove(tag);
                db.SaveChanges();

                // Das Objekt tag gibt es nicht in der Liste, sondern ein Duplikat davon!!!
                var tag2 = idm.Tags.SingleOrDefault(t => t.ArtworkTagId == id);
                // Jetzt noch in der Session ablegen
                idm.Tags.Remove(tag2);
            }

            // Tamino: Aktuellen Status von der DB abfragen. Alle Tags des Bildes in eine Liste schreiben
            ICollection<ArtworkTag> taggings = null;

            taggings = db.Taggings.Include("Artwork").Include("TaggingGroup")
            .Where(t => t.TaggingGroupId == sessionControl.Group.TaggingGroupId && t.ArtworkId == idm.ImageEntryID).ToList();

            // Tamino: Nach anzahl der tags und verischidenen tags auswerten 

            var _image = new TagAnalysisImage
            {

                ArtworkID = idm.ImageEntryID,
                numberTags = 0,
                numberDifferentTags = 0,

            };

            var tagList = new List<TagList>();

            foreach (ArtworkTag _tag in taggings)
            {
                var tam = new TagList
                {
                    Tag = _tag.Tag,
                    Anzahl = 1,
                };

                if (tagList.Exists(x => x.Tag == tam.Tag))
                {
                    tagList.Find(x => x.Tag == tam.Tag).Anzahl++;
                    _image.numberTags++;
                }
                else
                {
                    tagList.Add(tam);
                    _image.numberTags++;
                    _image.numberDifferentTags++;
                }
            }

            // Tamino: string für das Update bauen

            var writer = new StringBuilder();
            writer.Append("<table class='table-condensed table-striped col-sm-12'>");
            foreach (TagList tags in tagList)
            {
                writer.Append("<tr><td class='col-sm-6'>" + tags.Tag +
                    "</td><td class='col-sm-6' style='text-align: right;'>" + tags.Anzahl +
                    "</td></tr>");
            }
            writer.Append("</table><div id='ImageId' style='display: none;'>" + idm.ImageEntryID + "</div>");

            string[] pageUpdate = new string[] { _image.ArtworkID.ToString(), _image.numberTags.ToString(),
                _image.numberDifferentTags.ToString(), writer.ToString()};
            // Tamino: Tags auf det Statusseite aktualisieren

            TaggingHub.UpdateTags(pageUpdate, HttpContext.User.Identity.Name);

            // Das Modell für die Anzeige der Liste aktualisieren
            return PartialView("_TagList", idm);
        }
        #endregion


        private Artwork GetImage(int imageID)
        {
            return db.Artworks.SingleOrDefault(img => img.ArtworkId == imageID);
        }

        public ActionResult TaggingFinished()
        {
            // Die Liste aufbauen
            var model = new List<TaggingReviewModel>();

            var sessionControl = GetSessionControl();

            foreach (var idm in sessionControl.Images.OrderBy(img => img.Order))
            {
                // ab dem zweiten Lauf alle zeigen
                // im ersten lauf, in der gezeigten Reihenfolge darstellen
                if (sessionControl.Run > 1 || idm.Order > 0)
                {
                    var entry = db.Artworks.SingleOrDefault(img => img.ArtworkId == idm.ImageEntryID);
                    var trm = new TaggingReviewModel()
                    {
                        TaggingImage = idm,
                        PoolImage = entry,
                    };

                    model.Add(trm);
                }
            }

            ViewBag.TaggingState = TaggingState.Finished;

            return View(model);
        }

    }
}
