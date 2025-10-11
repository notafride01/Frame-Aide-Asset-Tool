using notafridge.FrameAid;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace notafridge.FrameAid
{
    public class ComboInput2 : MonoBehaviour
    {
        private StateMachineBehaviour mMachineBehaviour;

        private Animator animator;
        private FrameAideTool FrameAideTool;

        public int comboStep = 0;
        public float comboResetTime = 1f; // time allowed between presses;

        private float lastPressTime;
        private bool returnIDLE = false;

        void Start()
        {
            animator = GetComponent<Animator>();
            FrameAideTool = GetComponent<FrameAideTool>();
        }

        void Update()
        {

            if (Input.GetKeyDown(KeyCode.F))
            {
                HandleCombo();
            }

            if (Time.time - lastPressTime > comboResetTime && comboStep != 0)
            {
                comboStep = 0;
            }

            if (FrameAideTool.GetCurrentClipName(animator) == "PlayerCombo_IDLE" && !returnIDLE)
            {
                returnIDLE = true;

                if (returnIDLE)
                {
                    comboStep = 0;
                    returnIDLE = false;
                }

            }
        }

        void HandleCombo()
        {
            lastPressTime = Time.time;

            if (comboStep == 0)
            {
                Debug.Log("First Step");
                comboStep = 1;
                animator.Play("PlayerCombo_1", 0, 0f);
            }

            if (InClip("PlayerCombo_1"))
            {
                Debug.Log("In Combo 1");
                if (comboStep == 1 && FrameAideTool.GetCurrentFrame(animator) >= 22)
                {
                    animator.Play("PlayerCombo_2", 0, 0f);
                    comboStep = 2;
                }
            }

            if (InClip("PlayerCombo_2"))
            {
                if (comboStep == 2 && FrameAideTool.GetCurrentFrame(animator) > 12)
                {
                    animator.Play("PlayerCombo_3", 0, 0f);

                    if (FrameAideTool.GetCurrentFrame(animator) >= 20)
                    {
                        comboStep = 0;
                    }
                }
            }

            bool InClip(string name) => FrameAideTool.GetCurrentClipName(animator) == name;
        }
    }
}
