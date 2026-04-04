// using UnityEngine;

// public class SpriteMover : MonoBehaviour
// {
//     // The target position to move to (set in the Inspector)
//     public Vector3 targetPoint;
//     public Vector3 startPoint;
//     // The movement speed in units per second (set in the Inspector)
//     public float speed = 5.0f;

//     void Start()
//     {
//         transform.position = startPoint;
//     }

//     void Update()
//     {
//         // Move the sprite from its current position towards the target point
//         transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);

//         // Optional: Check if the sprite has reached the target position
//         if (Vector3.Distance(transform.position, targetPoint) < 0.001f)
//         {
//             // Logic for when the destination is reached can go here
//             // e.g., set a new target, destroy the object, etc.
//         }
//     }
// }

