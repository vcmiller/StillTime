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

        private void OnEnable() {
            List<Command> commands = ScriptParser.ParseScript(_script.text);
            INode graphRoot = GraphBuilder.BuildGraph(commands, out INode nodeForTimeout);

            TraversalState state = new(
                graphRoot,
                nodeForTimeout,
                false,
                null, 
                Enumerable.Empty<Gate>(), 
                Enumerable.Empty<INode>(),
                Enumerable.Empty<INode>(),
                false);
            
            _nodeRunner.RunNode(state);
        }
    }
}