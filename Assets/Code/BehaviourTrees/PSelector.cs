using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSelector : Node
{
    bool sorted = false;
    Node[] nodeArray;
    public PSelector(string name)
    {
        this.name = name;
    }

    void OrderNodes()
    {
        nodeArray = children.ToArray();
        Sort(nodeArray, 0, children.Count - 1);
        children = new List<Node>(nodeArray);
    }

    public override Status Process()
    {
        if (!sorted)
        {
            OrderNodes();
            sorted = true;
        }
        Status childStatus = children[currentChild].Process();
        if (childStatus == Status.RUNNING) return Status.RUNNING;
        else if (childStatus == Status.SUCCESS)
        {
            children[currentChild].sortOrder = 1;
            currentChild = 0;
            sorted = false;
            return Status.SUCCESS;
        }
        else children[currentChild].sortOrder = 10;

        currentChild++;
        if (currentChild >= children.Count)
        {
            currentChild = 0;
            sorted = false;
            return Status.FAILURE;
        }
        return Status.RUNNING;
    }

    //QuickSort
    //Adapted from: https://exceptionnotfound.net/quick-sort-csharp-the-sorting-algorithm-family-reunion/
    int Partition(Node[] array, int low,
                                int high)
    {
        Node pivot = array[high];

        int lowIndex = (low - 1);

        //2. Reorder the collection.
        for (int j = low; j < high; j++)
        {
            if (array[j].sortOrder <= pivot.sortOrder)
            {
                lowIndex++;

                Node temp = array[lowIndex];
                array[lowIndex] = array[j];
                array[j] = temp;
            }
        }

        Node temp1 = array[lowIndex + 1];
        array[lowIndex + 1] = array[high];
        array[high] = temp1;

        return lowIndex + 1;
    }

    public void Sort(Node[] array, int low, int high)
    {
        if (low < high)
        {
            int partitionIndex = Partition(array, low, high);
            Sort(array, low, partitionIndex - 1);
            Sort(array, partitionIndex + 1, high);
        }
    }
}