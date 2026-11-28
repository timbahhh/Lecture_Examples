namespace LogHub.Graphs;
public class ServiceGraph
{
    public IEnumerable<Node> DepthFirst(Node? root)
    {
        if (root is null)
            yield break;
        var visited = new HashSet<Node>();
        var stack = new Stack<Node>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            var current = stack.Pop();

            if (!visited.Add(current))
                continue;

            yield return current;

            foreach (var dependency in current.Dependencies.AsEnumerable().Reverse())
            {
                if (!visited.Contains(dependency))
                    stack.Push(dependency);
            }
        }
    }
    public IEnumerable<Node> BreadthFirst(Node? root)
    {
        if (root is null)
            yield break;
        var visited = new HashSet<Node>();
        var queue = new Queue<Node>();
        queue.Enqueue(root);
        visited.Add(root);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            yield return current;

            foreach (var dependency in current.Dependencies)
            {
                if (visited.Add(dependency))
                    queue.Enqueue(dependency);
            }
        }
    }

    public List<Node>? FindCycle(Node? root)
    {
        if (root is null)
            return null;
        var visited = new HashSet<Node>();
        var active = new HashSet<Node>();
        var path = new List<Node>();

        List<Node>? Search(Node current)
        {
            if (active.Contains(current))
            {
                var startIndex = path.IndexOf(current);
                return path.Skip(startIndex).Append(current).ToList();
            }

            if (visited.Contains(current))
                return null;

            visited.Add(current);
            active.Add(current);
            path.Add(current);

            foreach (var dependency in current.Dependencies)
            {
                var cycle = Search(dependency);

                if (cycle is not null)
                    return cycle;
            }

            active.Remove(current);
            path.RemoveAt(path.Count - 1);

            return null;
        }

        return Search(root);
    }
    public IEnumerable<Node> DepthFirstYield(Node root)
    {
        var visited = new HashSet<Node>();

        IEnumerable<Node> Traverse(Node current)
        {
            if (!visited.Add(current))
                yield break;

            yield return current;

            foreach (var dependency in current.Dependencies)
            {
                foreach (var node in Traverse(dependency))
                {
                    yield return node;
                }
            }
        }

        return Traverse(root);
    }
}

