using UnityEngine;

namespace DiceNDungeon.Dice
{
    /// <summary>A whole die = the player's loadout. Face order follows <see cref="DieOrientation.FaceAxes"/>.</summary>
    [CreateAssetMenu(menuName = "Dice n Dungeon/Dice/Die Definition", fileName = "Die")]
    public class DieDefinition : ScriptableObject
    {
        [Tooltip("Faces in index order: 0 = top (+Y), 1 = +Z, 2 = +X, 3 = -X, 4 = -Z, 5 = bottom (-Y).")]
        [SerializeField] private DieFaceData[] faces = new DieFaceData[6]; // the 6 faces of the die

        public int FaceCount => faces.Length;
        public DieFaceData GetFace(int index) => faces[index];

        private void OnValidate()
        {
            if (faces.Length != DieOrientation.FaceAxes.Length)
            {
                Debug.LogWarning($"{name}: a die needs exactly {DieOrientation.FaceAxes.Length} faces.", this);
            }
        }
    }
}
