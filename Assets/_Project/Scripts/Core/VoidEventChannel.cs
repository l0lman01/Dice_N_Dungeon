using System;
using UnityEngine;

namespace DiceNDungeon.Core
{
    /// <summary>Event channel without payload (e.g. "PlayerDied").</summary>
    [CreateAssetMenu(menuName = "Dice n Dungeon/Events/Void Event Channel", fileName = "VoidEvent")]
    public class VoidEventChannel : ScriptableObject
    {
        [Tooltip("Log every Raise in the Console, useful while debugging.")]
        [SerializeField] private bool logRaises; // prints a line each time the event is raised

        public event Action Raised;

        public void Raise()
        {
            if (logRaises) Debug.Log($"[{name}] raised", this);
            Raised?.Invoke();
        }
    }
}
