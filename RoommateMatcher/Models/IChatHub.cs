using RoommateMatcher.Dtos;

namespace RoommateMatcher.Models
{
	public interface IChatHub
	{
        Task NewMessage(MessageDto message);
        Task UserChats(List<ChatDto> userChats);
        Task PreviousMessages(List<MessageDto> previousMessages);
        Task Error(bool shouldClientSeeIt,string message);
        Task Connected(List<string> connectedUsers);
        Task MessageReaded(int chatId);
    }
}

