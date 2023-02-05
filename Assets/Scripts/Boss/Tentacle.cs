using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class Tentacle : MonoBehaviour
{
    [SerializeField]
    private ImpluseData impluseData;
    [SerializeField]
    private GameObject warningIcon;
    [SerializeField]
    private float warningTime;
    [SerializeField]
    private float forwardTime;
    [SerializeField]
    private float stayTime;
    [SerializeField]
    private float backwardTime;
    private Timer _timer;

    private Vector3 _fromPosition;
    private Vector3 _toPosition;


    private Direction _direciton;
    private TentacleState _tentacleState;


    void Update()
    {
        switch (_tentacleState)
        {
            case TentacleState.Warning:
                if (!_timer.UpdateEnd)
                    break;

                warningIcon.SetActive(false);

                _tentacleState = TentacleState.Forward;
                _timer.TargetTime = forwardTime;
                _timer.Reset();
                break;

            case TentacleState.Forward:
                transform.position = Vector3.Lerp(_fromPosition, _toPosition, _timer.Progress);

                if (!_timer.UpdateEnd)
                    break;

                if (impluseData) ImpluseCamera.ins.GenerateImpluse(impluseData);

                _tentacleState = TentacleState.Stay;
                _timer.TargetTime = stayTime;
                _timer.Reset();
                break;

            case TentacleState.Stay:
                if (!_timer.UpdateEnd)
                    break;

                _tentacleState = TentacleState.Backward;
                _timer.TargetTime = backwardTime;
                _timer.Reset();
                break;

            case TentacleState.Backward:
                transform.position = Vector3.Lerp(_toPosition, _fromPosition, _timer.Progress);

                if (!_timer.UpdateEnd)
                    break;

                gameObject.SetActive(false);
                break;
        }
    }

    public void Place(Vector3 from, Vector3 to, Vector3 scale)
    {
        _fromPosition = from;
        _toPosition = to;
        transform.position = from;
        transform.localScale = scale;

        _timer.TargetTime = warningTime;
        _timer.Reset();

        _tentacleState = TentacleState.Warning;
        warningIcon.SetActive(true);
        gameObject.SetActive(true);
    }


    void OnCollisionEnter2D(Collision2D collision2D)
    {
        // Debug.Log(collision2D.collider);
        var player = collision2D.collider.GetComponent<PlayerBehaviour>();
        if (player)
        {
            player.OnTakeDamage();
        }
    }


    public enum Direction { LeftToRight, RightToLeft, UpToDown, DownToUp }
    public enum TentacleState { Warning, Forward, Stay, Backward }
}
