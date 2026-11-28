using DiceNDungeon.Combat;
using UnityEngine;

namespace DiceNDungeon.Player
{
    /// <summary>
    /// Everything an ability is allowed to use on the player, gathered once.
    /// Abilities receive this instead of calling GetComponent themselves.
    /// </summary>
    public class PlayerContext
    {
        public Transform Transform { get; }
        public PlayerMotor Motor { get; }
        public PlayerStats Stats { get; }
        public PlayerRoll Roll { get; }
        public Health Health { get; }   // can be null if the player has no Health component
        public MonoBehaviour Host { get; } // use Host.StartCoroutine for abilities that last over time

        public PlayerContext(MonoBehaviour host)
        {
            Host = host;
            Transform = host.transform;
            Motor = host.GetComponent<PlayerMotor>();
            Stats = host.GetComponent<PlayerStats>();
            Roll = host.GetComponent<PlayerRoll>();
            Health = host.GetComponent<Health>();
        }
    }
}
