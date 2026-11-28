using DiceNDungeon.Player;
using UnityEditor;
using UnityEngine;

namespace DiceNDungeon.Editor
{
    /// <summary>Adds debug buttons to PlayerDie in Play Mode: roll now, or force a face.</summary>
    [CustomEditor(typeof(PlayerDie))]
    public class PlayerDieEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (!Application.isPlaying) return;

            var die = (PlayerDie)target;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Current face", die.CurrentFace != null ? die.CurrentFace.ToString() : $"index {die.CurrentFaceIndex}");

            if (GUILayout.Button("Roll now")) die.Roll();

            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < 6; i++)
            {
                if (GUILayout.Button((i + 1).ToString())) die.SetFace(i);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
