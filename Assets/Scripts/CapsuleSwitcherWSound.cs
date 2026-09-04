using UnityEngine;
using UnityEngine.InputSystem;

// Inside a GameManager object, Assign every possessable capsule to
// capsules in the inspector. Each frame it raycasts from the currently-active
// capsule's camera pivot to find what you're looking at and toggles its outline via
// Highlightable. Pressing E swaps possession to whatever's currently highlighted.
[RequireComponent(typeof(AudioSource))]
public class CapsuleSwitcherWSound : MonoBehaviour
{
    [Tooltip("Every capsule that can be possessed. Whichever one has isActive = true " +
             "in the inspector at scene start is treated as the starting capsule.")]
    public PlayerMovement[] capsules;

    [Tooltip("Max distance the look-ray checks for a switchable capsule.")]
    public float interactRange = 45f;

    [Tooltip("Layers the interact raycast should hit. Defaults to everything.")]
    public LayerMask interactMask = ~0;

    [Tooltip("Push the ray origin forward past your own collider by this much so you " +
             "don't immediately hit yourself.")]
    public float raySkin = 1.1f;

    [Header("Audio Settings")]
    [Tooltip("Sound played when successfully teleporting/swapping.")]
    public AudioClip teleportSuccessSound;

    [Tooltip("Sound played when E is pressed but no target is highlighted.")]
    public AudioClip teleportFailSound;

    private PlayerMovement current;
    private PlayerMovement lookTarget;
    private AudioSource audioSource;

	public MindTransferEffect effect;

    void Start()
    {
        // Cache the AudioSource component on this GameObject
        audioSource = GetComponent<AudioSource>();

        foreach (var c in capsules)
        {
            if (c != null && c.isActive)
            {
                current = c;
                break;
            }
        }

        if (current == null && capsules.Length > 0)
        {
            current = capsules[0];
            current.Possess();
        }
    }

    void Update()
    {
        if (current == null) return;

        UpdateHighlight();

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TrySwitch(); // will fail if not highlighted
        }
    }

    private void UpdateHighlight()
    {
        PlayerMovement newTarget = GetLookTarget();
        if (newTarget == lookTarget) return; // No change in target

        // We can assume a new target, so remove old highlight
        if (lookTarget != null && lookTarget.highlight != null)
        {
            lookTarget.highlight.SetHighlighted(false);
        }

        // Set the new highlight
        lookTarget = newTarget;
        if (lookTarget != null && lookTarget.highlight != null)
        {
            lookTarget.highlight.SetHighlighted(true);
        }
    }

    private PlayerMovement GetLookTarget()
    {
        if (current.cameraPivot == null) return null;

        // Starting point a bit outside the bean to not collide with itself (or it's outline)
        Vector3 origin = current.cameraPivot.position + current.cameraPivot.forward * raySkin;
        Ray ray = new Ray(origin, current.cameraPivot.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactMask))
        {
            // We are looking at a collider
            PlayerMovement target = hit.collider.GetComponentInParent<PlayerMovement>();

            if (target != null && target != current && System.Array.IndexOf(capsules, target) >= 0)
            {
                // We are looking at a player
                return target;
            }
        }

        return null;
    }

    private void TrySwitch()
    {
        // Play fail sound if there's no valid target highlighted
        if (lookTarget == null)
        {
            PlaySound(teleportFailSound);
            return;
        }

        // Play success sound when switching occurs
        PlaySound(teleportSuccessSound);
		//effect.playTransfer();

        if (lookTarget.highlight != null)
        {
            // set the highlight off before switching, so we don't 
            // leave it on when we switch to a new capsule
            lookTarget.highlight.SetHighlighted(false);
        }

        current.Unpossess();
        lookTarget.Possess();
        current = lookTarget;
        lookTarget = null;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            // PlayOneShot allows overlapping sound effects without interrupting previous ones
            audioSource.PlayOneShot(clip);
        }
    }
}