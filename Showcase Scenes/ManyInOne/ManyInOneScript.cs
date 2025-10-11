using notafridge.FrameAid;
using Unity.VisualScripting;
using UnityEngine;

namespace notafridge.FrameAid
{
    public class ManyInOneScript : MonoBehaviour
    {
        /// <ManyInOneScript Demonstration>
        /// 
        /// Demonstrates FrameAidTool by controlling sections of a single animation state.  
        /// Instead of separate states for "IDLE" and "RUNNING", both are stored in one clip ("AllInOne_Player").  
        /// Frames 0–8 = IDLE, 11–34 = RUNNING. This approach can combine many animations into one state.  
        /// 
        /// Uses PlayTillFrame() to play a section depending on if the character is walking or not  
        /// 
        /// </ManyInOneScript Demonstration>

        private Animator animator;
        private FrameAideTool frameAideTool;
        private Rigidbody2D rb;

        public bool IDLE;
        public bool ISMOVING;

        public bool facingRight = true;

        public bool animPlaying;

        public float horizontalInput;
        public float moveSpeed;

        public Coroutine playTillRoutine;

        public enum AnimState
        {
            Idle,
            Moving,
            Climbing,
            Jumping
        }

        public AnimState state, lastState;

        void Start()
        {
            animator = GetComponent<Animator>();
            frameAideTool = GetComponent<FrameAideTool>();
            rb = GetComponent<Rigidbody2D>();
            playTillRoutine = StartCoroutine(frameAideTool.PlayTillFrame("MainChar", 0, 8));
        }

        void Update()
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            movement(horizontalInput);

            // decide desired state from input only
            state = (horizontalInput != 0) ? AnimState.Moving : AnimState.Idle;

            // if state changed, kill current slice and switch now
            if (state != lastState)
            {
                if (playTillRoutine != null)
                {
                    StopCoroutine(playTillRoutine);
                    playTillRoutine = null;
                }
                animPlaying = false;    // allow immediate restart
                lastState = state;
            }

            animationSwitch();
        }


        private void FixedUpdate()
        {

        }

        void movement(float x)
        {
            rb.linearVelocity = new Vector2(x * moveSpeed, 0);

            if (x < 0 && facingRight)
            {
                flip();
            }
            else if (x > 0 && !facingRight)
            {
                flip();
            }
        }

        void animationSwitch()
        {
            if (!animPlaying)
            {
                switch (state)
                {
                    case AnimState.Idle:
                        playTillRoutine = StartCoroutine(frameAideTool.PlayTillFrame("MainChar", 0, 8));
                        animPlaying = true;
                        break;

                    case AnimState.Moving:
                        playTillRoutine = StartCoroutine(frameAideTool.PlayTillFrame("MainChar", 11, 34));
                        animPlaying = true;
                        break;
                }
            }

            if (animPlaying)
            {
                int current = frameAideTool.GetCurrentFrame(animator);
                if (state == AnimState.Idle && current >= 8) animPlaying = false;
                if (state == AnimState.Moving && current >= 34) animPlaying = false;
            }
        }


        void flip()
        {
            facingRight = !facingRight;

            Vector2 scale = transform.localScale;

            scale.x *= -1;

            transform.localScale = scale;
        }
    }
}

