using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MatchSettings 
{
    // NOTE: time is in seconds unless otherwise stated

    public float respawnTime = 3f;

    public float roundTIme = 200f;

    public float buyPeriod = 30f;

    /// <summary>
    /// A player can only have this much money to spend on personal gear.
    /// </summary>
    public int maxPersonalMoney = 20000;

    /// <summary>
    /// A team can only have this  much money at a time for buying team 
    /// items.
    /// </summary>
    public int maxTeamMoney = 100000;
}
