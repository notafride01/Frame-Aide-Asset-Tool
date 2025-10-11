using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace notafridge.FrameAid
{
    public class FrameAideTool : MonoBehaviour
    {
        private Animator animator;
        private RuntimeAnimatorController controller;
        private AnimationClip[] animationClips;

        //animName and frameIndex will keep track of
        //the animators current frameIndex and animation Name
        public string animName;
        public int frameIndex;

        //Stores the information of the animation clip, name, length
        //total amount of frames
        [System.Serializable]
        public class AnimClip
        {
            public string name;
            public float animLength;
            public int totalFrames;
            public List<Keyframe> keys = new List<Keyframe>();
        }

        //Represents a single frame of animation
        //The time reference and the frameIndex (frame 0,1,etc)
        [System.Serializable]
        public class Keyframe
        {
            public int frameIndex;
            public float timeReference;
        }

        //Represents all animations with there time properly spliced
        //into frames with timeReference association
        [HideInInspector]
        [SerializeField] public List<AnimClip> animationData = new List<AnimClip>();

        /// <summary>
        /// The information below is pertaining to animation events
        /// that can be called within the inspector panel
        /// </summary>

        [System.Serializable]
        public enum ConditionType
        {
            LessThan = '<',
            GreaterThan = '>',
            Equal = '=',
            LessThanEqual,
            GreaterThanEqual
        }
        [System.Serializable]
        public class FrameEvent
        {
            public string animationName;
            public int targetFrame;
            public ConditionType condition;
            public MonoBehaviour targetScript;
            public string methodName;

            [SerializeField] private List<string> Parameters = new List<string>(); // Stores parameters as strings for serialization

            public void Execute()
            {
                if (targetScript == null || string.IsNullOrEmpty(methodName)) return;

                MethodInfo method = targetScript.GetType().GetMethod(methodName);
                if (method == null) return;

                ParameterInfo[] paramInfo = method.GetParameters();
                object[] parameters = new object[paramInfo.Length];

                for (int i = 0; i < paramInfo.Length; i++)
                {
                    Type paramType = paramInfo[i].ParameterType;

                    // Convert string input into the expected parameter type
                    if (paramType == typeof(int))
                    {
                        parameters[i] = int.TryParse(Parameters[i], out int intValue) ? intValue : 0;
                    }
                    else if (paramType == typeof(float))
                    {
                        parameters[i] = float.TryParse(Parameters[i], out float floatValue) ? floatValue : 0f;
                    }
                    else if (paramType == typeof(string))
                    {
                        parameters[i] = Parameters[i];
                    }
                    else
                    {
                        Debug.LogWarning($"Unsupported parameter type: {paramType}");
                        return;
                    }
                }

                // Invoke the method with the parsed parameters
                method.Invoke(targetScript, parameters);
            }
        }

        public List<FrameEvent> frameEvents = new List<FrameEvent>();

        void Awake()
        {
            //We get the controller to get all the animation clips of an animator
            animator = GetComponent<Animator>();

            if (animator != null)
            {
                controller = animator.runtimeAnimatorController;
            }

            //if the controller is null the animator cannot be found,
            //Tool only works with an animator.
            if (controller == null)
            {
                Debug.LogError("Animator Controller is missing!");
                return;
            }

            //Put all the clips in <List> animationClips to be processed
            animationClips = controller.animationClips;
            ProcessAnimations();
        }

        void Update()
        {
            //animName will get the name of the animation constantly
            //frameIndex will keep getting the current frame of an animation
            animName = GetCurrentClipName(animator);
            frameIndex = GetCurrentFrame(animator);
        }

        //All the code in FixedUpdate pertains to calling
        //functions using the animation event in the inspector
        void FixedUpdate()
        {

            if (animator == null) return;
            
            foreach (FrameEvent frameEvent in frameEvents)
            {
                if (frameEvent.animationName == animName)
                {
                    bool conditionMet = false;

                    //For each conditions, switch case statement is used for each condition Type
                    //If any of the conditions are met set conditionMet to true and break;
                    switch (frameEvent.condition)
                    {
                        case ConditionType.Equal:
                            conditionMet = (frameIndex == frameEvent.targetFrame);
                            break;
                        case ConditionType.LessThan:
                            conditionMet = (frameIndex < frameEvent.targetFrame);
                            break;
                        case ConditionType.GreaterThan:
                            conditionMet = (frameIndex > frameEvent.targetFrame);
                            break;
                    }

                    //Once the condition is met then the script is allowed to execute the function
                    if (conditionMet)
                    {
                        frameEvent.Execute();
                    }
                }
            }
        }


        //PlayTillFrame is a function that when called will play an animation 
        //At a specific frame until it reaches the end of the selected frame.
        //If bool reset is true then it for the animator to update

        public IEnumerator PlayTillFrame(string animationName, int startFrameIndex, int endFrameIndex)
        {
            bool animFound = false;
            bool firstKeyFound = false;
            bool endKeyFound = false;

            AnimClip referenceClip = null;
            Keyframe referenceKeyFrameFirst = null;
            Keyframe referenceKeyFrameLast = null;

            //If the animName is null then give out error/warning
            if (string.IsNullOrEmpty(animationName))
            {
                Debug.LogWarning("Invalid animation name provided.");
            }

            //Go through to see if the animation Name is correct
            for (int i = 0; i < animationData.Count;)
            {
                //If animation Clip name is equal to string then found animation break;
                if (animationData[i].name == animationName)
                {
                    animFound = true;
                    referenceClip = animationData[i];
                    break;
                }
                i++;
            }
            
            //If it isn't found then give a warning log
            if (!animFound)
            {
                Debug.LogWarning($"Animation Name + " + animationName + " was not found"); 
            }

            //If found then within the Animation go through and
            //the frame that the clip wants to start playing at
            if (referenceClip != null)
            {
                for (int i = 0; i < referenceClip.keys.Count;)
                {
                    //If the frame is found we make a reference of it in referenceKeyFrameFirst
                    //Set the boolean to true and get out of the loop
                    if (referenceClip.keys[i].frameIndex == startFrameIndex)
                    {
                        referenceKeyFrameFirst = referenceClip.keys[i];
                        firstKeyFound = true;
                        break;
                    }
                    i++;
                }

                //Second loop goes to find the Last Keyframe
                for (int i = 0; i < referenceClip.keys.Count;)
                {
                    if (referenceClip.keys[i].frameIndex == endFrameIndex)
                    {
                        referenceKeyFrameLast = referenceClip.keys[i];
                        endKeyFound = true;
                        break;
                    }
                    i++;
                }

                //If any of the loop fails give out a warning log
                if (!firstKeyFound)
                {
                    Debug.LogWarning($"Starting Frame {startFrameIndex} not found in animation {animName}.");
                }

                if (!endKeyFound)
                {
                    Debug.LogWarning($"Ending Frame {endFrameIndex} not found in animation {animName}.");
                }    
            }

            bool isRunning = false;

            //Once found find the frameIndex and whatever normalized time it is then 
            //play it at that time setting the is running boolean to true
            if (firstKeyFound && endKeyFound)
            {
                isRunning = true;
                animator.PlayInFixedTime(animationName, 0, referenceKeyFrameFirst.timeReference);

                while (isRunning && animName == animationName)
                {
                    //While the boolean is true keep track of what the current frame is

                    bool reachedEndFrame = GetCurrentFrame(animator) >= referenceKeyFrameLast.frameIndex;

                    if (reachedEndFrame)
                    {
                        //animator.Rebind();
                        //animator.Update(0);
                        break;
                    }

                    yield return new WaitForFixedUpdate();        
                }
            }
        }

        //Play at Frame when called will play an animation at a specific frame
        //taking in two paramteres the name of the animation and the specific frame 
        //you want to play at
        public void PlayAtFrame(string animName, int frameIndex)
        {
            bool animFound = false;
            bool keyFound = false;
            AnimClip referenceClip = null;
            Keyframe referenceKeyFrame = null;

            // Check if animName is valid
            if (string.IsNullOrEmpty(animName))
            {
                Debug.LogWarning("Invalid animation name provided.");
                return;
            }

            //Find the string animation name in the list of animation the animator has
            for (int i = 0; i < animationData.Count;)
            {
                //If animation Clip name is equal to string then
                //found animation and break out of the loop
                if (animationData[i].name == animName)
                {
                    animFound = true;
                    referenceClip = animationData[i];
                    break;
                }
                i++;
            }

            if (!animFound)
            {
                Debug.LogWarning($"{animName} cannot be found in the animator");
                return;
            }

            //Once found go to the frame animationClip and find the associated clip
            if (referenceClip != null)
            {
                for (int i = 0; i < referenceClip.keys.Count;)
                {
                    if (referenceClip.keys[i].frameIndex == frameIndex)
                    {
                        referenceKeyFrame = referenceClip.keys[i];
                        keyFound = true;
                        break;
                    }
                    i++;
                }

                //if the frameIndex was not found the send out a log warning
                if (!keyFound)
                {
                    Debug.LogWarning($"Frame {frameIndex} not found in animation {animName}.");
                    return;
                }
            }

            //Once the frameIndex is valid then get the time reference and
            //play the animation at that time
            if (keyFound)
            {
                animator.PlayInFixedTime(animName, 0, referenceKeyFrame.timeReference);
            }
        }

        //When called this function will get the current frame the animation
        //the frames are determined by the number in the Animation panel when you first
        //set up and create an animation
        public int GetCurrentFrame(Animator animator)
        {
            bool foundClip = false;
            AnimClip designatedClip = null;
            Keyframe keyframe = null;

            //first get the name of the clip that the animator is playing
            string animationName = GetCurrentClipName(animator);

            //Go through the animators series of animations to find the matching clip
            for (int i = 0; i < animationData.Count;)
            {
                if (animationData[i].name == animationName)
                {
                    //Once its found we put the refence into 'designatedClip'
                    //set found Clip to true and break out of the loop
                    designatedClip = animationData[i];
                    foundClip = true;
                    break;
                }
                i++;
            }

            //Once the clip has been found
            //Get the current normalizedTime
            float normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

            //We want the normalizedTime to be modulus so that it removes
            //normalizedTime from being warped by the animator looping
            normalizedTime %= 1f;

            //To get the proper currentTime we multiply it by the animation Length
            //giving us the current time associated with the animation length and not normalizedTime

            //Debug.Log("NormalizedTime is " +  normalizedTime);
            //Debug.Log("DesignatedClip is " + designatedClip.name);

            float currentTime = normalizedTime * designatedClip.animLength;

            //Once the clip is found
            if (foundClip)
            {
                //Iterate through all the frames time reference that the animationClip has 
                //The goal is to get the closest frame.timeReference that is CLOSEST to the currentTime
                for (int i = 0; i < designatedClip.keys.Count;)
                {
                    //Loop will keep going until the timeReference is greater than the currentTime 
                    if (designatedClip.keys[i].timeReference <= currentTime)
                    {     
                        keyframe = designatedClip.keys[i];
                    }
                    i++;
                }
            }

            //If the clip is NOT looping then the function will keep getting the frameCount wrong 
            //Animation will be a still frame but the current frame will keep incrementing
            if (!animator.GetCurrentAnimatorClipInfo(0)[0].clip.isLooping)
            {
                //To fix this check if its at its final frame
                //if true the set its keyframe.frameIndex to the totalFrames - 1
                if (designatedClip.totalFrames == keyframe.frameIndex + 1)
                {
                    keyframe.frameIndex = designatedClip.totalFrames - 1;
                    //return keyframe.frameIndex;
                }
            }


            return keyframe.frameIndex;
        }

        //This function when called properly proccesses each animation time
        //into desginated frame slots, that will be used by each function
        // to properly get the frames of the animation

        public string GetCurrentClipName(Animator animator)
        {
            if (animator == null) return string.Empty;

            // First check current clips
            var clips = animator.GetCurrentAnimatorClipInfo(0);
            if (clips.Length > 0 && clips[0].clip != null)
                return clips[0].clip.name;

            // If in transition, check the next clips
            var next = animator.GetNextAnimatorClipInfo(0);
            if (next.Length > 0 && next[0].clip != null)
                return next[0].clip.name;

            return string.Empty; // nothing valid this frame
        }



        private void ProcessAnimations()
        {
            //For each animation clip in the animationClip List
            foreach (AnimationClip clip in animationClips)
            {
                ///<summary> 
                /// Get the name of the Clip,
                /// 
                /// Get the fixedFrameRate which is the time it takes to go
                /// from one frame to another
                /// 
                /// Get the clip length, how long the clip is in float variable
                /// 
                /// We get the total amount of frames in an animation by dividing the
                /// Clip length by its fixedFrameRate and round it to get the proper amount
                /// of keyframe in an animation
                ///</summary>


                string clipName = clip.name;
                float fixedFrameRate = 1f / clip.frameRate;
                float clipLength = clip.length;
                float totalKeyFrameCalc = (clipLength / fixedFrameRate);
                int totalKeyFrame = (int)Math.Round(totalKeyFrameCalc);

                //Put the name, total key frames, and the length into a new AnimClip
                AnimClip animClip = new AnimClip
                {
                    name = clipName,
                    totalFrames = totalKeyFrame,
                    animLength = clipLength
                };

                //For each animClip, get the time that each frame is in the animation
                double normalizedTime = 0;
                for (int j = 0; j < totalKeyFrame; j++)
                {
                    //Each loop, 'j' will be the frameIndex its getting its 
                    //frame information from and making a new keyframe and inserting said information

                    //EXAMPLE: frame 1: frameIndex = 0 timeReference = 0
                    //EXAMPLE: frame 2: frameIndex = 1 timeReference = 0.0167
                    if (j > 0) normalizedTime += fixedFrameRate;

                    Keyframe keyframe = new Keyframe
                    {
                        frameIndex = j,
                        timeReference = (float)normalizedTime
                    };

                    //Once the keyframe has been made add it into the animation clips
                    //Keys List
                    animClip.keys.Add(keyframe);
                }

                //Once the animation clip iterates through all its keyframes then put
                //it into the animationData <List> getting all the proper time for each frame.
                //in every animation that an animator has.
                animationData.Add(animClip);
            }
        }

        public int getTotalFrames(string animName)
        {
            bool foundClip = false;
            AnimClip animClip = null;
            int totalframes = 0;

            for (int i = 0; i < animationData.Count; i++)
            {
                if (animName == animationData[i].name)
                {
                    animClip = animationData[i];
                    foundClip = true;
                    break;
                }
            }

            if (foundClip)
            {
                totalframes = animClip.totalFrames;
                Debug.Log($"Found animation '{animName}' with {totalframes} frames.");
            }
            else
            {
                Debug.LogWarning($"Clip '{animName}' not found. Total frames = 0.");
            }

            return totalframes;
        }

    }
}

