using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SoTaggy.Web.Areas.Admin.Models;
using SoTaggy.Web.Data;
using SoTaggy.Web.Models;
using System.IO;
using System.Text;
using System.Reflection;



namespace SoTaggy.Web.Areas.Admin.Controllers
{
    public class TaggingAnalysisAjaxController : Controller
    {

        private readonly TagDbContext db = new TagDbContext();

        // GET: Admin/TaggingAnalysisAjax
        public ActionResult Index()
        {
            var searchModel = new TagSearchModel();


            ViewBag.ArtworkPoolId = new MultiSelectList(db.ArtworkPools, "ArtworkPoolId", "Name", null);
            ViewBag.GroupTagId = new MultiSelectList(db.GroupTags, "GroupTagId", "Tag", null);

            return View(searchModel);
        }

        public ActionResult TagSearch(TagSearchModel searchData, string buttonType)
        {
            var TaggingStartTime = searchData.TaggingDateBegin.Add(searchData.TaggingTimeBegin);
            var TaggingEndTime = searchData.TaggingDateEnd.Add(searchData.TaggingTimeEnd);

            var context = new TagDbContext();

            bool errorExist = false;


            // 1. Vorbedingungen prüfen
            // wenn Auswahl, dann muss auch was ausgewählt sein => sonst Fehler!
            // wenn Zeitraum, dann muss startTime <= endTime sein => sonst fehler


            /*
             *  Anzeige, wenn "Alle" gewählt ist und trotzdem Elemente der Liste angeklickt wurden.
                Nur zum Verständnis, wenn Liste nicht die gewünschten Ergebnisse anzeigt.
             * 
            if (searchData.IsArtworkSelection == false && searchData.ArtworkPoolId != null)
            {
                ModelState.AddModelError("ArtworkPoolId", "Sie haben in der Liste eine Auswahl getroffen, aber bereits 'Alle' gewählt.");
            }

            if (searchData.IsGroupTagSelection == false && searchData.GroupTagId != null)
            {
                ModelState.AddModelError("GroupTagId", "Sie haben in der Liste eine Auswahl getroffen, aber bereits 'Alle' gewählt.");
            }
            */

            if (searchData.IsArtworkSelection == true && (searchData.ArtworkPoolId == null || searchData.ArtworkPoolId.Count() == 0))
            {
                ModelState.AddModelError("ArtworkPoolId", "Bitte wählen Sie Elemente aus der Liste.");
                errorExist = true;
            }


            if (searchData.IsTimeSpanSelection == true && (TaggingStartTime >= TaggingEndTime))
            {

                ModelState.AddModelError("TaggingDateBegin", "Die Startzeit muss vor der Endzeit liegen.");
                errorExist = true;

            }


            if (searchData.IsGroupTagSelection == true && (searchData.GroupTagId == null || searchData.GroupTagId.Count() == 0))
            {
                ModelState.AddModelError("GroupTagId", "Bitte wählen Sie Elemente aus der Liste.");
                errorExist = true;

            }

            if (errorExist)
            {

                // das Modell für die Anzeige muss jetzt wieder gebaut werden
                ViewBag.ArtworkPoolId = new MultiSelectList(db.ArtworkPools, "ArtworkPoolId", "Name", null);
                ViewBag.GroupTagId = new MultiSelectList(db.GroupTags, "GroupTagId", "Tag", null);


                return View(searchData);
            }

            ICollection<ArtworkTag> taggings = null;

            if (searchData.IsArtworkSelection == true && searchData.IsTimeSpanSelection == true && searchData.IsGroupTagSelection == true)
            {
                taggings = context.Taggings.Include("Artwork").Include("TaggingGroup")
                .Where(t => t.TaggingTime >= TaggingStartTime && t.TaggingTime <= TaggingEndTime &&
                searchData.ArtworkPoolId.Contains(t.Artwork.ArtworkPool.ArtworkPoolId) &&
                t.TaggingGroup.GroupTags.Any(g => searchData.GroupTagId.Contains(g.GroupTagId))
                ).ToList();
            }

           // ArtworkId == Auswahl && TimeSpan == Zeitraum && GroupTagId == Alle
            else if (searchData.IsArtworkSelection == true && searchData.IsTimeSpanSelection == true && searchData.IsGroupTagSelection == false)
            {
                taggings = context.Taggings.Include("Artwork").Include("TaggingGroup")
                .Where(t => t.TaggingTime >= TaggingStartTime && t.TaggingTime <= TaggingEndTime &&
                searchData.ArtworkPoolId.Contains(t.Artwork.ArtworkPool.ArtworkPoolId)).ToList();
            }

            // ArtworkId == Auswahl && TimeSpan == keine Einschränkung && GroupTagId == Auswahl
            else if (searchData.IsArtworkSelection == true && searchData.IsTimeSpanSelection == false && searchData.IsGroupTagSelection == true)
            {
                taggings = context.Taggings.Include("Artwork").Include("TaggingGroup")
                .Where(t => t.TaggingGroup.GroupTags.Any(g => searchData.GroupTagId.Contains(g.GroupTagId) &&
                searchData.ArtworkPoolId.Contains(t.Artwork.ArtworkPool.ArtworkPoolId))
                ).ToList();
            }

            // ArtworkId == Alle && TimeSpan == Zeitraum && GroupTagId == Auswahl
            else if (searchData.IsArtworkSelection == false && searchData.IsTimeSpanSelection == true && searchData.IsGroupTagSelection == true)
            {
                taggings = context.Taggings.Include("Artwork").Include("TaggingGroup")
                .Where(t => t.TaggingTime >= TaggingStartTime && t.TaggingTime <= TaggingEndTime &&
                t.TaggingGroup.GroupTags.Any(g => searchData.GroupTagId.Contains(g.GroupTagId))
                ).ToList();
            }

            // ArtworkId == Auswahl && TimeSpan == keine Einschränkung && GroupTagId == Alle
            else if (searchData.IsArtworkSelection == true && searchData.IsTimeSpanSelection == false && searchData.IsGroupTagSelection == false)
            {
                taggings = context.Taggings.Include("Artwork").Include("TaggingGroup")
                .Where(t => searchData.ArtworkPoolId.Contains(t.Artwork.ArtworkPool.ArtworkPoolId))
                .ToList();
            }

            // ArtworkId == Alle && TimeSpan == Zeitraum && GroupTagId == Alle
            else if (searchData.IsArtworkSelection == false && searchData.IsTimeSpanSelection == true && searchData.IsGroupTagSelection == false)
            {
                taggings = context.Taggings.Include("Artwork").Include("TaggingGroup")
                .Where(t => t.TaggingTime >= TaggingStartTime && t.TaggingTime <= TaggingEndTime
                ).ToList();
            }

            // ArtworkId == Alle && TimeSpan == keine Einschränkung && GroupTagId == Auswahl
            else if (searchData.IsArtworkSelection == false && searchData.IsTimeSpanSelection == false && searchData.IsGroupTagSelection == true)
            {
                taggings = context.Taggings.Include("Artwork").Include("TaggingGroup")
                .Where(t => t.TaggingGroup.GroupTags.Any(g => searchData.GroupTagId.Contains(g.GroupTagId))
                ).ToList();
            }

            // ArtworkId == Alle && TimeSpan == keine Einschränkung && GroupTagId == Alle
            else if (searchData.IsArtworkSelection == false && searchData.IsTimeSpanSelection == false && searchData.IsGroupTagSelection == false)
            {
                taggings = context.Taggings.Include("Artwork").Include("TaggingGroup")
                .ToList();
            }


            if (buttonType == "download")
            {
                return TaggingAnalysisDownload(taggings);
            }
            else
            {
                return TaggingAnalysisWeb(taggings);
            }
        }

