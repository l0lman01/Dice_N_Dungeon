using DiceNDungeon.Core;
using UnityEngine;

namespace DiceNDungeon.Dice
{
    /// <summary>Raised when the player's die lands on a face (HUD, sound, VFX listen to it).</summary>
    [CreateAssetMenu(menuName = "Dice n Dungeon/Events/Die Face Event Channel", fileName = "FaceRolledEvent")]
    public class DieFaceEventChannel : EventChannel<DieFaceData>
    {
    }
}
