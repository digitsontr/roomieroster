using AutoMapper;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;
using RoommateMatcher.Models;
using RoommateMatcher.Dtos;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RoommateMatcher.Hubs
{
    [Authorize]
	public class ChatHub:Hub
	{
        public readonly static List<UserDto> _Connections = new List<UserDto>();
        private readonly static Dictionary<string, string> _ConnectionsMap = new Dictionary<string, string>();

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ChatHub(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task SendPrivate(string receiverName, string message)
        {
            if (_ConnectionsMap.TryGetValue(receiverName, out string connectionId))
            {
                var sender = _Connections.Where(u => u.Username
                    == IdentityName).First();
                var reciever = _Connections.Where(u => u.Username
                == receiverName).First();

                if (!string.IsNullOrEmpty(message.Trim()))
                {
                    var chatId = _context.Messages
                        .Where(z => (z.ReceiverId == reciever.Id &&
                        z.SenderId == sender.Id) || (z.ReceiverId
                        == sender.Id && z.SenderId == reciever.Id))
                        .Select(z=>z.ChatId)
                        .FirstOrDefault();

                    if (chatId < 1)
                    {
                        Chat chat = new Chat();

                       var entity = await _context.Chats.AddAsync(chat);
                       await _context.SaveChangesAsync();

                       chatId = entity.Entity.Id;
                    }

                    var messageModel = new Message()
                    {
                        Content = Regex.Replace(message, @"<.*?>", string.Empty),
                        ReceiverId = reciever.Id,
                        SenderId = sender.Id,
                        CreatedAt = DateTime.Now,
                        ChatId = chatId
                    };

                    await _context.Messages.AddAsync(messageModel);
                    await _context.SaveChangesAsync();

                  
                    await Clients.Caller.SendAsync("newMessage",
                        JsonConvert.SerializeObject(messageModel, Formatting.Indented,
                            new JsonSerializerSettings
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            }));
                    await Clients.Client(connectionId).SendAsync("newMessage",
                      JsonConvert.SerializeObject(messageModel,Formatting.Indented,
                            new JsonSerializerSettings
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            }));
                }
            }
        }

        public async Task GetMessages()
        {
            var sender = _Connections.Where(u => u.Username
                    == IdentityName).First();
            var messages = _context.Messages
                      .Where(z=>z.ReceiverId == sender.Id || z.SenderId == sender.Id).OrderBy(z=>z.ChatId);

        
                await Clients.Caller.SendAsync("previousMessages",
                  JsonConvert.SerializeObject(messages, Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }));
            
        }

        public override Task OnConnectedAsync()
        {
            try
            {
                var user = _context.Users.Where(u => u.UserName
                    == IdentityName).SingleOrDefault();
                var userDto = _mapper.Map<UserDto>(user);
   

                if (!_Connections.Any(u => u.Username == IdentityName))
                {
                    _Connections.Add(userDto);
                    _ConnectionsMap.Add(IdentityName, Context.ConnectionId);
                }


                Clients.All.SendAsync("userConnected", _Connections.Select(z=>z.Username).ToList());
            }
            catch (Exception ex)
            {
                Clients.Caller.SendAsync("onError", "OnConnected:" + ex.Message);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                var user = _Connections.Where(u => u.Username
                == IdentityName).First();
                _Connections.Remove(user);

                _ConnectionsMap.Remove(user.Username);

            }
            catch (Exception ex)
            {
                Clients.Caller.SendAsync("onError", "OnDisconnected: " + ex.Message);
            }

            return base.OnDisconnectedAsync(exception);
        }

        private string IdentityName
        {
            get { return Context.User.Identity.Name; }
        }
    }
}

