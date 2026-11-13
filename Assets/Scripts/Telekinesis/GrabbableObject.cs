using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableObject : MonoBehaviour
{
   public Rigidbody rb {get; private set;}

   private void Awake()
   {
    rb = GetComponent<Rigidbody>();
   }
}
