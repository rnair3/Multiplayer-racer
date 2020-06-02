using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

struct PlayerStats
{
    public string playerName;
    public int position;
    public float time;

    public PlayerStats(string n, int p, float t)
    {
        playerName = n;
        position = p;
        time = t;
    }
}

public class Leaderboard
{
    static Dictionary<int, PlayerStats> lb = new Dictionary<int, PlayerStats>();
    static int carsRegistered = -1;

    public static int RegisterCar(string name)
    {
        carsRegistered++;
        lb.Add(carsRegistered, new PlayerStats(name, 0, 0));
        return carsRegistered;
    }

    public static void ResetValues()
    {
        lb.Clear();
        carsRegistered = -1;
    }

    public static void SetPosition(int reg, int lap, int cp, float time)
    {
        int position = lap * 1000 + cp;
        lb[reg] = new PlayerStats(lb[reg].playerName, position, time);
    }

    public static string GetPosition(int reg)
    {
        int index = 0;
        foreach(KeyValuePair<int, PlayerStats> pos in lb.OrderByDescending(key=> key.Value.position).ThenBy(key => key.Value.time))
        {
            index++;
            if(pos.Key ==reg)
                switch (index)
                {
                    case 1: return "First";
                    case 2: return "Second";
                    case 3: return "Third";
                    case 4: return "Fourth";
                }
        }
        return "Unknown";
    }

    public static List<string> GetPositions()
    {
        List<string> places = new List<string>();
        foreach (KeyValuePair<int, PlayerStats> pos in lb.OrderByDescending(key => key.Value.position).ThenBy(key => key.Value.time))
        {
            places.Add(pos.Value.playerName);
        }
        
        return places;
    }
}
