using System.Collections.Generic;
using XNode;

namespace BTSystem.Core
{
    public static class Extentions
    {
        public static List<Node> GetConnectionsFrom(this Node _node, string _exit)
        {
            List<Node> connections = new List<Node>();

            //Filter through ports till _exit name is found.
            foreach (var port in _node.Ports)
            {
                if (port.fieldName == _exit)
                {
                    for (int i = 0; i < port.ConnectionCount; i++)
                    {
                        connections.Add(port.GetConnection(i).node);
                    }
                    break;
                }
            }

            if(connections.Count > 1)
            {
                //Sort based upon height
                connections.Sort((a, b) => a.position.y.CompareTo(b.position.y));
            }
            
            return connections;
        }
    }
}
