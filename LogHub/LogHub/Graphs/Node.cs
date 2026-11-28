namespace LogHub.Graphs;

public class Node
{
    public string Name { get; }

    public List<Node> Dependencies { get; } = new();

    public Node(string name)
    {
        Name = name;
    }

    public void AddDependency(Node dependency)
    {
        Dependencies.Add(dependency);
    }

    public override string ToString()
    {
        return Name;
    }
}
