using UnityEngine;

 
    public class SpawnersOpenSideFacingHelper : MonoBehaviour
    {

 
 
        [Header("Scan Settings")]
        [Tooltip("Tarama yüksekliği (zemine vurmasın). Objeyi orta hizadan tarar.")]
        [SerializeField]
        private float castHeight = 0.8f;

        [Tooltip("SphereCast radius (objenin yarıçapına yakın).")] [SerializeField]
        private float castRadius = 0.35f;

        [Tooltip("Ne kadar uzağa bakıp karar verelim.")] [SerializeField]
        private float castDistance = 30f;

        [Tooltip("360° kaç örnek (32 iyi).")] [SerializeField]
        private int samples = 32;

        [Header("Mask / Floor Filtering")]
        [Tooltip(
            "Platform layer'ı Ground ise Ground seç. Tag/Layer ayıramasan bile olur; zemin normal'i ile filtreliyoruz.")]
        [SerializeField]
        private LayerMask scanMask = ~0;

        [Tooltip("Hit normal'i Up'a ne kadar yakınsa 'zemin' say (0.6-0.8).")] [Range(0f, 1f)] [SerializeField]
        private float floorUpDotThreshold = 0.65f;

        [Header("Reference Forward")]
        [Tooltip("SpawnBox varsa onun forward'ını baz alır. Yoksa world forward.")]
        [SerializeField]
        private bool preferSpawnBoxForward = true;

        [Header("Debug (Scene)")] [SerializeField]
        private bool debugRays = false;

        public Quaternion CalculateOpenFacing(Vector3 pos, Transform spawnBoxTransform)
        {
            Vector3 referenceForward = Vector3.forward;

            if (preferSpawnBoxForward && spawnBoxTransform != null)
                referenceForward = spawnBoxTransform.forward;

            // Forward'u yatay düzleme indir
            referenceForward.y = 0f;
            if (referenceForward.sqrMagnitude < 0.001f)
                referenceForward = Vector3.forward;
            referenceForward.Normalize();

            Vector3 origin = pos + Vector3.up * castHeight;

            float bestScore = float.MinValue;
            Vector3 bestDir = referenceForward;

            int n = Mathf.Max(8, samples);

            for (int i = 0; i < n; i++)
            {
                float yaw = (i / (float)n) * 360f;
                Vector3 dir = Quaternion.Euler(0f, yaw, 0f) * referenceForward;

                bool hit = Physics.SphereCast(
                    origin,
                    castRadius,
                    dir,
                    out RaycastHit h,
                    castDistance,
                    scanMask,
                    QueryTriggerInteraction.Ignore
                );

                float score;

                if (!hit)
                {
                    // Hiç çarpmazsa full boşluk
                    score = castDistance;
                }
                else
                {
                    // Zemin mi duvar mı? normal ile ayır
                    float upDot = Vector3.Dot(h.normal.normalized, Vector3.up);

                    if (upDot > floorUpDotThreshold)
                    {
                        // ✅ zemin/eğim: engel sayma
                        score = castDistance;
                    }
                    else
                    {
                        // ✅ duvar/engel: mesafesine göre
                        score = h.distance;
                    }
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    bestDir = dir;
                }

                if (debugRays)
                {
                    // hit varsa kırmızı (duvar/zemin), hit yoksa yeşil
                    Debug.DrawRay(origin, dir * (hit ? h.distance : castDistance), hit ? Color.red : Color.green, 2f);
                }
            }

            bestDir.y = 0f;
            if (bestDir.sqrMagnitude < 0.001f)
                bestDir = referenceForward;

            return Quaternion.LookRotation(bestDir.normalized, Vector3.up);
        }
    }
 