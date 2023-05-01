using UnityEngine;

[Tooltip(@"
/**
 * Applies local euler rotation to a GameObject with the given x, y, z rotation values per second.
 * (in the order z, x, y).
 */
")]
[DisallowMultipleComponent]
public class Rotator : MonoBehaviour
{
    [Tooltip("How many degrees should the object rotate over the X, Y, Z axis ")]
    [SerializeField] private Vector3 rotation;

    void Update()
    {
        transform.Rotate(rotation * Time.deltaTime);
    }
}
