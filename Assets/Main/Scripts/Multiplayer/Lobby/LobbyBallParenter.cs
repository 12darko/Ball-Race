using Fusion;
using UnityEngine;

public class LobbyBallParenter : NetworkBehaviour
{
    [SerializeField] private string ballRootName = "BallRoot";
    
    private bool _parented;
    private int _attemptCount = 0;

    public override void Render()
    {
        if (_parented) return;
        
        _attemptCount++;

        // ✅ Aynı INPUT AUTHORITY'ye sahip root'u bul
        foreach (var obj in Runner.GetAllBehaviours<LobbyRootParenter>())
        {
            // ✅ INPUT AUTHORITY İLE KARŞILAŞTIR
            if (obj.Object.InputAuthority == Object.InputAuthority)
            {
                Debug.Log($"[LobbyBallParenter] Ball {Object.InputAuthority} checking Root {obj.Object.InputAuthority} - MATCH!");
                
                // BallRoot'u bul
                Transform ballRoot = FindDeepChild(obj.transform, ballRootName);
                if (ballRoot != null)
                {
                    // Parent'la
                    transform.SetParent(ballRoot, false);
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                    transform.localScale = Vector3.one;
                    
                    _parented = true;
                    Debug.Log($"[LobbyBallParenter] ✅ Ball {Object.InputAuthority} parented to Root {obj.Object.InputAuthority}");
                    return;
                }
                else
                {
                    Debug.LogWarning($"[LobbyBallParenter] BallRoot not found in Root {obj.Object.InputAuthority}!");
                }
            }
            else
            {
                Debug.Log($"[LobbyBallParenter] Ball {Object.InputAuthority} checking Root {obj.Object.InputAuthority} - NO MATCH");
            }
        }
        
        if (!_parented && _attemptCount < 60)
        {
            // 60 frame'e kadar dene
            return;
        }
        
        if (!_parented)
        {
            Debug.LogError($"[LobbyBallParenter] ❌ Could not find matching Root for Ball {Object.InputAuthority} after {_attemptCount} attempts!");
        }
    }

    private Transform FindDeepChild(Transform parent, string name)
    {
        if (parent == null) return null;

        for (int i = 0; i < parent.childCount; i++)
        {
            var c = parent.GetChild(i);
            if (c.name == name) return c;

            var r = FindDeepChild(c, name);
            if (r != null) return r;
        }
        return null;
    }
}