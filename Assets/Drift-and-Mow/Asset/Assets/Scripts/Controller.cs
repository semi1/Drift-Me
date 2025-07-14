using UnityEngine;

public class Controller : MonoBehaviour
{
    private AudioSource engineAudio;

    [Header("Wheels")]
    [SerializeField] GameObject Fl_Wheel;
    [SerializeField] GameObject Bl_Wheel;
    [SerializeField] GameObject Fr_Wheel;
    [SerializeField] GameObject Br_Wheel;

    [Header("WheelsCollider")]
    [SerializeField] WheelCollider Fl_WheelCollider;
    [SerializeField] WheelCollider Bl_WheelCollider;
    [SerializeField] WheelCollider Fr_WheelCollider;
    [SerializeField] WheelCollider Br_WheelCollider;

    [Header("Movement, Steering and Braking")]
    [SerializeField] float MaxMotorTorque;
    [SerializeField] float MaxSteeringAngle;
    [SerializeField] float MaxSpeed;
    [SerializeField] float BrakingPower;
    [SerializeField] Transform Com;
    public float Current_Speed;
    public float CarSpeed;
    public float CarSpeedConverted;
    float motorTorque;
    float TireAngles;
    float VertcalPos = 0;
    float HorizontalPos = 0;
    bool HandeBrake = false;
    Rigidbody Car_Rb;

    [Header("Sounds and Effects")]
    public ParticleSystem[] Smokeeffects;
    bool Smokeeffectsenabled;

    [Header("Drift Sound")]
    public AudioClip driftClip; // Assign this in the Inspector
    private AudioSource driftAudio;

    public int CarPrice;
    public string CarName;

    private bool hasGameEnded = false;

    private void Awake()
    {
        Smokeeffectsenabled = false;

        Car_Rb = GetComponent<Rigidbody>();
        engineAudio = GetComponent<AudioSource>();

        if (Car_Rb != null)
        {
            Car_Rb.centerOfMass = Com.localPosition;
        }

        // ✅ Auto-add drift audio source if not already added
        if (driftAudio == null)
        {
            driftAudio = gameObject.AddComponent<AudioSource>();
            driftAudio.playOnAwake = false;
            driftAudio.loop = true;
            driftAudio.volume = 1.6f;
            driftAudio.spatialBlend = 1f; // Make it 3D
        }

        if (driftClip != null)
        {
            driftAudio.clip = driftClip;
        }
        else
        {
            Debug.LogWarning("Drift clip not assigned!");
        }
    }

    private void Update()
    {
        if (hasGameEnded) return;

        CalculateMovement();
        Steering();
        ApplyTransformToWheels();
        HandleEngineAudio();
        HandleDriftSound();
    }


    public void MoveInput(float input)
    {
        VertcalPos = input;
    }

    public void SteeringInput(float input)
    {
        HorizontalPos = input;
    }

    void CalculateMovement()
    {
        CarSpeed = Car_Rb.linearVelocity.magnitude;
        CarSpeedConverted = Mathf.Round(CarSpeed * 3.6f);
        HandeBrake = Input.GetKey(KeyCode.Space);

        if (HandeBrake)
        {
            motorTorque = 0;
            ApplyBrakes();

            if (!Smokeeffectsenabled)
            {
                EnableSmokeEffect(true);
                Smokeeffectsenabled = true;
            }
        }
        else
        {
            ReleaseBrakes();

            if (CarSpeedConverted < MaxSpeed)
            {
                motorTorque = MaxMotorTorque * VertcalPos;
            }
            else
            {
                motorTorque = 0;
            }

            if (Smokeeffectsenabled)
            {
                EnableSmokeEffect(false);
                Smokeeffectsenabled = false;
            }
        }

        ApplyMotorTorque();
    }

    public void Steering()
    {
        TireAngles = MaxSteeringAngle * HorizontalPos;
        Fr_WheelCollider.steerAngle = TireAngles;
        Fl_WheelCollider.steerAngle = TireAngles;
    }

