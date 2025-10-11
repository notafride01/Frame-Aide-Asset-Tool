using UnityEngine;
using notafridge.FrameAid;

namespace notafridge.FrameAid
{
    public class HitBoxScript : MonoBehaviour
    {
        /// <HitBoxScript>
        /// 
        /// Adds a knockback effect when the player punches an object tagged "Enemy".
        /// 
        /// </HitBoxScript>

        private PlayerMovement playerScript;
        private Rigidbody2D rbPlayer;

        private Animator animator;
        private FrameAideTool frameAideTool;
        private void Start()
        {
            playerScript = GetComponentInParent<PlayerMovement>();
            rbPlayer = GetComponent<Rigidbody2D>();
            animator = GetComponentInParent<Animator>();
            frameAideTool = GetComponentInParent<FrameAideTool>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "ENEMY")
            {
                if (frameAideTool.animName == "PUNCH")
                {
                    Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
                    rb.AddForce(new Vector2(playerScript.punchForce, 0), ForceMode2D.Impulse);
                }
            }
        }
    }
}


