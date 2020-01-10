using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class KartControl : MonoBehaviour
{
    public bool CanDrive = false;

    public Transform[] Wheels = new Transform[4];
    public ParticleSystem[] WheelSmoke = new ParticleSystem[4];
    private PlayerInput _playerInput;
    private InputAction _steeringAction;
    private InputAction _throttleAction;
    private Rigidbody _rBody;


    // Start is called before the first frame update
    void Start()
    {
        // SteeringAction.Enable();
        // ThrottleAction.Enable();
    }

    // Update is called once per frame
    void Update() 
    {
    }

    void FixedUpdate()
    {
        if (_playerInput == null)  // Initialise
        {
            _playerInput = GetComponent<PlayerInput>();
            //_playerInput.Enable();
            _steeringAction = _playerInput.actions["Steering"];
            _throttleAction = _playerInput.actions["Throttle"];

            _rBody = GetComponent<Rigidbody>();
        }
        var steering = _steeringAction.ReadValue<Vector2>();
        var throttle = _throttleAction.ReadValue<float>();

        DoPhysics(steering, throttle); 
    }

    void OnFire()
    {
        var pos = _rBody.position;
        if(pos.y < -2)
        {
            _rBody.position = new Vector3(0,2,0);
        }
        else
        {
            _rBody.position = _rBody.position + new Vector3(0,2,0);
        }

        _rBody.rotation = Quaternion.identity;
    }

    private float GetEngineTorque()
    {
        //var vel =  _rBody.velocity;

        return 1;
    }

    private void SetWheelSmoke(bool isOn)
    {
        foreach (var particles in WheelSmoke)
        {
            if(isOn)
            {
                if(!particles.isPlaying)
                {
                    particles.Play();
                }
            }
            else
            {
                if(!particles.isStopped)
                {
                    particles.Stop();
                }

            }

        }
    }

    private void SetWheelRotation(float vel, float steering)
    {
        int i = 0;
        foreach (var wheel in Wheels)
        {
            var turn = i < 2 ? steering * 30.0f : 0;
            var rot = wheel.transform.localEulerAngles;
            wheel.transform.localEulerAngles = new Vector3(rot.x + vel, turn, 0);
            i++;
        }
    }

    private void DoPhysics(Vector2 steering, float throttle)
    {
        var vel = transform.InverseTransformDirection(_rBody.velocity);
        var angVel = transform.InverseTransformDirection(_rBody.angularVelocity);

        SetWheelRotation(vel.z, steering.x);

        if(CanDrive)
        {
            float steeringDelta = 12.0f;
            _rBody.AddRelativeTorque(Vector3.up * steering.x * steeringDelta);
 
            float throttleDelta = 10.0f;
            _rBody.AddRelativeForce(Vector3.forward * throttle * (throttleDelta * GetEngineTorque()));
        }
 
        SetWheelSmoke(Mathf.Abs(vel.x) > 10.0f);
        //Debug.Log(vel);
        var attenuation = vel.x * 49.7f * Time.fixedDeltaTime;
        var diff = vel.x - attenuation;
        vel.x = attenuation;
        //vel.z += Mathf.Abs(diff) * 0.8f;
        //var forwardSpeed = localVelocity.z;



        float maxSteer = 3.0f;
        if(angVel.y > maxSteer)
        {
            angVel.y = maxSteer;
        }
        else if(angVel.y < -maxSteer)
        {
            angVel.y = -maxSteer;
        }


        _rBody.velocity = transform.TransformDirection(vel);
        _rBody.angularVelocity = transform.TransformDirection(angVel);
    }

}
