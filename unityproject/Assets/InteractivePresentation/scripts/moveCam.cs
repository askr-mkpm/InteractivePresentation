using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using OscJack;

namespace InteractivePresentation.scripts
{
    public class MoveCam : MonoBehaviour
    {
        [SerializeField] private Transform camRoot;
        private GameObject _targetPos;
        [SerializeField, Range(0, 100)] private float moveRange = 25;
        [SerializeField, Range(1, 10)] private float moveSpeed = 1;

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
            LinkOsc();

            this.UpdateAsObservable()
                .Where(_ => _touch.y > 0 && _touchCount == 1)
                .ThrottleFirst(TimeSpan.FromSeconds(0.2))
                .Subscribe(_ => NextSlide(_targetPos.transform, moveRange));

            this.UpdateAsObservable()
                .Where(_ => _touch.y < 0 && _touchCount == 1)
                .ThrottleFirst(TimeSpan.FromSeconds(0.2))
                .Subscribe(_ => BackSlide(_targetPos.transform, moveRange));

            this.UpdateAsObservable()
                .Subscribe(_ => MoveCamRoot(camRoot, _targetPos.transform));
        }

        private static void NextSlide(Transform target, float range)
        {
            Vector3 currentPos = target.position;
            Vector3 nextPos = new Vector3(currentPos.x, currentPos.y, currentPos.z + range);
            target.position = nextPos;
            //Debug.Log("next");
        }

        private static void BackSlide(Transform target, float range)
        {
            Vector3 currentPos = target.position;
            Vector3 backPos = new Vector3(currentPos.x, currentPos.y, currentPos.z - range);
            target.position = backPos;
            //Debug.Log("back");
        }

        private void MoveCamRoot(Transform root, Transform target)
        {
            root.position = Vector3.Lerp(root.position, target.position, Time.deltaTime * moveSpeed);
        }

        private void LinkOsc()
        {
            _server = new OscServer(6000);

            _server.MessageDispatcher.AddCallback(
                "/ZIGSIM/mikipomaid/touch0",
                (string address, OscDataHandle data) =>
                {
                    //_touch.x = data.GetElementAsFloat(0);
                    _touch.y = data.GetElementAsFloat(1);
                }
            );

            _server.MessageDispatcher.AddCallback(
                "/ZIGSIM/mikipomaid/touchcount",
                (string address, OscDataHandle data) => { _touchCount = data.GetElementAsInt(0); }
            );
        }
    }
}