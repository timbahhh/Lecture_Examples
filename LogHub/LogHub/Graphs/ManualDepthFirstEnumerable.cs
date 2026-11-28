using System.Collections;

namespace LogHub.Graphs;

public class ManualDepthFirstEnumerable : IEnumerable<Node>
{
    private readonly Node _root;

    public ManualDepthFirstEnumerable(Node root)
    {
        _root = root;
    }

    public IEnumerator<Node> GetEnumerator()
    {
        return new ManualDepthFirstEnumerator(_root);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private class ManualDepthFirstEnumerator : IEnumerator<Node>
    {
        private readonly Node _root;
        private Stack<Node> _stack = new();
        private HashSet<Node> _visited = new();

        public Node Current { get; private set; } = null!;

        object IEnumerator.Current => Current;

        public ManualDepthFirstEnumerator(Node root)
        {
            _root = root;
            Reset();
        }

        public bool MoveNext()
        {
            while (_stack.Count > 0)
            {
                var node = _stack.Pop();

                if (!_visited.Add(node))
                    continue;

                foreach (var dependency in node.Dependencies.AsEnumerable().Reverse())
                    _stack.Push(dependency);

                Current = node;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            _stack.Clear();
            _visited.Clear();
            _stack.Push(_root);
            Current = null!;
        }

        public void Dispose()
        {
            _stack.Clear();
            _visited.Clear();
        }
    }
}