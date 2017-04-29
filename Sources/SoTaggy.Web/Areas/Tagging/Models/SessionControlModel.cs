using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoTaggy.Web.Data;

namespace SoTaggy.Web.Areas.Tagging.Models
{
    public enum TaggingState
    {
        Ready,
        InProgress,
        Finished
    }

    public class SessionControlModel
    {
        public TaggingGroup Group { get; private set; }

        public List<ImageDisplayModel> Images { get; private set; }

        public int CurrentOrder { get; private set; }
        public int MaxOrder { get; private set; }
        public int Run { get; private set; }

        public SessionControlModel(TaggingGroup group)
        {
            this.Group = group;
            Images = new List<ImageDisplayModel>();
            Run = 0;

            foreach (var img in group.ArtworkPool.Artworks)
            {
                var imgModel = new ImageDisplayModel()
                {
                    ImageEntryID = img.ArtworkId,
                    Order = 0,
                };

                Images.Add(imgModel);
            }

            // Hier sicher stellen, dass ImgageList ein Element enthällt

            ResestSession();
        }

        public ImageDisplayModel GetCurrentImage()
        {
            // bei Beginn ist alles 0
            if (CurrentOrder == 0 && MaxOrder == 0)
                return GetNextImage();

            var imgModel = Images.SingleOrDefault(img => img.Order == CurrentOrder);
            return imgModel;
        }


        /// <summary>
        /// Liefert die ID des nächsten Bildes in der Session
        /// </summary>
        /// <returns></returns>
        public ImageDisplayModel GetNextImage()
        {
            // sind wir in der Historie oder im Neuland?
            if (CurrentOrder == MaxOrder)
            {
                // Neuland
                // Alle Bilde, die noch nicht gezeigt wurden
                var images = Images.Where(img => img.Order == 0).ToList();

                var n = images.Count;

                if (n == 0)
                {
                    // wenn n = 0, dann sind alle Bidler gezeigt => zurücksetzen
                    ResestSession();
                    return GetNextImage();
                }
                else
                {
                    // sonst: es geht einen Schritt weiter
                    CurrentOrder++;
                    MaxOrder++;

                    // aus den noch nicht gezeigten eines per Zufall auswählen
                    var r = new Random();

                    // Der 0-basierte Index des Bildes
                    var index = Math.Min((int)(r.NextDouble() * n + 0.1), images.Count-1);
                    
                    var imgModel = images[index];

                    // Setze die Reihenfolge
                    imgModel.Order = CurrentOrder;

                    // ImageID geht an den Aufrufer
                    return imgModel;
                }
            }
            else // CurrentOrder < MaxOrder
            {
                // Suche das Element mit der CurrentOrder + 1
                CurrentOrder++;
                return GetCurrentImage();
            }
        }

        /// <summary>
        /// Setzt den Zustand auf 0 zurück. Erhöht die Anzahl der Läufe
        /// </summary>
        private void ResestSession()
        {
            Run++;
            CurrentOrder = 0;
            MaxOrder = 0;

            Images.ForEach(img => img.Order = 0);
        }

        /// <summary>
        /// Liefert die ID des vorhergegangen Bildes
        /// </summary>
        /// <returns></returns>
        public ImageDisplayModel GetPrevImage()
        {
            CurrentOrder--;
            // Wenn erstes Bild erreicht, dann bei diesem bleiben
            if (CurrentOrder < 1)
                CurrentOrder = 1;

            return GetCurrentImage();
        }

    }

}