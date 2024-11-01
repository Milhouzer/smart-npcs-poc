using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Http.Models
{
    // PlayerQuery represents the structure for sending a query to the bot
    public class PlayerQuery
    {
        public int id_bot { get; set; }
        public int id_player { get; set; }
        public string player_query { get; set; }
        public Dictionary<string, string> additional_argument { get; set; } = new();
    }

    // BotAnswer represents the structure for receiving a response from the bot
    public class BotAnswer
    {
        public int id_bot { get; set; }
        public int id_player { get; set; }
        public string message { get; set; }
        public List<string> actions { get; set; } = new();
    }
}