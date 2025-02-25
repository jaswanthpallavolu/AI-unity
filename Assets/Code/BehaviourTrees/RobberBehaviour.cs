using System;
using System.Collections;
using System.Collections.Generic;
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
    GameObject pickup;

    [Range(0, 1000)]
    [SerializeField] int money = 800;

    new void Start()
    {
        base.Start();
        Sequence steal = new Sequence("Steal");
        Leaf hasGotMoney = new Leaf("Has got money", HasMoney);
        PSelector openDoor = new PSelector("Open Door");
        Leaf openFrontDoor = new Leaf("Open front door", OpenFrontDoor, 1);
        Leaf openBackDoor = new Leaf("Open back door", OpenBackDoor, 2);
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

        Inverter invertMoney = new Inverter("Invert Money");
        invertMoney.AddChild(hasGotMoney);
        steal.AddChild(invertMoney);

        openDoor.AddChild(openFrontDoor);
        openDoor.AddChild(openBackDoor);
        steal.AddChild(openDoor);

        // selectObject.AddChild(goToDiamond);
        // selectObject.AddChild(goToPainting);
        steal.AddChild(selectObject);

        steal.AddChild(goToVan);
        tree.AddChild(steal);

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
            money += 300;
            pickup.SetActive(false);
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
}