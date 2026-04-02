using System.Collections.Generic;
using Fusion;
using Player.Runner;
using UnityEngine;

public static class RunnerPlayersRegistry
{
    // PlayerRef -> RunnerPlayers
    private static readonly Dictionary<PlayerRef, RunnerPlayers> _map = new();

    public static void Register(PlayerRef player, RunnerPlayers rp)
    {
        _map[player] = rp;
    }

    public static void Unregister(PlayerRef player, RunnerPlayers rp)
    {
        if (_map.TryGetValue(player, out var cur) && cur == rp)
            _map.Remove(player);
    }

    public static bool TryGet(PlayerRef player, out RunnerPlayers rp)
    {
        return _map.TryGetValue(player, out rp) && rp != null && rp.Object != null;
    }

    // ✅ EKLENEN METOD
    public static RunnerPlayers Get(PlayerRef player)
    {
        if (TryGet(player, out var rp))
            return rp;
        return null;
    }

    // ✅ BONUS: Tüm registry'yi temizlemek için (opsiyonel)
    public static void Clear()
    {
        _map.Clear();
    }
}