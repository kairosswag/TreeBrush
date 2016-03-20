using System;
using UnityEngine;

namespace TreeBrush
{
    public class Main : IMod
    {
        private GameObject _go;

        public void onEnabled()
        {
            _go = new GameObject();
            _go.AddComponent<TreeBrush>();
        }

        public void onDisabled()
        {
            UnityEngine.Object.Destroy(_go);
        }

        public string Description
        {
            get
            {
                return "Provides a brush tool that helps creating Forests.";
            }
        }

        public string Identifier
        {
            get; set;
        }

        public string Name
        {
            get
            {
                return "Tree Brush";
            }
        }

    }
}
