using System;
using AlgoDat;

namespace beispiel01
{
    class Program
    {
        static void Main(string[] args)
        {
            TicTacToeGraph<TicTacToeNode> graph = new();
            graph.Generate();
        }
    }
}
