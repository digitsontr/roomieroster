using AutoMapper;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;
using RoommateMatcher.Models;
using RoommateMatcher.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using RoommateMatcher.Loggers;

namespace RoommateMatcher.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatHub>
    {
        private static readonly ConcurrentDictionary<string, UserDto>
            _Connections = new ConcurrentDictionary<string, UserDto>();
        private static readonly ConcurrentDictionary<string, string>
            _ConnectionsMap = new ConcurrentDictionary<string, string>();

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly HubLogger _logger;

        public ChatHub(AppDbContext context, IMapper mapper, HubLogger logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task SendPrivate(string receiverName, string message)
        {
            try
            {
                var sender = _Connections[IdentityName];
                var receiver = await _context.Users.Where(z => z.UserName
                    == receiverName).SingleOrDefaultAsync();

                if (receiver == null)
                {
                    _logger.Log($"ERROR: There is no reciever on database");

                    await Clients.Caller.Error(false,
                            $"{receiverName} not found");
                    return;
                }

                if (!string.IsNullOrEmpty(message.Trim()))
                {
                    var chatId = _context.Messages
                        .Where(z => (z.RecieverUserName == receiver.UserName &&
                        z.SenderUserName == sender.Username) || (z.RecieverUserName
                        == sender.Username && z.SenderUserName == receiver.UserName))
                        .Select(z => z.ChatId)
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
                        RecieverUserName = receiver.UserName,
                        SenderUserName = sender.Username,
                        CreatedAt = DateTime.Now,
                        ChatId = chatId
                    };

                    await _context.Messages.AddAsync(messageModel);
                    await _context.SaveChangesAsync();

                    await Clients.Caller
                            .NewMessage(_mapper.Map<MessageDto>(messageModel));

                    _logger.Log($"INFO: Message has been sent to user " +
                             $"{receiverName}");

                    if (_ConnectionsMap.TryGetValue(receiverName,
                        out string receiverConnectionId))
                    {
                        await Clients.Client(receiverConnectionId)
                            .NewMessage(_mapper.Map<MessageDto>(messageModel));
                    }
                    else
                    {
                        var chat = _context.UnreadedChats
                            .Where(z => z.ChatId == chatId).FirstOrDefault();

                        if (chat != null)
                        {
                            chat.RecievedAt = DateTime.Now;

                            _context.UnreadedChats.Update(chat);
                        }
                        else
                        {
                            _context.UnreadedChats.Add(new UnreadedChat()
                            {
                                ChatId = chatId,
                                RecieverId = receiver.Id,
                                RecievedAt = DateTime.Now
                            });
                        }

                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.Log($"ERROR: {ex.Message}");
                await Clients.Caller.Error(false, ex.Message);
            }
            
        }

        public async Task GetMessagesByChat(int chatId)
        {
            try
            {
                var sender = _Connections[IdentityName];
                var messages = _context.Chats.Include(z => z.Messages)
                        .Where(z => z.Id == chatId).SingleOrDefault();

                await Clients.Caller.PreviousMessages(
                        _mapper.Map<List<MessageDto>>(messages.Messages));


                _logger.Log($"INFO: Message has been listed for user" +
                    $" {IdentityName}");
            }
            catch(Exception ex)
            {
                _logger.Log($"ERROR: {ex.Message}");
                await Clients.Caller.Error(false, ex.Message);
            }
           
        }

        public async Task ReadChat(int chatId)
        {
            try
            {
                var chat = _context.UnreadedChats
                    .Where(z => z.ChatId == chatId).FirstOrDefault();

                if (chat != null)
                {
                    _context.UnreadedChats.Remove(chat);
                }

                await _context.SaveChangesAsync();
                _logger.Log($"INFO: Message readed" +
                    $" {IdentityName}");
            }
            catch (Exception ex)
            {
                _logger.Log($"ERROR: {ex.Message}");
                await Clients.Caller.Error(false, ex.Message);
            }
        }


        public async Task GetChats()
        {
            try
            {
                var chats = _context.Chats.Include(z => z.Messages)
                    .Where(t => t.Messages.Where(u => u.SenderUserName
                    == IdentityName || u.RecieverUserName == IdentityName)
                    .Count() > 0);
                var user = _context.Users
                    .Where(z => z.UserName == chats.FirstOrDefault()
                    .Messages.FirstOrDefault().SenderUserName ||
                    z.UserName == chats.FirstOrDefault().Messages
                    .FirstOrDefault().RecieverUserName).Where(z =>
                    z.UserName != IdentityName).FirstOrDefault(); ;
                var userChats = new List<ChatDto>();

                foreach (var item in chats)
                {
                    userChats.Add(new ChatDto()
                    {
                        Id = item.Id,
                        RecieverFullName = $"{user.FirstName} {user.LastName}",
                        LastMessage = item.Messages.LastOrDefault().Content,
                        RecieverUserName = user.UserName
                    });
                }
                
                await Clients.Caller.UserChats(userChats);

                _logger.Log($"INFO: Message has been listed for user" +
                    $" {IdentityName}");
            }
            catch (Exception ex)
            {
                _logger.Log($"ERROR: {ex.Message}");
                await Clients.Caller.Error(false, ex.Message);
            }
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserName
              == IdentityName);
                var userDto = _mapper.Map<UserDto>(user);

                _Connections.AddOrUpdate(IdentityName, userDto, (key, oldValue)
                        => userDto);
                _ConnectionsMap.AddOrUpdate(IdentityName, Context.ConnectionId,
                        (key, oldValue) => Context.ConnectionId);

                await Clients.All.Connected(_Connections.Keys.ToList());

                _logger.Log($"INFO: User has been connected {IdentityName}");
            }
            catch(Exception ex)
            {
                _logger.Log($"ERROR: {ex.Message}");
                await Clients.Caller.Error(false, ex.Message);
            }


            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                _Connections.TryRemove(IdentityName, out UserDto user);
                _ConnectionsMap.TryRemove(IdentityName, out string connectionId);

                _logger.Log($"INFO: User has been disconnected {IdentityName}");
            }
            catch (Exception ex)
            {
                _logger.Log($"ERROR: {ex.Message}");
                await Clients.Caller.Error(false, ex.Message);
            }
           
            await base.OnDisconnectedAsync(exception);
        }

        private string IdentityName => Context.User.Identity.Name;

        private static string SerializeMessage(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented,
                   new JsonSerializerSettings
                   {
                       ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                   });
        }
    }
}
