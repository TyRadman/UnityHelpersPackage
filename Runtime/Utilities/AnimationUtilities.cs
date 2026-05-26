using System.Collections.Generic;
using UnityEngine;

public static class AnimationUtilities
{
    public struct AnimationEventInfo
    {
        public AnimationEvent Event;
        public float Time;

        public AnimationEventInfo(AnimationEvent evt)
        {
            Event = evt;
            Time = evt.time;
        }
    }

    public static List<AnimationEventInfo> GetEventsWithTimes(this AnimationClip clip)
    {
        List<AnimationEventInfo> result = new List<AnimationEventInfo>();

        if (clip == null)
        {
            Debug.LogError("AnimationClip is null");
            return result;
        }

        AnimationEvent[] events = clip.events;

        if (events == null || events.Length == 0)
        {
            return result;
        }

        for (int i = 0; i < events.Length; ++i)
        {
            result.Add(new AnimationEventInfo(events[i]));
        }

        return result;
    }

    public static void Play(this Animation animation, AnimationClip clip)
    {
        if (animation == null || clip == null)
        {
            return;
        }

        animation.clip = clip;
        animation.Play();
    }
}
