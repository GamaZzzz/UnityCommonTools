using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(ParticleSystem))]
public class PathParticle : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem particle;

    void Start()
    {
        if (!particle)
        {
            particle = GetComponent<ParticleSystem>();
        }
    }

#if UNITY_EDITOR
    void Reset()
    {
        Start();
    }
#endif

    public void DrawPath(List<Vector3> points)
    {
        Queue<FrameDate> frames;
        float distance = ParticleUtils.CalculateDirection(points, out frames);
        particle.transform.position = points[0];
        AnimationCurve curve_X;
        AnimationCurve curve_Y;
        AnimationCurve curve_Z;
        float lifeTime = distance / particle.startLifetime;
        ParticleUtils.MakeCurve(frames, distance, lifeTime, out curve_X, out curve_Y, out curve_Z);
        var vel = particle.velocityOverLifetime;
        vel.enabled = true;
        vel.space = ParticleSystemSimulationSpace.Local;
        vel.x = new ParticleSystem.MinMaxCurve(lifeTime, curve_X);
        vel.y = new ParticleSystem.MinMaxCurve(lifeTime, curve_Y);
        vel.z = new ParticleSystem.MinMaxCurve(lifeTime, curve_Z);
    }
}

