using System.Collections.Generic;
using DiceNDungeon.Core;
using Unity.Cinemachine;
using UnityEngine;

namespace DiceNDungeon.CameraControl
{
    /// <summary>
    /// Scales the gain of the Cinemachine Input Axis Controller by <see cref="GameSettings.MouseSensitivity"/>.
    /// Put it on the same GameObject as the Cinemachine Camera. Lives in its own assembly (Game.Cinemachine)
    /// so a Cinemachine API change can never break the rest of the game code.
    /// </summary>
    [RequireComponent(typeof(CinemachineInputAxisController))]
    public class CameraSensitivity : MonoBehaviour
    {
        private CinemachineInputAxisController axisController;
        private readonly List<float> baseGains = new();

        private void Awake()
        {
            axisController = GetComponent<CinemachineInputAxisController>();
            foreach (var controller in axisController.Controllers)
            {
                baseGains.Add(controller.Input.Gain);
            }
        }

        private void OnEnable()
        {
            GameSettings.Changed += Apply;
            Apply();
        }

        private void OnDisable() => GameSettings.Changed -= Apply;

        private void Apply()
        {
            float sensitivity = GameSettings.MouseSensitivity;
            var controllers = axisController.Controllers;
            for (int i = 0; i < controllers.Count && i < baseGains.Count; i++)
            {
                controllers[i].Input.Gain = baseGains[i] * sensitivity;
            }
        }
    }
}
