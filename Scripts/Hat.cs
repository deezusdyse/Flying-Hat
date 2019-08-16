using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hat : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    Rigidbody rigidBody;
    AudioSource soundSource;

    bool transition = false;
    bool collisionsDisabled = false;

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        soundSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!transition)
        {
            thrusting();
            applyRotation();
        }
        if (Debug.isDebugBuild)
        {
            debugKeys();
        }
    }

    private void debugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            nextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collisionsDisabled || transition) { 
            return; 
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // do nothing
                break;
            case "Finish":
                beginSuccess();
                break;
            default:
                beginDeath();
                break;
        }
    }

    private void beginSuccess()
    {
        transition = true;
        soundSource.Stop();
        soundSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("nextLevel", levelLoadDelay);
    }

    private void beginDeath()
    {
        transition = true;
        soundSource.Stop();
        soundSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("levelOne", levelLoadDelay);
    }

    private void nextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0; // loop back to start
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void levelOne()
    {
        SceneManager.LoadScene(0);
    }

    private void thrusting()
    {
        if (Input.GetKey(KeyCode.Space)) // can thrust while rotating
        {
            thrustApply();
        }
        else
        {
            stopThrusting();
        }
    }

    private void stopThrusting()
    {
        soundSource.Stop();
        mainEngineParticles.Stop();
    }



    private void applyRotation()
    {
        if (Input.GetKey(KeyCode.A))
        {
            RotateManually(rcsThrust * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            RotateManually(-rcsThrust * Time.deltaTime);
        }
    }

        private void thrustApply()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!soundSource.isPlaying) // so it doesn't layer
        {
            soundSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void RotateManually(float rotationThisFrame)
    {
        rigidBody.freezeRotation = true; // take manual control of rotation
        transform.Rotate(Vector3.forward * rotationThisFrame);
        rigidBody.freezeRotation = false; // resume physics control of rotation
    }
}