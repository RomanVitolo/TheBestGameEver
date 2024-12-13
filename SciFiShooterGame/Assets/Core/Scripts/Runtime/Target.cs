using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Target : MonoBehaviour
{
    private void OnEnable()
    {   
        this.gameObject.layer = LayerMask.NameToLayer("Enemy");
    }
}
