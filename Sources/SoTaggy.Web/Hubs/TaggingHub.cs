using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using SoTaggy.Web.Data;
using SoTaggy.Web.Areas.Admin.Models;
using Microsoft.Ajax.Utilities;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace SoTaggy.Web.Hubs
{
    public class TaggingHub : Hub
    {            

        
        public static void UpdateTags(string[] pageUpdate, string groupName)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<TaggingHub>();

            hubContext.Clients.Group(groupName).updateImageClient(pageUpdate);
        }
        public Task AssignToGroupServer(string GroupId)
        {
            return Groups.Add(Context.ConnectionId, GroupId);
        }
        public static void UpdateViews(Artwork oldImage, Artwork newImage, string groupName)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<TaggingHub>();
        }
    }
}