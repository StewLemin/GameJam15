using UnityEngine;

public class MindTransferEffect : MonoBehaviour
{

    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void playTransfer()
    {
        Debug.Log("got into animation function block");
        animator.SetTrigger("PlayMindStart");
    }
}
