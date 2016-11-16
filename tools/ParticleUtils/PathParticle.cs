using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


struct FrameDate
{
    public FrameDate(Vector3 direction, float distance)
    {
        Direction = direction;
        Distance = distance;
    }
    public Vector3 Direction;
    public float Distance;
}

public class PathParticle : MonoBehaviour
{
    public ParticleSystem Particle;
    public Transform[] wayPoints;

    void Start()
    {
        if (wayPoints.Length > 1)
        {
            Queue<FrameDate> frames;
            float distance = CalculateDirection(wayPoints, out frames);
            AnimationCurve curve_X;
            AnimationCurve curve_Y;
            AnimationCurve curve_Z;
            float lifeTime = distance / Particle.startLifetime;
            MakeCurve(frames, distance, lifeTime, out curve_X, out curve_Y, out curve_Z);
            var vel = Particle.velocityOverLifetime;
            vel.enabled = true;
            vel.space = ParticleSystemSimulationSpace.Local;
            vel.x = new ParticleSystem.MinMaxCurve(lifeTime, curve_X);
            vel.y = new ParticleSystem.MinMaxCurve(lifeTime, curve_Y);
            vel.z = new ParticleSystem.MinMaxCurve(lifeTime, curve_Z);
        }
    }

    float CalculateDirection(Transform[] wayPoints, out Queue<FrameDate> frames)
    {
        int size = wayPoints.Length;
        float totalDistance = 0;
        frames = new Queue<FrameDate>();

        for (int i = 1; i < size; ++i)
        {
            Vector3 dir = wayPoints[i].position - wayPoints[i - 1].position;
            float dis = Vector3.Distance(wayPoints[i].position, wayPoints[i - 1].position);
            totalDistance += dis;
            dir.Normalize();
            frames.Enqueue(new FrameDate(dir, dis));
        }
        return totalDistance;
    }

    void MakeCurve(Queue<FrameDate> frames,
        float totalDistance,
        float totalTime,
        out AnimationCurve curve_X,
        out AnimationCurve curve_Y,
        out AnimationCurve curve_Z)
    {
        curve_X = new AnimationCurve();
        curve_Y = new AnimationCurve();
        curve_Z = new AnimationCurve();
        float curTime = 0;
        while (frames.Count > 0)
        {
            FrameDate data = frames.Dequeue();
            curve_X.AddKey(new Keyframe(curTime, data.Direction.x, float.PositiveInfinity, float.PositiveInfinity));
            curve_Y.AddKey(new Keyframe(curTime, data.Direction.y, float.PositiveInfinity, float.PositiveInfinity));
            curve_Z.AddKey(new Keyframe(curTime, data.Direction.z, float.PositiveInfinity, float.PositiveInfinity));
            curTime += (data.Distance / totalDistance);
        }
    }
}

