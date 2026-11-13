using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class ObjectCollisionAudio : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip[] impactSounds;

    [Header("Impact Settings")]
    [SerializeField] private float minimumImpactVelocity = 10f;
    [SerializeField] private float maximumImpactVelocity = 20f;

    [Header("Sound Variation")]
    [SerializeField] private float minimumPitch = 0.85f;
    [SerializeField] private float maximumPitch = 1.15f;
    [SerializeField] private float soundCooldown = 0.15f;

    private AudioSource audioSource;
    private float nextSoundTime;

    private void Awake()
    {
        // Get the AudioSource attached to this object.
        audioSource = GetComponent<AudioSource>();

        // Configure the sound to play in 3D space.
        audioSource.spatialBlend = 1f;

        // Allow multiple impact sounds to overlap if needed.
        audioSource.playOnAwake = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ignore collisions if there are no impact sounds assigned.
        if (impactSounds == null || impactSounds.Length == 0)
            return;

        // Prevent several sounds from playing almost simultaneously.
        if (Time.time < nextSoundTime)
            return;

        // Measure how quickly the object was moving when it collided.
        float impactStrength = collision.relativeVelocity.magnitude;

        // Ignore very gentle collisions.
        if (impactStrength < minimumImpactVelocity)
            return;

        // Select a random impact sound.
        AudioClip clip = impactSounds[
            Random.Range(0, impactSounds.Length)
        ];

        // Calculate sound volume based on collision strength.
        float volume = Mathf.InverseLerp(
            minimumImpactVelocity,
            maximumImpactVelocity,
            impactStrength
        );

        // Randomize pitch slightly to make impacts sound less repetitive.
        audioSource.pitch = Random.Range(minimumPitch, maximumPitch);

        // Play the impact sound.
        audioSource.PlayOneShot(clip, volume);

        // Set the next time a collision sound can play.
        nextSoundTime = Time.time + soundCooldown;
    }
}
