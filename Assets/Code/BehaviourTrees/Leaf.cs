using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : Node
{
    public delegate Status Tick();
    public Tick ProcessMethod;
    public delegate Status TickM(int index);
    public TickM ProcessMethodM;
    int index;

    public Leaf() { }
    public Leaf(string name, Tick pm)
    {
        this.name = name;
        ProcessMethod = pm;
    }
    public Leaf(string name, int index, TickM pm)
    {
        this.name = name;
        this.index = index;
        ProcessMethodM = pm;
    }
    public Leaf(string name, Tick pm, int order)
    {
        this.name = name;
        ProcessMethod = pm;
        sortOrder = order;
    }

    public override Status Process()
    {
        Status status;
        if (ProcessMethod != null)
            status = ProcessMethod();
        else if (ProcessMethodM != null)
            status = ProcessMethodM(index);
        else status = Status.FAILURE;
        Debug.Log(name + " " + status);
        return status;
    }
}