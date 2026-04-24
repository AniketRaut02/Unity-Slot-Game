using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Reel : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float spinSpeed = 1500f;
    [SerializeField] private float symbolHeight = 100f; // Height of one node + spacing
    [SerializeField] private int nodeCount = 5;         // total slot nodes in a reel
    [SerializeField] private SymbolData[] allSymbols; // For randomizing during the blur

    [Header("References")]
    [SerializeField] private List<SlotNode> nodes;

    //------Events---------
    public event Action OnReelStopped;

    private bool isSpinning = false;
    private bool isStopping = false;
    private SymbolData targetSymbol;

    // Bounds for the wrap-around logic
    private float topY;
    private float bottomY;

    private void Start()
    {
        // Calculate the physical boundaries based on the number of nodes
        topY = (nodeCount / 2) * symbolHeight;
        bottomY = -topY;
    }

    private void Update()
    {
        if (!isSpinning) return;

        MoveNodes();
    }

    public void StartSpinning()
    {
        isSpinning = true;
        isStopping = false;
    }

    /// <summary>
    /// Called by the ReelManager when it's time for this specific reel to lock.
    /// </summary>
    public void StopReel(SymbolData target)
    {
        targetSymbol = target;
        isStopping = true;
    }

    private void MoveNodes()
    {
        // We iterate backwards so we can safely modify the list if needed
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            SlotNode node = nodes[i];

            // Move downward
            node.RectTrans.anchoredPosition += Vector2.down * spinSpeed * Time.deltaTime;

            // Check if the node has passed the bottom threshold
            if (node.RectTrans.anchoredPosition.y <= bottomY)
            {
                WrapNodeToTop(node);
            }
        }
    }

    private void WrapNodeToTop(SlotNode node)
    {
        // 1. Find the exact Y position of the highest node currently on the belt
        float highestY = float.MinValue;
        foreach (SlotNode n in nodes)
        {
            // Ignore the node we are currently wrapping
            if (n != node && n.RectTrans.anchoredPosition.y > highestY)
            {
                highestY = n.RectTrans.anchoredPosition.y;
            }
        }

        // 2. Snap the wrapping node exactly one symbolHeight above the highest node
        // This completely eliminates floating-point drift and guarantees perfect padding.
        node.RectTrans.anchoredPosition = new Vector2(node.RectTrans.anchoredPosition.x, highestY + symbolHeight);

        // 3. Put the node at the front of the list to maintain order
        nodes.Remove(node);
        nodes.Insert(0, node);

        // 4. RNG Injection or Blur
        if (isStopping)
        {
            node.SetSymbol(targetSymbol);
            isSpinning = false;
            StartCoroutine(SnapToPayline(node));
        }
        else
        {
            SymbolData randomSymbol = allSymbols[UnityEngine.Random.Range(0, allSymbols.Length)];
            node.SetSymbol(randomSymbol);
        }
    }

    private IEnumerator SnapToPayline(SlotNode targetNode)
    {
        // Calculate the exact fixed distance this node needs to travel to hit Y: 0
        float distanceToTravel = targetNode.RectTrans.anchoredPosition.y;

        Vector2 startPos = targetNode.RectTrans.anchoredPosition;
        Vector2 targetPos = new Vector2(0, 0);

        float elapsed = 0f;
        float snapDuration = 0.3f; // Fast, punchy snap

        // Lerp the entire container or the nodes to bridge that exact distance
        while (elapsed < snapDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / snapDuration;

            // An ease-out curve calculation could go here for extra juice

            float currentY = Mathf.Lerp(startPos.y, targetPos.y, t);

            // Calculate how much we moved this frame and apply it to ALL nodes
            float deltaY = currentY - targetNode.RectTrans.anchoredPosition.y;
            foreach (SlotNode n in nodes)
            {
                n.RectTrans.anchoredPosition += new Vector2(0, deltaY);
            }

            yield return null;
        }

        // Hard lock to ensure mathematically perfect alignment at the end
        float finalCorrection = targetPos.y - targetNode.RectTrans.anchoredPosition.y;
        foreach (SlotNode n in nodes)
        {
            n.RectTrans.anchoredPosition += new Vector2(0, finalCorrection);
        }

        OnReelStopped?.Invoke();
    }
}