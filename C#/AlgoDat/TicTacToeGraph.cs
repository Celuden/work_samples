using System;

namespace AlgoDat
{
    public class TicTacToeGraph<T> where T : TicTacToeNode
    {
        string currentString = "X";

        /// <summary>Generates the tic tac toe play graph</summary>
        /// <returns>A graph representation of the play graph</returns>
        public Graph<T> Generate()
        {
            Graph<T> graph = new();
            TicTacToeNode startNode = new();
            graph.AddNode((T)startNode);

            Dictionary<int, DoubleLinkedList<TicTacToeNode>> nodeDict = new();
            nodeDict.Add(10, new());
            nodeDict.Get(10).Append(startNode);
            int count = 0;

            // Loop for the several game-states, maximum of 10 layers in total (including empty startNode)
            for (int i = 9; i > 0; i--)
            {
                nodeDict.Add(i, new());

                // Generate nodes for each node from previous layer
                foreach (TicTacToeNode TNode in nodeDict.Get(i+1))
                {
                    for (int u = 0; u < 3; u++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            TicTacToeNode newNode = TransferToNewNode(TNode);

                            // Check if the game is already completed
                            if (!CheckForCompletion(newNode, currentString))
                            {
                                // Check if something can be written inside a field
                                if (newNode.TTTNode[u, j] == "-")
                                {
                                    newNode.TTTNode[u, j] = currentString;

                                    // Check if node already exists on this layer, if not, create edge and add it to list
                                    if (nodeDict.Get(i).Search(newNode) == null)
                                    {
                                        graph.AddEdge((T)TNode, (T)newNode);
                                        nodeDict.Get(i).Append(newNode);
                                        count++;
                                    }
                                    // If node already exists on this layer, create edge to it, don't add it to list
                                    else
                                    {
                                        // Look for a node in the current layer, that is equal to newNode and add edge from previous layer to this node
                                        foreach (TicTacToeNode node in nodeDict.Get(i))
                                        {
                                            if (node.CompareTo(newNode) == 0)
                                            {
                                                graph.AddEdge((T)TNode, (T)node);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                // Switch "Players"
                FlipTTTString(currentString);
            }
            Console.WriteLine(count);
            return graph;
        }

        /// <summary>Flips between X and O, the current player</summary>
        private void FlipTTTString(string _currentString)
        {
            if (_currentString == "X")
            {
                currentString = "O";
            }
            else
            {
                currentString = "X";
            }
        }

        /// <summary>Transfers array from List to a new node</summary>
        /// <returns>An exact copy of the node from the list</returns>
        private TicTacToeNode TransferToNewNode(TicTacToeNode listNode)
        {
            TicTacToeNode newNode = new();
            for (int i = 0; i < 3; i++)
            {
                for (int u = 0; u < 3; u++)
                {
                    newNode.TTTNode[i, u] = listNode.TTTNode[i, u];
                }
            }
            return newNode;
        }

        /// <summary>Checks if the game is complete based on the previous layer's symbol</summary>
        /// <returns>A bool that is either true if someone won, or false if the game is still going</returns>
        private bool CheckForCompletion(TicTacToeNode node, string _currentString)
        {
            // CheckForCompletion checks the symbol from the previous layer, which is why it needs to be flipped to correctly check if a game is over or not since it was the previous player's turn
            // Standard case is X
            string _previousString = "X";
            if (_currentString == "X")
            {
                _previousString = "O";
            }
            
            for (int i = 0; i < 3; i++)
            {
                for (int u = 0; u < 3; u++)
                {
                    // Check if the current position is the same as the current player's symbolstring
                    if (node.TTTNode[i, u] == _previousString)
                    {
                        // Top-Left-Position: possible win all directions
                        if (i == 0 && u == 0)
                        {
                            // Check row
                            if (node.TTTNode[i, u+1] == _previousString && node.TTTNode[i, u+2] == _previousString)
                            {
                                return true;
                            }

                            // Check column
                            if (node.TTTNode[i+1, u] == _previousString && node.TTTNode[i+2, u] == _previousString)
                            {
                                return true;
                            }

                            // Check diagonal
                            if (node.TTTNode[i+1, u+1] == _previousString && node.TTTNode[i+2, u+2] == _previousString)
                            {
                                return true;
                            }
                        }

                        // Top-Middle-Position: possible column win
                        if (i == 0 && u == 1)
                        {
                            // Check column
                            if (node.TTTNode[i+1, u] == _previousString && node.TTTNode[i+2, u] == _previousString)
                            {
                                return true;
                            }
                        }

                        // Top-Right-Position: possible win column and reverse diagonal
                        if (i == 0 && u == 2)
                        {
                            // Check column
                            if (node.TTTNode[i+1, u] == _previousString && node.TTTNode[i+2, u] == _previousString)
                            {
                                return true;
                            }

                            // Check reverse diagonal
                            if (node.TTTNode[i+1, u-1] == _previousString && node.TTTNode[i+2, u-2] == _previousString)
                            {
                                return true;
                            }
                        }

                        // Middle-Left-Position: possible win row
                        if (i == 1 && u == 0)
                        {
                            // Check row
                            if (node.TTTNode[i, u+1] == _previousString && node.TTTNode[i, u+2] == _previousString)
                            {
                                return true;
                            }
                        }

                        // Bottom-Left-Position: possible win row
                        if (i == 2 && u == 0)
                        {
                            // Check row
                            if (node.TTTNode[i, u+1] == _previousString && node.TTTNode[i, u+2] == _previousString)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            // Default case is, that the game is not over yet
            return false;
        }
    }

    public class TicTacToeNode : IComparable<TicTacToeNode>
    {
        public string[,] TTTNode { get; set; }
        public TicTacToeNode()
        {
            TTTNode = new string[3, 3] {{"-", "-", "-"}, {"-", "-", "-"}, {"-", "-", "-"}};
        }
        public int CompareTo(TicTacToeNode other)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int u = 0; u < 3; u++)
                {
                    if (TTTNode[i, u].CompareTo(other.TTTNode[i, u]) != 0)
                    {
                        return 1;
                    }
                }
            }
            return 0;
        }
        public override string ToString()
        {
            string buildString = "";

            for (int i = 0; i < 3; i++)
            {
                for (int u = 0; u < 3; u++)
                {
                    buildString += TTTNode[i, u].ToString();
                }
                buildString += "\r\n";
            }
            return buildString;
        }
    }
}
