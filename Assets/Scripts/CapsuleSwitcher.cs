using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Put this on any single GameObject in the scene (e.g. an empty "GameManager" object,
/// or the object holding your CinemachineBrain). Assign every possessable capsule to
/// `capsules` in the inspector. Each frame it raycasts from the currently-active
/// capsule's camera pivot to find what you're looking at and toggles its outline via
/// `Highlightable`. Pressing E swaps possession to whatever's currently highlighted.
/// </summary>
public class CapsuleSwitcher : MonoBehaviour
{
    [Tooltip("Every capsule that can be possessed. Whichever one has isActive = true " +
             "in the inspector at scene start is treated as the starting capsule.")]
    public PlayerMovement[] capsules;

    [Tooltip("Max distance the look-ray checks for a switchable capsule.")]
    public float interactRange = 15f;

    [Tooltip("Layers the interact raycast should hit. Defaults to everything.")]
    public LayerMask interactMask = ~0;

    [Tooltip("Push the ray origin forward past your own collider by this much so you " +
             "don't immediately hit yourself.")]
    public float raySkin = 1.1f;

    private PlayerMovement current;
    private PlayerMovement lookTarget;

    void Start()
    {
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
            TrySwitch();
        }
    }

    private void UpdateHighlight()
    {
        PlayerMovement newTarget = GetLookTarget();
        if (newTarget == lookTarget) return;

        if (lookTarget != null && lookTarget.highlight != null)
        {
            lookTarget.highlight.SetHighlighted(false);
        }

        lookTarget = newTarget;

        if (lookTarget != null && lookTarget.highlight != null)
        {
            lookTarget.highlight.SetHighlighted(true);
        }
    }

    private PlayerMovement GetLookTarget()
    {
        if (current.cameraPivot == null) return null;

        Vector3 origin = current.cameraPivot.position + current.cameraPivot.forward * raySkin;
        Ray ray = new Ray(origin, current.cameraPivot.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactMask))
        {
            PlayerMovement target = hit.collider.GetComponentInParent<PlayerMovement>();

            if (target != null && target != current && System.Array.IndexOf(capsules, target) >= 0)
            {
                return target;
            }
        }

        return null;
    }

    private void TrySwitch()
    {
        if (lookTarget == null) return;

        if (lookTarget.highlight != null)
        {
            lookTarget.highlight.SetHighlighted(false);
        }

        current.Unpossess();
        lookTarget.Possess();
        current = lookTarget;
        current.highlight.SetHighlighted(false);
        lookTarget = null;
    }
}