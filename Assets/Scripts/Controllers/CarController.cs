using Cars;
using UnityEngine;

public class CarController : MonoBehaviour
{
    #region SerializeField

    [SerializeField] private Car car;
    [SerializeField] private Material oldMaterial;

    #endregion

    #region Public

    public float speed;
    public float rotationSpeed;

    #endregion

    #region Property

    public int CarRotateDirection { get; set; }

    #endregion

    #region Private

    private readonly Color32 _trailColor32 = new Color32(249, 65, 68,255);
    private Rigidbody _rigidbody;
    
    private int _stepCount;
    private int _numberOfSteps;

    #endregion
    
    private void Start()
    {
        //Access Rigidbody
        _rigidbody = GetComponent<Rigidbody>();
        
        //Set speed
        speed = LevelController.Instance.carForwardSpeed;
        
        //Set rotation speed
        rotationSpeed = LevelController.Instance.carRotateSpeed;
    }

    private void OnEnable()
    {
        if (car.Type != CarType.Old) return;
        
        //if is car old then set stepCount
        _stepCount = 0;
        
        //Then get trails list count
        _numberOfSteps = car.Trails.Count;
    }

    private void FixedUpdate()
    {
        //Get gameObject transform
        var goTransform = transform;
        
        //if game platyng now then execute if statement 
        if (LevelController.Instance.LevelStateProp == LevelController.LevelState.Playing)
        {
            //Which state of car?
            switch (car.State)
            {
                case CarState.Playing:
                    
                    // Is car current or old one ?
                    switch (car.Type)
                    {
                        case CarType.Current:
                            
                            // Move Forward
                            _rigidbody.velocity = goTransform.forward * speed;

                            #if UNITY_EDITOR
                            
                            //Get Input from user with editor
                            UserControlByKeyboard();                       
                            
                            #endif
                            
                            //For button input
                            if (CarRotateDirection != 0)
                            {
                                goTransform.Rotate(Vector3.up * CarRotateDirection, rotationSpeed);
                            }

                            // Store steps
                            car.Trails.Add(new CarTrail(goTransform.position, goTransform.rotation));
                            break;

                        case CarType.Old:
                            
                            //if _stepCount smaller than numberOfSteps then execute
                            if (_stepCount < _numberOfSteps)
                            {
                                goTransform.position = car.Trails[_stepCount].Position;
                                goTransform.rotation = car.Trails[_stepCount].Rotation;

                                _stepCount++;
                            }
                            break;
                    }
                    break;
                
                case CarState.Success:
                    //Car's velocity set by 0
                    _rigidbody.velocity = Vector3.zero;
                    break;
                
                case CarState.Fail:
                    //Car's velocity set by 0
                    _rigidbody.velocity = Vector3.zero;
                    break;
            }
        }
        else
        {
            //Car's velocity set by 0
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }

    //Set car with parameter
    public void SetCar(Car car)
    {
        this.car = car;
    }

    //Update car state
    public void UpdateCarState(CarState state)
    {
        car.State = state;
    }

    //Update car type
    private void UpdateCarType(CarType type)
    {
        car.Type = type;
        gameObject.tag = "Old";
    }

    //After you reach the exit then car need to be old one
    public void MakeOldOne()
    {
        Relocate();
        UpdateCarType(CarType.Old);
        UpdateCarState(CarState.Playing);
        UpdateMaterials();
    }

    //Update car parts with new material
    private void UpdateMaterials()
    {
        foreach (Transform part in gameObject.transform)
        {
            if (part.gameObject.GetComponent<MeshRenderer>() != null)
            { 
                part.gameObject.GetComponent<MeshRenderer>().material = oldMaterial;
            }
        }

        gameObject.GetComponent<TrailRenderer>().startColor = _trailColor32;
    }

    //To play again
    public void Replay()
    {
        Relocate();
        
        if (car.Type == CarType.Current) car.Trails.Clear();
        
        UpdateCarState(CarState.Playing);
    }

    //Resetting car for replay
    public void Relocate()
    {
        var goTransform = transform;
        
        gameObject.SetActive(false);
        
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        
        goTransform.position = car.Trails[0].Position;
        goTransform.rotation = car.Trails[0].Rotation;
        
        gameObject.SetActive(true);
    }

    //To play from the editor
    private void UserControlByKeyboard()
    {
        //Rotate right
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up, rotationSpeed);
        }
        
        //Rotate left
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up * -1, rotationSpeed);
        }
    }
    
    //For the impact obstacle, old cars or target points
    private void OnCollisionEnter(Collision other)
    {
        if (car.Type != CarType.Current) return;
        
        if (other.gameObject.CompareTag("Obstacles") || other.gameObject.CompareTag("Old"))
        {
            LevelController.Instance.SetLevelState(LevelController.LevelState.Fail);
        }

        if (other.gameObject.CompareTag("Target"))
        {
            LevelController.Instance.SetLevelState(LevelController.LevelState.Success);
        }
    }
}