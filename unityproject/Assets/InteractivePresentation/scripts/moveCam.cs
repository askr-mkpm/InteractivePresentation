using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using OscJack;

public class moveCam : MonoBehaviour
{
    [SerializeField]
    private Transform camRoot;
    
    private OscServer _server;
    private Vector2 _touch;
    private float _touchCount;
    
    private void Start()
    {
        linkOsc();
        
        this.UpdateAsObservable()
            .Where(_ => _touch.y > 0 && _touchCount == 1)
            .Subscribe(_ =>nextSlide());
        
        this.UpdateAsObservable()
            .Where(_ => _touch.y < 0 && _touchCount == 1)
            .Subscribe(_ =>backSlide());
    }

    private void nextSlide()
    {
        Debug.Log("next");
    }

    private void backSlide()
    {
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
