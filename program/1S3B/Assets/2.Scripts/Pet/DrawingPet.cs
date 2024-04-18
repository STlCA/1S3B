using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingPet : MonoBehaviour
{
    public List<Pet> Pets = new List<Pet>();

    public void Drawing()
    {
        GetPet();
    }

    private void GetPet()
    {
        int PetNumber = Random.Range(0, Pets.Count);

        switch (PetNumber)
        {
            case 0:
                break;
            default:
                break;
        }
    }
}
