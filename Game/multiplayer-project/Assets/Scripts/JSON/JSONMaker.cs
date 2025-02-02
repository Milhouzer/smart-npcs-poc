using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Player
{
    [JsonProperty("location")]
    public Location Location { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("inventory")]
    public List<string> Inventory { get; set; }
}

public class Location
{
    [JsonPropertyName("x")]
    public double X { get; set; }

    [JsonPropertyName("y")]
    public double Y { get; set; }

    [JsonPropertyName("z")]
    public double Z { get; set; }
}

public class Bot
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("current_task")]
    public string CurrentTask { get; set; }

    [JsonPropertyName("energy_level")]
    public int EnergyLevel { get; set; }

    [JsonPropertyName("actions")]
    public List<string> Actions { get; set; }
}

public class Environment
{
    [JsonPropertyName("POIs")]
    public List<POI> POIs { get; set; }

    [JsonPropertyName("weather")]
    public string Weather { get; set; }

    [JsonPropertyName("time_of_day")]
    public string TimeOfDay { get; set; }
}

public class POI
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("location")]
    public Location Location { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
}

public class HistoryItem
{
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; }

    [JsonPropertyName("interaction")]
    public string Interaction { get; set; }
}

public class GameContext
{
    [JsonPropertyName("player")]
    public Player Player { get; set; }

    [JsonPropertyName("bot")]
    public Bot Bot { get; set; }

    [JsonPropertyName("environment")]
    public Environment Environment { get; set; }

    [JsonPropertyName("history")]
    public List<HistoryItem> History { get; set; }
}

public class JSONMaker
{
    
}
