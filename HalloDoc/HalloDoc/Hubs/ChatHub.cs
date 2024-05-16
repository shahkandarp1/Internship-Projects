using DocumentFormat.OpenXml.InkML;
using HalloDoc.Repository.Interface;
using HalloDoc.Repository.Repository;
using HalloDoc.ViewModels;
using Microsoft.AspNetCore.SignalR;

namespace HalloDoc.Hubs
{
    public class ChatHub:Hub
    {
        private readonly IJwtService _jwt;
        private readonly IHttpContextAccessor _context;
        private readonly IAdmin _admin;
        private Dictionary<string, string> activeMembers = new Dictionary<string, string>();
        public ChatHub(IHttpContextAccessor context,IJwtService jwt,IAdmin admin)
        {
            _context = context;
            _jwt = jwt;
            _admin = admin;
        }
        public override Task OnConnectedAsync()
        {
            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            if(cookieModel.role != "Admin")
            {
                Groups.AddToGroupAsync(Context.ConnectionId, cookieModel.aspId.ToString());
                Groups.AddToGroupAsync(Context.ConnectionId, cookieModel.aspId.ToString() + "_admin");
            }
            else
            {
                List<AspNetUser> aspnetusers = _admin.getAllAspNetUsers();
                for(var i=0;i<aspnetusers.Count;i++)
                {
                    Groups.AddToGroupAsync(Context.ConnectionId, aspnetusers[i].Id.ToString() + "_admin");
                }
            }
            activeMembers.Add(cookieModel.aspId.ToString(), Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            activeMembers.Remove(cookieModel.aspId.ToString());
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string reciever, string requestId, string message, string timestamp)
        {
            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            await Clients.Group(reciever).SendAsync("ReceiveMessage", cookieModel.aspId, requestId, message, timestamp);
        }

        public async Task SendMessageToGroup(string reciever, string requestId, string message, string timestamp,string chatwith,string sentby)
        {
            var request = _context.HttpContext.Request;
            var token = request.Cookies["jwt"];
            CookieModel cookieModel = _jwt.GetDetails(token);

            await Clients.Group(reciever + "_admin").SendAsync("ReceiveMessageInGroup", reciever, requestId, message, timestamp, chatwith, sentby);
        }

        public async Task AddToGroup(string reciever)
        {
            Groups.AddToGroupAsync(Context.ConnectionId, reciever + "_admin");
        }
    }
}
