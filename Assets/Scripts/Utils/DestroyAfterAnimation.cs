using UnityEngine;
using System.Collections;

public class DestroyAfterAnimation : MonoBehaviour
{

    public float time = 3;
    private Animator animator;
    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {

            AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0);
            if (clips.Length > 0)
                Destroy(this.gameObject, clips[0].clip.length);
        }
        else
            Destroy(this.gameObject, 1.0f);
    }
}
