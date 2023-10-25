using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    //[SerializedField] makes any declared variable customizable at run time.
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float sceneDelay = 2;
    [SerializeField] AudioClip mainEngineSound;
    [SerializeField] AudioClip rocketDestroyedSound;
    [SerializeField] AudioClip levelCompleteSound;
    [SerializeField] ParticleSystem mainEngineParticle;
    [SerializeField] ParticleSystem successParticle;
    [SerializeField] ParticleSystem deathParticle;

    Rigidbody rigidBody;
    AudioSource audioSource;
    bool collissionEnabled = false;

    enum State {Start,Alive,Dying,Trancending }
    State state = State.Start;


    // Start is called before the first frame update
    void Start()
    {
        //enable to limit fps 
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 10;
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        GetCurrentScene();
        GetSceneCount();

    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Start) //disables rotation before launch
        {
            Thrust();
        }
        else if (state == State.Alive) //check if the rocket is in play if not disables controls
        {
            Thrust();
            Rotate();
        }

        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
        

    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            DebugMode();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
    }

    //unity collision detector
    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; }

        CheckColision(collision);
    
    }

    private void CheckColision(Collision collision)
    {
        if (collissionEnabled) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StopSfx();
                state = State.Trancending;
                PlaySfx();
                Invoke("LoadNextScene", sceneDelay);
                break;
            default:
                StopSfx();
                state = State.Dying;
                PlaySfx();
                Invoke("LoadFirstScene", sceneDelay);
                break;
        }
    }

    private void LoadFirstScene()
    {
        SceneManager.LoadScene(0);
    }

    private bool InitializeSceneCount()
    {
        if (GetCurrentScene() != (GetSceneCount() - 1))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void LoadNextScene()
    {
        if (InitializeSceneCount())
        {
            int scene = GetCurrentScene() + 1;
            SceneManager.LoadScene(scene);
        }
        else //go back to base level
        {
            SceneManager.LoadScene(0);
        }
    }

    private int GetCurrentScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        int index = scene.buildIndex;
        return index;
    }

    private int GetSceneCount()
    {
        int numberOfLevels = SceneManager.sceneCountInBuildSettings;
        return numberOfLevels;
    }

    private void PlaySfx()
    {
        if (!audioSource.isPlaying)
        {
            if (state == State.Alive)
            {
                audioSource.PlayOneShot(mainEngineSound);
                mainEngineParticle.Play();
            }
            else if (state == State.Dying)
            {
                mainEngineParticle.Stop();
                deathParticle.Play();
                audioSource.PlayOneShot(rocketDestroyedSound);
            }
            else if (state == State.Trancending)
            {
                successParticle.Play();
                audioSource.PlayOneShot(levelCompleteSound);
            }
        }
    }

    private void StopSfx()
    {
        audioSource.Stop();
    }

    private float FrameIndependentMovement(float thrust)
    {
        //calculate rate to rotation base on the fps
        //lower fps lower rotation rate - high fps high rotation rate
        float movementByFrame = thrust * Time.deltaTime;
        return movementByFrame;
    }    
    
    private void Thrust()
    {
        //listens to the keystroke during runtime
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W))
        {
            state = State.Alive;
            PlaySfx();
            RocketThrust();
        }

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.W))
        {
            StopSfx();
            mainEngineParticle.Stop();
        }
    }

    private void RocketThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * FrameIndependentMovement(mainThrust));
    }

    private void Rotate()
    {
        rigidBody.angularVelocity = Vector3.zero;

        if (Input.GetKey(KeyCode.A))
        {
            RotateLeft();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            RotateRight();
        }

        //rigidBody.freezeRotation = false; //resume physics control of rotation after key press
    }

    private void RotateRight()
    {
        transform.Rotate(-Vector3.forward * FrameIndependentMovement(rcsThrust));
    }

    private void RotateLeft()
    {
        transform.Rotate(Vector3.forward * FrameIndependentMovement(rcsThrust));
    }

    private void DebugMode()
    {
        if (!collissionEnabled)
        {
            collissionEnabled = true;
            print("debug mode on");
        }
        else
        {
            print("debug mode off");
            collissionEnabled = false;
        }
    }
}
