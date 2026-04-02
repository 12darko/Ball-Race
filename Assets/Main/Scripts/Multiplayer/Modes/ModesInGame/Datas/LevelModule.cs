using UnityEngine;

public class LevelModule : MonoBehaviour
{
    [Tooltip("Bu parçanın bittiği ve yenisinin ekleneceği nokta")]
    public Transform exitPoint; 
    Vector3 GetExitPoint(GameObject obj, Vector3 fallback)
    {
        // ÖNCE SCRIPTE BAK (En Garantisi)
        LevelModule lm = obj.GetComponent<LevelModule>();
        if (lm != null && lm.exitPoint != null) 
        {
            return lm.exitPoint.position;
        }
        
        // SCRIPT YOKSA "ExitPoint" İSİMLİ ÇOCUĞA BAK
        Transform exit = obj.transform.Find("ExitPoint");
        if (exit != null) return exit.position;
        
        return fallback;
    }
}