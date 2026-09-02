using UnityEngine;
using UnityEngine.InputSystem;

// Inside of an empty GameObject in the scene Assign every possessable capsule to
// `capsules` in the inspector. Pressing E raycasts from the currently-active capsule's
// camera pivot; if it hits another listed capsule, control + camera priority swap to it
// and Cinemachine blends automatically.
public class CapsuleSwitcher : MonoBehaviour
{
    [Tooltip("Every capsule that can be possessed. Whichever one has isActive = true " +
             "in the inspector at scene start is treated as the starting capsule.")]
    public PlayerMovement[] capsules;

    [Tooltip("Max distance the look-ray checks for a switchable capsule.")]
    public float interactRange = 15f;

    [Tooltip("Layers the interact raycast should hit. Defaults to everything.")]
    public LayerMask interactMask = ~0;

    private PlayerMovement current;

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
        if (Keyboard.current == null || current == null) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            TrySwitch();
        }
    }

    private void TrySwitch()
    {
        if (current.cameraPivot == null) return;

        Ray ray = new Ray(current.cameraPivot.position, current.cameraPivot.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactMask))
        {
            PlayerMovement target = hit.collider.GetComponentInParent<PlayerMovement>();

            Debug.Log($"Hit: {hit.collider?.name}, resolved target: {target?.name}, current: {current.name}");

            if (target != null && target != current && System.Array.IndexOf(capsules, target) >= 0)
            {
                current.Unpossess();
                target.Possess();
                current = target;
            }
        }
    }
}