    public void ApplyTransformToWheels()
    {
        Vector3 Position;
        Quaternion rotation;

        Fl_WheelCollider.GetWorldPose(out Position, out rotation);
        Fl_Wheel.transform.position = Position;
        Fl_Wheel.transform.rotation = rotation;

        Fr_WheelCollider.GetWorldPose(out Position, out rotation);
        Fr_Wheel.transform.position = Position;
        Fr_Wheel.transform.rotation = rotation;

        Bl_WheelCollider.GetWorldPose(out Position, out rotation);
        Bl_Wheel.transform.position = Position;
        Bl_Wheel.transform.rotation = rotation;

        Br_WheelCollider.GetWorldPose(out Position, out rotation);
        Br_Wheel.transform.position = Position;
        Br_Wheel.transform.rotation = rotation;
    }

    void ApplyMotorTorque()
    {
        Fl_WheelCollider.motorTorque = motorTorque;
        Fr_WheelCollider.motorTorque = motorTorque;
        Bl_WheelCollider.motorTorque = motorTorque;
        Br_WheelCollider.motorTorque = motorTorque;
    }

    private void ApplyBrakes()
    {
        Fl_WheelCollider.brakeTorque = BrakingPower;
        Bl_WheelCollider.brakeTorque = BrakingPower;
        Fr_WheelCollider.brakeTorque = BrakingPower;
        Br_WheelCollider.brakeTorque = BrakingPower;
    }

    private void ReleaseBrakes()
    {
        Fl_WheelCollider.brakeTorque = 0;
        Bl_WheelCollider.brakeTorque = 0;
        Fr_WheelCollider.brakeTorque = 0;
        Br_WheelCollider.brakeTorque = 0;
    }

    void EnableSmokeEffect(bool enable)
    {
        foreach (ParticleSystem SmokeEffect in Smokeeffects)
        {
            if (enable) SmokeEffect.Play();
            else SmokeEffect.Stop();
        }
    }

    void HandleEngineAudio()
    {
        if (!engineAudio.isPlaying)
            engineAudio.Play();

        float speed01 = Mathf.Clamp01(CarSpeedConverted / MaxSpeed);
        //float targetVolume = Mathf.Lerp(0.2f, 1f, speed01);
        float targetVolume = Mathf.Lerp(0.2f, 1.2f, speed01);
        engineAudio.volume = Mathf.Clamp(targetVolume, 0f, 1.5f);
        engineAudio.volume = Mathf.Lerp(engineAudio.volume, targetVolume, Time.deltaTime * 5f);
        engineAudio.pitch = Mathf.Clamp(1f + (CarSpeedConverted / 100f), 1f, 2f);
    }

    void HandleDriftSound()
    {
        if (driftAudio == null || driftClip == null) return;

        bool isTurning = Mathf.Abs(HorizontalPos) > 0.2f;
        bool isMoving = CarSpeedConverted > 20f;

        if (isTurning && isMoving)
        {
            if (!driftAudio.isPlaying)
            {
                driftAudio.Play();
            }
            EnableSmokeEffect(true);
        }
        else
        {
            if (driftAudio.isPlaying)
            {
                driftAudio.Stop();
            }
            EnableSmokeEffect(false);
        }
    }

   

    public void OnGameOver()
    {
        hasGameEnded = true;

        if (engineAudio != null && engineAudio.isPlaying)
            engineAudio.Stop();

        if (driftAudio != null && driftAudio.isPlaying)
            driftAudio.Stop();
    }

    public void OnPause()
    {
        if (engineAudio != null && engineAudio.isPlaying)
            engineAudio.Pause();

        if (driftAudio != null && driftAudio.isPlaying)
            driftAudio.Pause();
    }

    public void OnResume()
    {
        hasGameEnded = false; // This re-enables Update()

        if (engineAudio != null)
            engineAudio.UnPause();

        if (driftAudio != null)
            driftAudio.UnPause();
    }


}
