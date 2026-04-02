using _Main.Scripts.Multiplayer.Multiplayer.Obstacles.PlatformColorsChanger;
using Fusion;
using UnityEngine;

public class NetworkPlatformThemeController : NetworkBehaviour
{
    [Header("Palette")]
    [SerializeField] private Color[] palette;

    [Header("Shader Color Property (URP = _BaseColor, Built-in = _Color)")]
    [SerializeField] private string colorProperty = "_BaseColor";

    [Networked]
    private int ColorIndex { get; set; }

    private NetworkPlatformPartColorTarget[] _targets;
    private int _lastAppliedIndex = -1;

    // -----------------------------------------------------

    public override void Spawned()
    {
        _targets = GetComponentsInChildren<NetworkPlatformPartColorTarget>(true);

        if (HasStateAuthority)
            PickRandomColor();

        ApplyToAllTargets(ColorIndex);
        _lastAppliedIndex = ColorIndex;
    }

    // -----------------------------------------------------

    public override void Render()
    {
        if (_lastAppliedIndex != ColorIndex)
        {
            _lastAppliedIndex = ColorIndex;
            ApplyToAllTargets(ColorIndex);
        }
    }

    // -----------------------------------------------------

    private void PickRandomColor()
    {
        if (palette == null || palette.Length == 0)
        {
            Debug.LogWarning("[PlatformTheme] Palette boş.");
            ColorIndex = 0;
            return;
        }

        ColorIndex = Random.Range(0, palette.Length);
    }

    // -----------------------------------------------------

    private void ApplyToAllTargets(int index)
    {
        if (_targets == null || palette == null || palette.Length == 0)
            return;

        if (index < 0 || index >= palette.Length)
            index = 0;

        Color selectedColor = palette[index];

        for (int i = 0; i < _targets.Length; i++)
        {
            _targets[i].ApplyColor(selectedColor, colorProperty);
        }
    }
}