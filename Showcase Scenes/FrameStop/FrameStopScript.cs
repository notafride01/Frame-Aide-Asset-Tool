using System;
using System.Collections;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using notafridge.FrameAid;
using TMPro;

namespace notafridge.FrameAid
{
    /// <FrameStopScript Demonstration>
    /// 
    /// Demonstrates the accuracy of the tool by stopping an animation at a specific frame.
    /// 
    /// When the IEnumerator starts, it selects a random number between 0 and 20 as the target frame. 
    /// Once the animation reaches that frame, it pauses, then the cycle repeats.
    /// 
    /// The number displayed on the object indicates the current frame of the animation.
    /// 
    /// </FrameStopScript Demonstration>


    public class FrameStopScript : MonoBehaviour
    {
        private Animator animator;
        private FrameAideTool FrameAidScript;

        public TextMeshProUGUI textUI;

        bool loopStarted = false;

        void Start()
        {
            animator = GetComponent<Animator>();
            FrameAidScript = GetComponent<FrameAideTool>();
            //Time.timeScale = 0.25f;
        }

        void Update()
        {
            if (!loopStarted)
            {
                StartCoroutine(TestingLoop());
            }
        }

        IEnumerator TestingLoop()
        {
            loopStarted = true;
            animator.speed = 1f;

            // Pick a random frame to stop at
            int ranNum = UnityEngine.Random.Range(0, 20);

            textUI.text = "Next pause target frame: " + ranNum;

            // Wait until the animator hits that frame
            yield return new WaitUntil(() => FrameAidScript.GetCurrentFrame(animator) == ranNum);

            animator.speed = 0f;

            yield return new WaitForSeconds(5f);
            loopStarted = false;

        }
    }
}

