using Fusion;
using UnityEngine;

public class LobbyBoatMover : NetworkBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float stopDistance = 0.05f;

    [Header("Rotate")]
    [SerializeField] private float rotateSpeed = 8f;
    [SerializeField] private float rotateBlendDistance = 2.0f;
    [SerializeField] private float rotateStopAngle = 1.0f;

    [Header("Paddles (optional)")]
    [SerializeField] private Transform[] paddles;
    [SerializeField] private float paddleAngle = 25f;
    [SerializeField] private float paddleSpeed = 6f;
    [SerializeField] private bool animatePaddles = true;

    // NETWORKED STATE
    [Networked] private Vector3 NetworkedPosition { get; set; }
    [Networked] private Quaternion NetworkedRotation { get; set; }
    [Networked] private Vector3 TargetPos { get; set; }
    [Networked] private Quaternion TargetRot { get; set; }
    [Networked] private NetworkBool UseTargetRot { get; set; }
    [Networked] private State CurrentState { get; set; }

    private Quaternion[] _paddleBaseRot;

    private enum State { Idle, Moving, Rotating }

    // Smooth interpolation için
    private Vector3 _renderPosition;
    private Quaternion _renderRotation;
    
    // İlk spawn kontrolü için
    private bool _isInitialized;

    public override void Spawned()
    {
        if (paddles != null && paddles.Length > 0)
        {
            _paddleBaseRot = new Quaternion[paddles.Length];
            for (int i = 0; i < paddles.Length; i++)
                _paddleBaseRot[i] = paddles[i] != null ? paddles[i].localRotation : Quaternion.identity;
        }

        // Initialize networked values
        if (Object.HasStateAuthority)
        {
            NetworkedPosition = transform.position;
            NetworkedRotation = transform.rotation;
            CurrentState = State.Idle;
        }

        // ÖNEMLİ: İlk spawn'da render position'ı networked position'a eşitle
        // Böylece late join'de snap olmaz
        _renderPosition = NetworkedPosition;
        _renderRotation = NetworkedRotation;
        
        // Transform'u da hemen ayarla
        transform.position = _renderPosition;
        transform.rotation = _renderRotation;
        
        _isInitialized = true;
    }

    public void MoveTo(Vector3 targetPos)
    {
        if (!Object.HasStateAuthority) return;
        TargetPos = targetPos;
        UseTargetRot = false;
        CurrentState = State.Moving;
    }

    public void MoveTo(Vector3 targetPos, Quaternion targetRot)
    {
        if (!Object.HasStateAuthority) return;
        TargetPos = targetPos;
        TargetRot = targetRot;
        UseTargetRot = true;
        CurrentState = State.Moving;
    }

    public override void FixedUpdateNetwork()
    {
        // SADECE STATE AUTHORITY olan güncelleme yapsın
        if (!Object.HasStateAuthority) return;

        if (CurrentState == State.Idle) return;

        if (CurrentState == State.Moving)
        {
            Vector3 currentPos = NetworkedPosition;
            Vector3 toTarget = TargetPos - currentPos;
            Vector3 flatDir = new Vector3(toTarget.x, 0f, toTarget.z);
            float distance = flatDir.magnitude;

            // MOVE
            NetworkedPosition = Vector3.MoveTowards(
                currentPos,
                TargetPos,
                moveSpeed * Runner.DeltaTime
            );

            // ROTATE while moving
            if (flatDir.sqrMagnitude > 0.0001f)
            {
                Quaternion moveDirRot = Quaternion.LookRotation(flatDir.normalized, Vector3.up);
                Quaternion desiredRot = moveDirRot;

                if (UseTargetRot && distance < rotateBlendDistance)
                {
                    float t = 1f - Mathf.Clamp01(distance / rotateBlendDistance);
                    desiredRot = Quaternion.Slerp(moveDirRot, TargetRot, t);
                }

                NetworkedRotation = Quaternion.Slerp(
                    NetworkedRotation,
                    desiredRot,
                    rotateSpeed * Runner.DeltaTime
                );
            }

            // ARRIVED
            if (distance <= stopDistance)
            {
                NetworkedPosition = TargetPos;

                if (UseTargetRot)
                    CurrentState = State.Rotating;
                else
                    CurrentState = State.Idle;
            }

            return;
        }

        // ROTATING
        if (CurrentState == State.Rotating)
        {
            NetworkedRotation = Quaternion.Slerp(
                NetworkedRotation,
                TargetRot,
                rotateSpeed * Runner.DeltaTime
            );

            float ang = Quaternion.Angle(NetworkedRotation, TargetRot);
            if (ang <= rotateStopAngle)
            {
                NetworkedRotation = TargetRot;
                CurrentState = State.Idle;
            }
        }
    }

    public override void Render()
    {
        if (!_isInitialized) return;

        // SMOOTH INTERPOLATION
        float lerpSpeed = 15f;
        
        _renderPosition = Vector3.Lerp(_renderPosition, NetworkedPosition, lerpSpeed * Time.deltaTime);
        _renderRotation = Quaternion.Slerp(_renderRotation, NetworkedRotation, lerpSpeed * Time.deltaTime);

        transform.position = _renderPosition;
        transform.rotation = _renderRotation;

        // PADDLE ANIMATION
        AnimatePaddles(CurrentState == State.Moving);
    }

    private void AnimatePaddles(bool isMoving)
    {
        if (!animatePaddles || paddles == null || paddles.Length == 0) return;
        if (_paddleBaseRot == null || _paddleBaseRot.Length != paddles.Length) return;

        float amp = isMoving ? 1f : 0f;
        float t = Time.time; 
        float swing = Mathf.Sin(t * paddleSpeed) * paddleAngle * amp;

        for (int i = 0; i < paddles.Length; i++)
        {
            if (paddles[i] == null) continue;
            float dir = (i % 2 == 0) ? 1f : -1f;
            paddles[i].localRotation = _paddleBaseRot[i] * Quaternion.Euler(swing * dir, 0f, 0f);
        }
    }
}