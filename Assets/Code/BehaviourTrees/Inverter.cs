using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inverter : Node
{
    public Inverter(string name)
    {
        this.name = name;
    }

    public override Status Process()
    {
        Status childStatus = children[0].Process();
        if (childStatus == Status.RUNNING) return Status.RUNNING;
        else if (childStatus == Status.FAILURE) return Status.SUCCESS;
        else return Status.FAILURE;
    }
}