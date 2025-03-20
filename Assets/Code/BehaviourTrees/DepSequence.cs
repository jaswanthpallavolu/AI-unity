using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DepSequence : Node
{
    BehaviourTree dependancy;
    NavMeshAgent agent;
    public DepSequence(string name, BehaviourTree tree, NavMeshAgent agent)
    {
        this.name = name;
        dependancy = tree;
        this.agent = agent;
    }

    public override Status Process()
    {
        if (dependancy.Process() == Status.FAILURE)
        {
            agent.ResetPath();
            Reset();
            return Status.FAILURE;
        }
        Status childStatus = children[currentChild].Process();
        if (childStatus == Status.RUNNING) return Status.RUNNING;
        else if (childStatus == Status.FAILURE) return Status.FAILURE;

        currentChild++;
        if (currentChild >= children.Count)
        {
            currentChild = 0;
            return Status.SUCCESS;
        }
        return Status.RUNNING;
    }
}