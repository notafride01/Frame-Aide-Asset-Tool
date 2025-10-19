> **What is FrameAide?**

Frame Aide is a tool that allows developers to get the frame data of an animation. Allowing the developers to get the current frame and play an animation at a specific frame.

> **Why use FrameAide?**
>
I found it difficult to play an animation at a specific frame, rather than using Unity's normalized Time or Fixed time, and the only other way you can allow certain things to happen at a specific frame 
is the use of animation events,

So this is a way to allow Unity developers who want an overhead function that does not require making animation events or needing information on an animation regarding its frame data

> **How?**
> 
You can go to the Unity Asset Store and download it from my store page to get access to it

> **Functionalities**
> 
There are 5 different function calls that can be used when the FrameAide.cs is attached to a gameobject with an animator component.

**PlayAtFrame (string animName, int frameIndex)** - Plays an animation at a specific frame

**GetCurrentFrame (Animator animator)** - When given a valid animator, it returns the current frame of the animation it's playing

**GetTotalFrames (string animName, int frameIndex)** - Plays an animation at a specific frame

**GetTotalFrames(string animName)** - When given an animation name, it will return the total number of frames that animation has

**GetCurrentClipName (Animator animator)** - return the clip name the animator is currently playing








