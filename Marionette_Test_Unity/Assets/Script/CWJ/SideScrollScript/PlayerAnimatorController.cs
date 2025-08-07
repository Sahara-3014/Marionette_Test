using UnityEngine;
using UnityEngine.Playables;

public class PlayerAnimatorController : MonoBehaviour
{

    public string playerState;
    public Animator animator;

    void Start()
    {
        
    }

    private void Update()
    {
        if (playerState == "Walk")
        {
            animator.SetBool("Walk", true);
        }
        else
        {
            animator.SetBool("Walk", false);
        }

        if (playerState == "Idle")
        {
            animator.SetBool("Idle", true);
        }
        else
        {
            animator.SetBool("Idle", false);
        }

    }


}
