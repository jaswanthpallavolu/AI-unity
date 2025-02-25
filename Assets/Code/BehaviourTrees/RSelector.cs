using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSelector : Node
{
    bool shuffled = false;
    public RSelector(string name)
    {
        this.name = name;
    }

    public override Status Process()
    {
        if (!shuffled)
        {
            children.Shuffle();
            shuffled = true;
        }
        Status childStatus = children[currentChild].Process();
        if (childStatus == Status.RUNNING) return Status.RUNNING;
        else if (childStatus == Status.SUCCESS)
        {
            currentChild = 0;
            shuffled = false;
            return Status.SUCCESS;
        }

        currentChild++;
        if (currentChild >= children.Count)
        {
            currentChild = 0;
            shuffled = false;
            return Status.FAILURE;
        }
        return Status.RUNNING;
    }
}