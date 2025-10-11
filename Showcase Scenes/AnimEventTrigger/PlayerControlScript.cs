using UnityEngine;
using UnityEngine.InputSystem;
namespace notafridge.FrameAid
{
    public class PlayerMovement : MonoBehaviour
    {
        /// <PlayerMovementScript Summary>
        /// 
        /// Basic player movement script.  
        /// 
        /// </PlayerMovementScript Summary>

        private Rigidbody2D rb;
        private Animator animator;

        public float moveSpeed = 5f;
        public float jumpForce = 5f;
        public float punchForce;

        public bool isPunch;
        public bool isMoving;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.F))
            {
                isPunch = true;
                animator.Play("PUNCH");
            }

        }

        private void FixedUpdate()
        {
            if (!isPunch)
            {
                playerMovement();
            }
        }

        void playerMovement()
        {
            float x = Input.GetAxisRaw("Horizontal");

            rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

            if (x != 0)
            {
                isMoving = true;
                animator.Play("MOVE");
            }
            else
            {
                animator.Play("IDLE");
            }
        }
    }
}

