using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : BTAgent
{

    [SerializeField] GameObject frontDoor;
    [SerializeField] GameObject backDoor;
    [SerializeField] GameObject diamond;
    [SerializeField] GameObject painting;
    [SerializeField] GameObject van;
    [SerializeField] GameObject[] art;
    [SerializeField] GameObject cop;
    [SerializeField] float lookDistance = 4f;
    [SerializeField] float lookAngle = 60f;
    GameObject pickup;

    [Range(0, 1000)]
    [SerializeField] int money = 800;

    new void Start()
    {
        base.Start();

        Leaf hasGotMoney = new Leaf("Has got money", HasMoney);
        PSelector openDoor = new PSelector("Open Door");
        Leaf openFrontDoor = new Leaf("Open front door", OpenFrontDoor, 1);
        Leaf openBackDoor = new Leaf("Open back door", OpenBackDoor, 2);
        openDoor.AddChild(openFrontDoor);
        openDoor.AddChild(openBackDoor);
        // PSelector selectObject = new PSelector("Select object");
        // Leaf goToDiamond = new Leaf("Go to diamond", GoToDiamond, 2);
        // Leaf goToPainting = new Leaf("Go to painting", GoToPainting, 1);
        RSelector selectObject = new RSelector("Select random object");
        for (int i = 0; i < art.Length; i++)
        {
            Leaf goToArt = new Leaf(art[i].name, i, GoToArt);
            selectObject.AddChild(goToArt);
        }

        Leaf goToVan = new Leaf("Go to van", GoToVan);

        Sequence runAway = new Sequence("runAway");
        Leaf canSeeCop = new Leaf("canSeeCop", CanSeeCop);
        Leaf fleeFromCop = new Leaf("fleeFromCop", FleeFromCop);
        runAway.AddChild(canSeeCop);
        runAway.AddChild(fleeFromCop);

        Inverter invertMoney = new Inverter("Invert Money");
        invertMoney.AddChild(hasGotMoney);
        Inverter cantSeeCop = new Inverter("Can't see cop");
        cantSeeCop.AddChild(canSeeCop);

        BehaviourTree stealConditions = new BehaviourTree();
        Sequence conditions = new Sequence("conditions");
        conditions.AddChild(cantSeeCop);
        conditions.AddChild(invertMoney);
        stealConditions.AddChild(conditions);

        DepSequence steal = new DepSequence("Steal", stealConditions, agent);
        steal.AddChild(invertMoney);
        steal.AddChild(openDoor);
        steal.AddChild(selectObject);
        steal.AddChild(goToVan);

        Selector stealWithFallback = new Selector("stealWithFallback");
        stealWithFallback.AddChild(steal);
        stealWithFallback.AddChild(goToVan);
        // selectObject.AddChild(goToDiamond);
        // selectObject.AddChild(goToPainting);
        Selector beThief = new Selector("Be Thief");
        beThief.AddChild(stealWithFallback);
        beThief.AddChild(runAway);

        tree.AddChild(beThief);

        tree.PrintTree();
    }

    public Node.Status HasMoney()
    {
        if (money < 500) return Node.Status.FAILURE;
        return Node.Status.SUCCESS;
    }

    public Node.Status OpenFrontDoor()
    {
        return GoToDoor(frontDoor);
    }

    public Node.Status OpenBackDoor()
    {
        return GoToDoor(backDoor);
    }

    public Node.Status CanSeeCop()
    {
        return CanSee(cop.transform.position, cop.tag, lookDistance, lookAngle);
    }

    public Node.Status FleeFromCop()
    {
        return Flee(cop.transform.position, 2f);
    }

    public Node.Status CanSee(Vector3 target, string tag, float distance, float maxAngle)
    {
        Vector3 directionToTarget = target - this.transform.position;
        float angle = Vector3.Angle(directionToTarget, this.transform.forward);
        if (angle <= maxAngle || directionToTarget.magnitude <= distance)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(this.transform.position, directionToTarget, out hitInfo))
            {
                if (hitInfo.collider.gameObject.tag == tag)
                {
                    return Node.Status.SUCCESS;
                }
            }
        }
        return Node.Status.FAILURE;
    }

    public Node.Status Flee(Vector3 location, float distance)
    {
        return GoToLocation(this.transform.position + (this.transform.position - location).normalized * distance);
    }

    public Node.Status GoToDiamond()
    {
        if (!diamond.activeSelf) return Node.Status.FAILURE;
        Node.Status status = GoToLocation(diamond.transform.position);
        if (status == Node.Status.SUCCESS)
        {
            diamond.transform.parent = transform;
            pickup = diamond;
        }
        return status;
    }

    public Node.Status GoToPainting()
    {
        if (!painting.activeSelf) return Node.Status.FAILURE;
        Node.Status status = GoToLocation(painting.transform.position);
        if (status == Node.Status.SUCCESS)
        {
            painting.transform.parent = transform;
            pickup = painting;
        }
        return status;
    }

    public Node.Status GoToArt(int i)
    {
        if (!art[i].activeSelf) return Node.Status.FAILURE;
        Node.Status status = GoToLocation(art[i].transform.position);
        if (status == Node.Status.SUCCESS)
        {
            art[i].transform.parent = transform;
            pickup = art[i];
        }
        return status;
    }

    public Node.Status GoToVan()
    {
        Node.Status status = GoToLocation(van.transform.position);
        if (status == Node.Status.SUCCESS)
        {
            if (pickup)
            {
                money += 300;
                pickup.SetActive(false);
            }
        }
        return status;
    }

    Node.Status GoToDoor(GameObject door)
    {
        Node.Status status = GoToLocation(door.transform.position);
        if (status == Node.Status.SUCCESS)
        {
            if (!door.GetComponent<Lock>().isLocked)
            {
                // door.SetActive(false);
                door.GetComponent<NavMeshObstacle>().enabled = false;
                return Node.Status.SUCCESS;
            }
            else return Node.Status.FAILURE;
        }
        return status;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, lookDistance);
    }
}