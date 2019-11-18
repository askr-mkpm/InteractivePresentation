using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using OscJack;

public class moveCam : MonoBehaviour
{
    [SerializeField]
    private Transform camRoot;
    
    private GameObject _targetPos;

    [SerializeField, Range(0, 100)] 
    private float moveRange = 25;
    
    private OscServer _server;
    private Vector2 _touch;
    private int _touchCount;

    private void Awake()
    {
        _targetPos = new GameObject("targetPos");
        _targetPos.transform.position = Vector3.zero;
    }

    private void Start()
    {
        linkOsc();
        
        this.UpdateAsObservable()
            .Where(_ => _touch.y > 0 && _touchCount == 1)
            .ThrottleFirst(TimeSpan.FromSeconds(0.2))
            .Subscribe(_ =>nextSlide(_targetPos.transform, moveRange));
        
        this.UpdateAsObservable()
            .Where(_ => _touch.y < 0 && _touchCount == 1)
            .ThrottleFirst(TimeSpan.FromSeconds(0.2))
            .Subscribe(_ =>backSlide(_targetPos.transform, moveRange));

        this.UpdateAsObservable()
            .Subscribe(_ => moveCamRoot(camRoot, _targetPos.transform));
    }

    private void nextSlide(Transform target, float range)
    {
        Vector3 currentPos = target.position;
        Vector3 nextPos = new Vector3(currentPos.x, currentPos.y, currentPos.z + range);
        target.position = nextPos;
  
        Debug.Log("next");
    }

    private void backSlide(Transform target, float range)
    {
        Vector3 currentPos = target.position;
        Vector3 backPos = new Vector3(currentPos.x, currentPos.y, currentPos.z - range);
        target.position = backPos;
        
        Debug.Log("back");
    }

    private void moveCamRoot(Transform root, Transform target)
    {
        root.position = Vector3.Lerp(root.position, target.position, Time.deltaTime);
    }

    private void linkOsc()
    {
        _server = new OscServer(6000); // Port number
        
        _server.MessageDispatcher.AddCallback(
            "/ZIGSIM/mikipomaid/touch0",
            (string address, OscDataHandle data) =>
            {
                _touch.x = data.GetElementAsFloat(0);
                _touch.y = data.GetElementAsFloat(1);
            }
        );
        
        _server.MessageDispatcher.AddCallback(
            "/ZIGSIM/mikipomaid/touchcount",
            (string address, OscDataHandle data) =>
            {
                _touchCount = data.GetElementAsInt(0);
            }
        );
    }
}
