using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using System.Diagnostics;
using Unity.Burst;
using Unity.Mathematics;

public class ParallelReplicator : MonoBehaviour
{
    [Sirenix.OdinInspector.Button]
    public void TestJob()
    {
        const int n = 10000;

        Stopwatch st = new Stopwatch();
        st.Start();

        var original = new NativeArray<int>(n, Allocator.TempJob);
        // var clone1 = new NativeArray<int>(n, Allocator.Persistent);
        // var clone2 = new NativeArray<int>(n, Allocator.Persistent);


        var originalJob = new SetJob
        {
            m_SetJob = original,
        };

        // originalJob.Schedule().Complete();
        var originaljob = originalJob.Schedule(original.Length, 2);
        originaljob.Complete();

        for (int i = 0; i < original.Length; i++)
        {
            UnityEngine.Debug.Log("Value: " + original[i]);
        }

        st.Stop();

        UnityEngine.Debug.Log("Time: " + st.ElapsedMilliseconds);

        original.Dispose();

        // var job1 = new MyJob
        // {
        //     input = original,
        //     output = clone1
        // };
        // var job2 = new MyJob
        // {
        //     input = original,
        //     output = clone2
        // };

        // var jobX = new MyJob
        // {
        //     input = original,
        //     output = clone2
        // };

        // // Run the jobs in parallel.
        // var jobs = JobHandle.CombineDependencies(job1.Schedule(), job2.Schedule());

        // // jobX.Schedule(); // Not allowed, throws exception because job2 is writing into copy2.

        // jobs.Complete();

        // jobX.Schedule().Complete(); // Allowed, because job2 has been completed by now.

        // original.Dispose();
        // clone1.Dispose();
        // clone2.Dispose();
    }

    [Sirenix.OdinInspector.Button]
    public void TestNormal()
    {
        const int n = 10000;

        Stopwatch st = new Stopwatch();
        st.Start();

        int[] original = new int[n];

        for (int i = 0; i < original.Length; i++)
        {
            original[i] = i;
        }

        for (int i = 0; i < original.Length; i++)
        {
            UnityEngine.Debug.Log("Value: " + original[i]);
        }

        st.Stop();

        UnityEngine.Debug.Log("Time: " + st.ElapsedMilliseconds);
    }
}

[BurstCompile]
public struct MyJob : IJob
{
    [ReadOnly]
    public NativeArray<int> input;

    public NativeArray<int> output;

    public void Execute()
    {
        for (var i = 0; i < output.Length; ++i)
            output[i] = input[i];
    }
}

[BurstCompile]
public struct SetJob : IJobParallelFor
{
    public NativeArray<int> m_SetJob;

    public void Execute(int i)
    {
        m_SetJob[i] = i;
    }
}
