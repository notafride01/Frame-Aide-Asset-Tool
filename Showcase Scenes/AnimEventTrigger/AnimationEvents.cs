using UnityEngine;
using notafridge.FrameAid;

namespace notafridge.FrameAid
{
    public class AnimationEvents : MonoBehaviour
    {
        /// <AnimationEvent Summary>
        /// 
        /// Demonstrates handling animation events in code instead of Unity’s Animation panel.  
        /// PlayerMoveScript sets booleans when specific frames are reached, keeping all event logic in the IDE/C# Script.  
        /// 
        /// </AnimationEvent Summary>

        private Animator animator;
        private FrameAideTool frameAideTool;
        private PlayerMovement playerMoveScript;
        void Start()
        {
            frameAideTool = GetComponent<FrameAideTool>();
            animator = GetComponent<Animator>();
            playerMoveScript = GetComponent<PlayerMovement>();
        }

        void Update()
        {
            if (animator != null)
            {
                //Check if the animation name is PUNCH, if true
                //then set booleans true at the start of the animation
                if (frameAideTool.animName == "PUNCH")
                {
                    if (frameAideTool.GetCurrentFrame(animator) == 1)
                    {
                        playerMoveScript.isPunch = true;
                        playerMoveScript.isMoving = false;
                    }

                    if (frameAideTool.GetCurrentFrame(animator) == 23)
                    {
                        playerMoveScript.isPunch = false;
                    }
                }
            }
        }
    }
}


