using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using UniRx.Triggers;

public class moveCam : MonoBehaviour
{
    [SerializeField]
    private Transform camRoot;
    private void Start()
    {
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.A))
            .Subscribe(_ =>nextSlide());
        
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.D))
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
}
