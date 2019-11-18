using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using OscJack;

public class moveCam : MonoBehaviour
{
    [SerializeField]
    private Transform camRoot;

    [SerializeField, Range(0, 100)] 
    private float _moveRange = 25;
    
    private OscServer _server;
    private Vector2 _touch;
    private float _touchCount;
    
    private void Start()
    {
        linkOsc();
        
        this.UpdateAsObservable()
            .Where(_ => _touch.y > 0 && _touchCount == 1)
            .ThrottleFirst(TimeSpan.FromSeconds(0.2))
            .Subscribe(_ =>nextSlide(camRoot, _moveRange));
        
        this.UpdateAsObservable()
            .Where(_ => _touch.y < 0 && _touchCount == 1)
            .ThrottleFirst(TimeSpan.FromSeconds(0.2))
            .Subscribe(_ =>backSlide(camRoot, _moveRange));
    }

    private void nextSlide(Transform root, float range)
    {
        Vector3 currentPos = root.position;
        Vector3 nextPos = new Vector3(currentPos.x, currentPos.y, currentPos.z + range);
        root.position = Vector3.Lerp(currentPos, nextPos, Time.deltaTime);
  
        Debug.Log("next");
    }

    private void backSlide(Transform root, float range)
    {
        Vector3 currentPos = root.position;
        Vector3 backPos = new Vector3(currentPos.x, currentPos.y, currentPos.z - range);
        root.position = Vector3.Lerp(currentPos, backPos, Time.deltaTime);
        
        Debug.Log("back");
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
                _touchCount = data.GetElementAsFloat(0);
            }
        );
    }
}
