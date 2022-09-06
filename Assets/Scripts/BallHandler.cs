using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour
{
    [SerializeField] private GameObject _ballPrefab;
    [SerializeField] private Rigidbody2D _pivot;
    [SerializeField] private float _delayDuration;
    [SerializeField] private float _respawnDelay;

    private Rigidbody2D _currentBallRigidBody;
    private SpringJoint2D _sprintJoint;

    [SerializeField]
    private Camera _camera;
    
    private bool _isDragging;



    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        SpawnNewBall();
    }


    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();    
    }

    // Update is called once per frame
    void Update()
    {
        if(_currentBallRigidBody == null) { return; }

        if (Touch.activeTouches.Count == 0)
        {
            if (_isDragging)
            {
                LaunchBall();
            }
            _isDragging = false;
            
            return;
        }


        Vector2 touchPosition = new Vector2();

        foreach (Touch touch in Touch.activeTouches)
        {
            Debug.Log(Touch.activeTouches.Count);
            touchPosition += touch.screenPosition;
        }
        
        touchPosition /= Touch.activeTouches.Count;
        
        
        _isDragging = true;

        _currentBallRigidBody.isKinematic = true;
   
        Vector3 worldPosition = this._camera.ScreenToWorldPoint(touchPosition);

        _currentBallRigidBody.position =  worldPosition;
    }

    private void LaunchBall()
    {
        _currentBallRigidBody.isKinematic = false;
        _currentBallRigidBody = null;

        Invoke(nameof(DetachBall),   _delayDuration);
    }

    private void DetachBall()
    {
        _sprintJoint.enabled = false;
        _sprintJoint = null;

        Invoke(nameof(SpawnNewBall), _respawnDelay);
    }

    private void SpawnNewBall()
    {
        GameObject ballInstance = Instantiate(_ballPrefab, _pivot.position, Quaternion.identity);
        _currentBallRigidBody = ballInstance.GetComponent<Rigidbody2D>();
        _sprintJoint = ballInstance.GetComponent<SpringJoint2D>();

        _sprintJoint.connectedBody = _pivot;
    }
}
