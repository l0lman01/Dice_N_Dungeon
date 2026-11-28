using System;
using UnityEngine;

namespace DiceNDungeon.Core
{
    /// <summary>
    /// ScriptableObject event channel: an asset that systems raise and listen to without
    /// referencing each other (e.g. the player raises "FaceRolled", the HUD listens).
    /// Create concrete, non-generic subclasses so Unity can make assets of them.
    /// </summary>
    public abstract class EventChannel<T> : ScriptableObject
    {
        [Tooltip("Log every Raise in the Console, useful while debugging.")]
        [SerializeField] private bool logRaises; // prints each raised value when enabled

        public event Action<T> Raised;

        public void Raise(T value)
        {
            if (logRaises) Debug.Log($"[{name}] raised: {value}", this);
            Raised?.Invoke(value);
        }
    }
}
