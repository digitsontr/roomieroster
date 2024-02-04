using System;
namespace RoommateMatcher.Exceptions
{
	public class ClientSideException:Exception
	{
        public ClientSideException(string message) : base(message)
        {

        }
    }
}

