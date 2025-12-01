using UnityEngine;

/*
This script creates an animation of the assigned game-object.
The game-object is scaled and moved vertically.
*/

public class ObjectAnimation : MonoBehaviour
{
    //////////////////// Define Variables ////////////////////
    [Header("Animation Settings")]
    [Tooltip("Amplitude of the vertical movement animation")]
    public float floatAmplitude = 0.05f;
    [Tooltip("Frequenc of the vertical movement animation")]
    public float floatFrequency = 2;
    [Tooltip("Amplitude of the vertical scaling animation")]
    public float scaleAmplitude = 0.05f;
    [Tooltip("Frequenc of the vertical scaling animation")]
    public float scaleFrequency = 2;


    [Header("Initial Settings")]
    [Tooltip("Initial position of the object")]
    private Vector3 initialPosition;
    [Tooltip("Initial scale of the object")]
    private Vector3 initialScale;


    [Header("Durations")]
    [Tooltip("Duration of the movement animation")]
    private float floatTime = 0f;
    [Tooltip("Duration of the scaling animation")]
    private float scaleTime = 0f;

    void Start()
    {
        initialPosition = transform.position;
        initialScale = transform.localScale;
    }









    //////////////////// Every Frame ////////////////////
    void Update()
    {
        // Update the timers
        floatTime += Time.deltaTime * floatFrequency;
        scaleTime += Time.deltaTime * scaleFrequency;


        // Calculate the new position on the y-axis of the object
        float newY = initialPosition.y + Mathf.Sin(floatTime) * floatAmplitude;

        // Add the new position on the Y-axis to the object's position
        transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);


        // Calculate the new scaling on the repsective axis
        float newScalex = initialScale.x + Mathf.Sin(scaleTime) * scaleAmplitude;
        float newScaley = initialScale.y + Mathf.Sin(scaleTime) * scaleAmplitude;
        float newScalez = initialScale.z + Mathf.Sin(scaleTime) * scaleAmplitude;

        // Update the object's scaling
        transform.localScale = new Vector3(newScalex, newScaley, newScalez);
    }
}
