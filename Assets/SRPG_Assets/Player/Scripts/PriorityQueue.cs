using System;
using System.Collections.Generic;

public class PriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority>
{
    private List<(TElement Element, TPriority Priority)> _nodes = new List<(TElement, TPriority)>();
    public int Count => _nodes.Count;

    public void Enqueue(TElement element, TPriority priority)
    {
        _nodes.Add((element, priority));
        int childIndex = _nodes.Count - 1;
        while (childIndex > 0)
        {
            int parentIndex = (childIndex - 1) / 2;
            if (_nodes[childIndex].Priority.CompareTo(_nodes[parentIndex].Priority) >= 0)
            {
                break;
            }
            Swap(childIndex, parentIndex);
            childIndex = parentIndex;
        }
    }

    public TElement Dequeue()
    {
        TElement root = _nodes[0].Element;
        _nodes[0] = _nodes[_nodes.Count - 1];
        _nodes.RemoveAt(_nodes.Count - 1);

        int parentIndex = 0;
        while (true)
        {
            int leftChild = parentIndex * 2 + 1;
            int rightChild = parentIndex * 2 + 2;
            int smallest = parentIndex;

            if (leftChild < _nodes.Count && _nodes[leftChild].Priority.CompareTo(_nodes[smallest].Priority) < 0)
                smallest = leftChild;
            if (rightChild < _nodes.Count && _nodes[rightChild].Priority.CompareTo(_nodes[smallest].Priority) < 0)
                smallest = rightChild;

            if (smallest == parentIndex) break;
            Swap(parentIndex, smallest);
            parentIndex = smallest;
        }
        return root;
    }

    private void Swap(int i, int j)
    {
        var temp = _nodes[i];
        _nodes[i] = _nodes[j];
        _nodes[j] = temp;
    }
}

