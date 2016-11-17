using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;



public class PathParticle : MonoBehaviour
{
    public ParticleSystem Particle;
    public Transform[] wayPoints;

    void Start()
    {
        if (wayPoints.Length > 1)
        {
            Queue<FrameDate> frames;
            float distance = ParticleUtils.CalculateDirection(new List<Transform>(wayPoints), out frames);
            Particle.transform.position = wayPoints[0].transform.position;
            AnimationCurve curve_X;
            AnimationCurve curve_Y;
            AnimationCurve curve_Z;
            float lifeTime = distance / Particle.startLifetime;
            ParticleUtils.MakeCurve(frames, distance, lifeTime, out curve_X, out curve_Y, out curve_Z);
            var vel = Particle.velocityOverLifetime;
            vel.enabled = true;
            vel.space = ParticleSystemSimulationSpace.Local;
            vel.x = new ParticleSystem.MinMaxCurve(lifeTime, curve_X);
            vel.y = new ParticleSystem.MinMaxCurve(lifeTime, curve_Y);
            vel.z = new ParticleSystem.MinMaxCurve(lifeTime, curve_Z);
        }
    }
}