        public ActionResult TaggingAnalysisWeb(ICollection<ArtworkTag> taggings)
        {
            var modelGroupedByImage = new List<TagAnalysisImage>();

            var model = new List<TagList>();

            foreach (var tag in taggings)
            {
                var imageTags = new TagAnalysisImage
                {
                    Artwork = tag.Artwork.Title,
                    ArtworkID = tag.ArtworkId.Value,
                    ImagePoolName = tag.Artwork.ArtworkPool.Name,

                };
                var tam = new TagList
                {
                    Tag = tag.Tag,
                    Anzahl = 1,
                };
                if (modelGroupedByImage.Exists(x => x.ArtworkID == imageTags.ArtworkID && x.ImagePoolName == imageTags.ImagePoolName))
                {

                    model = modelGroupedByImage.First(x => x.ArtworkID == imageTags.ArtworkID && x.ImagePoolName == imageTags.ImagePoolName).Tags.ToList();

                    if (model.Exists(x => x.Tag.ToLower().Equals(tam.Tag.ToLower())))
                    {
                        model.Find(x => x.Tag.ToLower().Equals(tam.Tag.ToLower())).Anzahl++;
                    }
                    else
                    {
                        model.Add(tam);
                    }

                    modelGroupedByImage.First(x => x.ArtworkID == imageTags.ArtworkID && x.ImagePoolName == imageTags.ImagePoolName).Tags = model;
                }
                else
                {


                    var taglist = new List<TagList>();

                    taglist.Add(tam);

                    imageTags.Tags = taglist;

                    modelGroupedByImage.Add(imageTags);

                }
            };
            // Bilder alphabetisch sortieren
            modelGroupedByImage = modelGroupedByImage.OrderBy(x => x.Artwork).ToList();
            // tags nach Anzahl der Nennungen sortieren
            foreach (var image in modelGroupedByImage)
            {
                image.Tags = image.Tags.OrderByDescending(x => x.Anzahl).ToList();
            };

            return PartialView("_TagList", modelGroupedByImage);
        }
        public ActionResult TaggingAnalysisDownload(ICollection<ArtworkTag> taggings)
        {
            var model = new List<TagAnalysisModel>();

            foreach (var tag in taggings)
            {
                var tam = new TagAnalysisModel
                {
                    Tag = tag.Tag,
                    Artwork = tag.Artwork.Title,
                    ImagePoolName = tag.Artwork.ArtworkPool.Name,
                    TaggingTime = tag.TaggingTime,
                    GroupTags = tag.TaggingGroup.GetTagList(),
                };


                model.Add(tam);
            }

            //TagDatei string bauen
            var writer = new StringBuilder();
            
            writer.AppendLine("Schlagwort;Bild;Bildpool;Zeitstempel;Gruppenschlagworte");
            
            foreach (var item in model)
            {
                writer.AppendLine(String.Format("{0};{1};{2};{3};{4}",
                    item.Tag,
                    item.Artwork,
                    item.ImagePoolName,
                    item.TaggingTime,
                    item.GroupTags
                    ));
            }

            // TagDatei string in der Session hinterlegen, kann später über den Link "TagDownload" abgerufen werden
            Session["TagDatei"] = writer.ToString();


            // Anzeige, wenn "Alle" gewählt ist und trotzdem Elemente der Liste angeklickt wurden.
            // Nur zum Verständnis, wenn Liste nicht die gewünschten Ergebnisse anzeigt.
            //ViewBag.ArtworkPoolId = new MultiSelectList(db.ArtworkPools, "ArtworkPoolId", "Name", null);
            //ViewBag.GroupTagId = new MultiSelectList(db.GroupTags, "GroupTagId", "Tag", null);

            return Content("Download...");
        }
        public ActionResult TagsDownload()
        {
            MemoryStream ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);

            writer.Write(Session["TagDatei"].ToString()); 

            writer.Flush();
            writer.Dispose();

            return File(ms.GetBuffer(), "text/csv", "Taggings.csv");
        }
    }
}