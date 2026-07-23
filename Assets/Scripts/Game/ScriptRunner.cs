using System;
using System.Collections.Generic;
using System.Linq;
using Commands;
using Nodes;
using UnityEngine;

namespace Game {
    public class ScriptRunner : MonoBehaviour {
        public TextAsset _script;
        public NodeRunner _nodeRunner;
        public float _timeBudget;

        private void OnEnable() {
            List<Command> commands = ScriptParser.ParseScript(_script.text);
            INode graphRoot = GraphBuilder.BuildGraph(commands);

            TraversalState state = new(graphRoot, _timeBudget, Enumerable.Empty<Gate>(), Enumerable.Empty<INode>(),
                Enumerable.Empty<INode>(), false);
            
            _nodeRunner.RunNode(state);
        }
    }
}