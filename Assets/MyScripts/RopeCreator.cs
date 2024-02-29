using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeCreator : MonoBehaviour
{
    public GameObject ropeSegmentPrefab; // Assign in Inspector
    public int segmentsCount = 10;       // Number of segments in the rope
    private LineRenderer lineRenderer;   // For visual representation of the rope

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        CreateRope();

        foreach (Transform child in transform)
        {
            var meshRenderer = child.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false; // Hide the segment
            }
        }
    }

    void Update()
    {
        UpdateLineRenderer();
    }

    
    void CreateRope()
    {
        GameObject previousSegment = this.gameObject; // Start with the Rope GameObject itself for the first segment
        float segmentLength = 1.0f; // Assuming the length of your segment is equal to its scale in y

        for (int i = 0; i < segmentsCount; i++)
        {
            // Calculate the position for the next segment
            Vector3 segmentPosition = transform.position + Vector3.down * (segmentLength * i);

            GameObject segment = Instantiate(ropeSegmentPrefab, segmentPosition, Quaternion.identity, transform);

            if(i == 0)
            {
                //segment.GetComponent<Rigidbody>().isKinematic = true;
            }

            if (i >= 0) // Skip the first segment which is the Rope GameObject itself
            {
                HingeJoint joint = segment.AddComponent<HingeJoint>();
                joint.connectedBody = previousSegment.GetComponent<Rigidbody>();

                // Configure the joint's anchor points
                joint.anchor = Vector3.up * (segmentLength / 2.0f); // Anchor at the top of the current segment
                joint.connectedAnchor = Vector3.down * (segmentLength / 2.0f); // Connect to the bottom of the previous segment

                // Additional joint configuration as needed...
            }

            previousSegment = segment;
        }

        // Adjust the line renderer to have as many points as there are segments
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = segmentsCount; // +1 to include the position of the Rope GameObject itself
        }

        
    }
    

    /*
    void CreateRope()
    {
        GameObject previousSegment = this.gameObject; // Start with the Rope GameObject itself for the first segment
        for (int i = 0; i < segmentsCount; i++)
        {
            // Instantiate segment as a child of the Rope GameObject
            GameObject segment = Instantiate(ropeSegmentPrefab, transform.position + Vector3.down * (i * 0.5f), Quaternion.identity, transform);

            HingeJoint joint = segment.AddComponent<HingeJoint>();
            joint.connectedBody = previousSegment.GetComponent<Rigidbody>();

            // If it's the first segment, you might want to adjust its anchor or make it static
            if (i == 0)
            {
                // Option 1: Make the first segment static (not affected by physics)
                segment.GetComponent<Rigidbody>().isKinematic = true;

                // Option 2: Attach the first segment's joint to an immovable object or configure it differently
                //joint.autoConfigureConnectedAnchor = true; // You might need to adjust this and the anchor points depending on your setup
            }

            if (i > 0) // Skip the first segment which is the Rope GameObject itself
            {
                HingeJoint joint = segment.AddComponent<HingeJoint>();
                joint.connectedBody = previousSegment.GetComponent<Rigidbody>();

                // Configure the joint's anchor points
                joint.anchor = Vector3.up * (segmentLength / 2.0f); // Anchor at the top of the current segment
                joint.connectedAnchor = Vector3.down * (segmentLength / 2.0f); // Connect to the bottom of the previous segment

                // Additional joint configuration as needed...
            }

            previousSegment = segment;
        }

        // Adjust the line renderer to have as many points as there are segments
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = segmentsCount + 1; // +1 to include the position of the Rope GameObject itself
        }
    }
    */

    void UpdateLineRenderer()
    {
        if (lineRenderer == null) return;

        // Update the line renderer's positions to match the rope segments
        for (int i = 0; i < segmentsCount; i++)
        {
            if (i < transform.childCount)
            {
                lineRenderer.SetPosition(i, transform.GetChild(i).position);
            }
        }
    }
}
