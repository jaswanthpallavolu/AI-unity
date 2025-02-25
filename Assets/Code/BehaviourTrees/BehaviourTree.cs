using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTree : Node
{
    public BehaviourTree()
    {
        name = "Tree";
    }
    public override Status Process()
    {
        if (children.Count == 0) return Status.SUCCESS;
        return children[currentChild].Process();
    }

    public struct NodeLevel
    {
        public int level;
        public Node node;
    }

    public void PrintTree()
    {
        string treePrintout = "";
        Node currentNode = this;
        Stack<NodeLevel> treeStack = new Stack<NodeLevel>();
        treeStack.Push(new NodeLevel { level = 0, node = currentNode });

        while (treeStack.Count != 0)
        {
            NodeLevel nodeLevel = treeStack.Pop();
            currentNode = nodeLevel.node;
            treePrintout += new string('-', nodeLevel.level) + currentNode.name + "\n";
            for (int i = currentNode.children.Count - 1; i >= 0; i--)
            {
                treeStack.Push(new NodeLevel { level = nodeLevel.level + 1, node = currentNode.children[i] });
            }
        }
        Debug.Log(treePrintout);
    }
}