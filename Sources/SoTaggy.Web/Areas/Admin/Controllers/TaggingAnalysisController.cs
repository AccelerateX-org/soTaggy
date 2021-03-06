﻿using System;
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
    //Klasse um zweiten Button zu realisieren (link: "http://stackoverflow.com/questions/442704/how-do-you-handle-multiple-submit-buttons-in-asp-net-mvc-framework")
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MultipleButtonAttribute : ActionNameSelectorAttribute
    {
        public string Name { get; set; }
        public string Argument { get; set; }

        public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
        {
            var isValidName = false;
            var keyValue = string.Format("{0}:{1}", Name, Argument);
            var value = controllerContext.Controller.ValueProvider.GetValue(keyValue);

            if (value != null)
            {
                controllerContext.Controller.ControllerContext.RouteData.Values[Name] = Argument;
                isValidName = true;
            }

            return isValidName;
        }
    }
    public class TaggingAnalysisController : Controller
    {
        private readonly TagDbContext db = new TagDbContext();



        //GET: /TagAnalysisModel/
        // Über die Get-Methode wird die Seite zunächst nur angezeigt

        public ActionResult Index()
        {
            var searchModel = new TagSearchModel();


            ViewBag.ArtworkPoolId = new MultiSelectList(db.ArtworkPools, "ArtworkPoolId", "Name", null);
            ViewBag.GroupTagId = new MultiSelectList(db.GroupTags, "GroupTagId", "Tag", null);

            return View(searchModel);
        }



        // SET: /TagAnalysisModel/
        // Über die Post-Methode kommen die Daten vom Formular zurück
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Download")]
        public ActionResult Index(TagSearchModel searchData)
        {
            var TaggingStartTime = searchData.TaggingDateBegin.Add(searchData.TaggingTimeBegin);
            var TaggingEndTime = searchData.TaggingDateEnd.Add(searchData.TaggingTimeEnd);

            var context = new TagDbContext();

            ICollection<ArtworkTag> taggings = null;

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




            // 2. nur weitermachen, wenn keine Fehler

            // ArtworkId == Auswahl && TimeSpan == Zeitraum && GroupTagId == Auswahl
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

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);


            //Response.Clear();
            //Response.AddHeader("content-Disposition", "attachement; filename=taggings.csv");
            //Response.ContentType = "text/csv";
            writer.Write("Schlagwort;Bild;Bildpool;Zeitstempel;Gruppenschlagworte");

            writer.Write(Environment.NewLine);
            foreach (var item in model)
            {
                writer.Write(String.Format("{0};{1};{2};{3};{4}",
                    item.Tag,
                    item.Artwork,
                    item.ImagePoolName,
                    item.TaggingTime,
                    item.GroupTags
                    ));
                writer.Write(Environment.NewLine);

            }

            writer.Flush();
            writer.Dispose();
            // Anzeige, wenn "Alle" gewählt ist und trotzdem Elemente der Liste angeklickt wurden.
            // Nur zum Verständnis, wenn Liste nicht die gewünschten Ergebnisse anzeigt.
            //ViewBag.ArtworkPoolId = new MultiSelectList(db.ArtworkPools, "ArtworkPoolId", "Name", null);
            //ViewBag.GroupTagId = new MultiSelectList(db.GroupTags, "GroupTagId", "Tag", null);
            return File(ms.GetBuffer(), "text/csv", "Taggings.csv"); // && View(searchData);

        }
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "TaggingAnalysisWeb")]
        public ActionResult ShowInBrowser(TagSearchModel searchData)
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

            return TaggingAnalysisWeb(searchData);
        }

        public ActionResult TaggingAnalysisWeb(TagSearchModel searchData)
        {
            var TaggingStartTime = searchData.TaggingDateBegin.Add(searchData.TaggingTimeBegin);
            var TaggingEndTime = searchData.TaggingDateEnd.Add(searchData.TaggingTimeEnd);

            var context = new TagDbContext();

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

                    if (model.Exists(x => x.Tag == tam.Tag))
                    {
                        model.Find(x => x.Tag == tam.Tag).Anzahl++;
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

            TagDbContext db = new TagDbContext();

            var img_model = db.Images.ToList();

            ViewBag.Taglist = modelGroupedByImage;
            return View(img_model);
        }

        public ActionResult ShowImage(int id = 0)
        {
            TagDbContext db = new TagDbContext();

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
