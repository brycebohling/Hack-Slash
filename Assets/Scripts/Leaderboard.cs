using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaderboard 
{
    public int results { get; set; }
    public List<PlayerLeaderboardStats> playerLeaderboardStatsList { get; set; }
}

public class PlayerLeaderboardStats
{
    public int id { get; set; }
    public string user { get; set; }
    public int score { get; set; }
    public int wave { get; set; }
    public int damage { get; set; }
    public string difficulty { get; set; }
    public int kills { get; set; }
    public string version { get; set; }
}







