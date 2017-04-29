using SoTaggy.Web.Areas.Admin.Models;
using SoTaggy.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoTaggy.Web.Areas.Admin.Controllers
{
    public class TaggingStateController : Controller
    {

        private readonly TagDbContext db = new TagDbContext();

        // GET: Admin/TaggingState
        public ActionResult Index()
        {
            var searchModel = new TagSearchStateModel();

            ViewBag.TaggingGroupId = new MultiSelectList(db.TaggingGroups, "TaggingGroupId", "Name", null);

            return View(searchModel);
        }

        [HttpPost]
        public ActionResult Index(TagSearchStateModel searchData)
        {
            // Alle Tags der ausgewählten Gruppe in eine Liste schreiben
            ICollection<ArtworkTag> taggings = null;

            taggings = db.Taggings.Include("Artwork").Include("TaggingGroup")
            .Where(t => t.TaggingGroupId == searchData.TaggingGroupId).ToList();

            // Alle bilder des Bilderpools der ausgewählten Gruppe in eine Liste schreiben
            ICollection<Artwork> imagesForGroup = null;

            imagesForGroup = db.Artworks.Where(t => t.ArtworkPoolId == db.TaggingGroups
                .FirstOrDefault(g => g.TaggingGroupId == searchData.TaggingGroupId).ArtworkPoolId).ToList();


            // Bilder in ein Analyse Model schreiben
            var modelGroupedByImage = new List<TagAnalysisImage>();

            foreach (Artwork image in imagesForGroup)
            {
                var newImage = new TagAnalysisImage
                {
                    Artwork = image.Title,
                    ArtworkID = image.ArtworkId,
                    ImagePoolName = image.ArtworkPool.Name,
                    numberTags = 0,
                    numberDifferentTags = 0,
                };
                modelGroupedByImage.Add(newImage);
            }


            // Tags nach Images gruppieren und nach anzahl der tags und verischidenen tags auswerten 

            var model = new List<TagList>();

            foreach (var tag in taggings)
            {
                var imageTags = new TagAnalysisImage
                {
                    Artwork = tag.Artwork.Title,
                    ArtworkID = tag.ArtworkId.Value,
                    ImagePoolName = tag.Artwork.ArtworkPool.Name,
                    numberTags = 0,
                    numberDifferentTags = 0,

                };
                var tam = new TagList
                {
                    Tag = tag.Tag,
                    Anzahl = 1,
                };

                if (modelGroupedByImage.First(x => x.ArtworkID == imageTags.ArtworkID && x.ImagePoolName == imageTags.ImagePoolName).Tags != null)
                {

                    model = modelGroupedByImage.First(x => x.ArtworkID == imageTags.ArtworkID && x.ImagePoolName == imageTags.ImagePoolName).Tags.ToList();

                    if (model.Exists(x => x.Tag == tam.Tag))
                    {
                        model.Find(x => x.Tag == tam.Tag).Anzahl++;

                        modelGroupedByImage.First(x => x.ArtworkID == imageTags.ArtworkID && x.ImagePoolName == imageTags.ImagePoolName).numberTags++;
                    }
                    else
                    {
                        model.Add(tam);

                        modelGroupedByImage.First(x => x.ArtworkID == imageTags.ArtworkID && x.ImagePoolName == imageTags.ImagePoolName).numberTags++;
                        modelGroupedByImage.First(x => x.ArtworkID == imageTags.ArtworkID && x.ImagePoolName == imageTags.ImagePoolName).numberDifferentTags++;
                    }
                }
                else
                {
                    model.Add(tam);

                    modelGroupedByImage.First(x => x.ArtworkID == imageTags.ArtworkID && x.ImagePoolName == imageTags.ImagePoolName).numberTags++;
                    modelGroupedByImage.First(x => x.ArtworkID == imageTags.ArtworkID && x.ImagePoolName == imageTags.ImagePoolName).numberDifferentTags++;
                }
                modelGroupedByImage.First(x => x.ArtworkID == imageTags.ArtworkID && x.ImagePoolName == imageTags.ImagePoolName).Tags = model;
            }   
                // Bilder alphabetisch sortieren
                modelGroupedByImage = modelGroupedByImage.OrderBy(x => x.Artwork).ToList();

                ViewBag.GroupId = db.TaggingGroups.FirstOrDefault(t => t.TaggingGroupId == searchData.TaggingGroupId).UserName.ToString();
                return PartialView("_TaggingState", modelGroupedByImage);
            }
            [HttpPost]
            public ActionResult GetTags(int imageId, string groupId)
            {
                // Tamino: Aktuellen Status von der DB abfragen. Alle Tags des Bildes in eine Liste schreiben
                ICollection<ArtworkTag> taggings = null;

                taggings = db.Taggings.Include("Artwork").Include("TaggingGroup")
                .Where(t => t.TaggingGroup.UserName == groupId && t.ArtworkId == imageId).ToList();

                // Tamino: Nach anzahl der tags und verischidenen tags auswerten 

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
                    }
                    else
                    {
                        tagList.Add(tam);
                    }
                }
                ViewBag.ImageId = imageId;
                return PartialView("_SingleImageTags", tagList);
            }
        }
    }