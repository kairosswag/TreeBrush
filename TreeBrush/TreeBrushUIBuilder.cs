using Parkitect.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace TreeBrush
{
    /// <summary>
    /// This class cobbles together the UI Elements. 
    /// 
    /// Many thanks to Jay2645, for providing example and tutorial and most of this class!
    /// <a href="https://parkitectnexus.com/forum/4/modding-technical/212/how-to-work-with-parkitects-native-window-system-instead-of-ongui">Link!</a>
    /// </summary>
    class TreeBrushUIBuilder : MonoBehaviour
    {

        //-----------------------------Copy Pasta----------------------------//
        public static GameObject rectTfmPrefab
        {
            get
            {
                // Welcome to the hackiest place on earth
                if (_rectTfmPrefab == null)
                {
                    // RectTransforms can't be added to objects directly; you have to instantiate a prefab
                    // It's a pain to deal with the assetbundles needed to do that, so we're just going to steal one
                    RectTransform foundRectTfm = FindObjectOfType<RectTransform>();
                    GameObject instantiatedGO = Instantiate<GameObject>(foundRectTfm.gameObject);
                    foreach (Transform child in instantiatedGO.transform)
                    {
                        Destroy(child);
                    }

                    UnityEngine.UI.Graphic imageComponent = instantiatedGO.GetComponent<Graphic>();
                    if (imageComponent != null)
                    {
                        DestroyImmediate(imageComponent);
                    }

                    List<Component> childComponents = new List<Component>(instantiatedGO.GetComponents<Component>());
                    // Iterate an arbitrary number of times to try and destroy all child components
                    for (int i = 0; i < 4; i++)
                    {
                        foreach (Component component in childComponents)
                        {
                            if (!(component is RectTransform))
                            {
                                DestroyImmediate(component);
                            }
                        }
                        childComponents = new List<Component>(instantiatedGO.GetComponents<Component>());
                        if (i % 2 == 0)
                        {
                            // Reverse the order to vary up the order in which we try to destroy the components
                            childComponents.Reverse();
                        }
                    }
                    _rectTfmPrefab = instantiatedGO;

                    // Reset the Rect Transform:
                    RectTransform rectTfm = _rectTfmPrefab.GetComponent<RectTransform>();
                    rectTfm.anchorMin = Vector2.zero;
                    rectTfm.anchorMax = Vector2.one;
                    rectTfm.anchoredPosition = Vector2.zero;
                    rectTfm.sizeDelta = Vector2.zero;
                    rectTfm.pivot = new Vector2(0.5f, 0.5f);
                }
                return _rectTfmPrefab;
            }
        }
        private static GameObject _rectTfmPrefab;
        //------------------------------ /copy -----------------------------//

        private UIMenuButton tbBuilderButton = null;
        private UIWindow tbWindow = null;
        private void Start()
        {
            MakeBrushMenuTab();
         // MakeBrushWindow();
        }

        private void MakeBrushMenuTab()
        {
            UIMenuButton[] allTheButtons = FindObjectsOfType<UIMenuButton>();
            UIMenuButton button = allTheButtons.FirstOrDefault(btn => btn.name.StartsWith("DecoBuilder"));
            if (button == null)
            {
                _handleButtonCreateError();
                return;
            }
            UIWindow decoWindow = Instantiate(button.windowContentGO);
            DecoBuilderTab tab = decoWindow.gameObject.GetComponentInChildren<DecoBuilderTab>();
            //copy
            GameObject tbUIMenuButton = Instantiate(button.gameObject);
            tbUIMenuButton.transform.SetParent(button.transform.parent, false);
            tbBuilderButton = tbUIMenuButton.GetComponent<UIMenuButton>();

            //reposition
            RectTransform cloneTfm = tbUIMenuButton.GetComponent<RectTransform>();
            Vector2 anchorMin = cloneTfm.anchorMin;
            Vector2 anchorMax = cloneTfm.anchorMax;
            anchorMin.x = 0.3515f;
            anchorMax.x = 0.3615f;
            cloneTfm.anchorMin = anchorMin;
            cloneTfm.anchorMax = anchorMax;
            tbUIMenuButton.name = "TreeBrush";

            //tooltip
            UITooltip tooltip = tbUIMenuButton.GetComponent<UITooltip>();
            tooltip.text = "Tree Brush";

            // This steals the materials from the Deco Builder so we can use them ourselves on our own objects
            if (tab != null)
            {
                Builder builder = Instantiate(tab.builderGO);
                //ghostMat = builder.ghostMaterial;
                //ghostIntersectMat = builder.ghostIntersectMaterial;
                //ghostCantBuildMat = builder.ghostCantBuildMaterial;
                Destroy(builder.gameObject);
            }

            Destroy(decoWindow.gameObject);
        }

        private void MakeBrushWindow()
        {
            UIWindowSettings settings = MakeWindow("Tree Brush", "TreeBrush", false, new Vector2(500.0f, 300.0f));
            GameObject windowGO = settings.gameObject;

            // Actually add the window component, which makes it renderable
            tbWindow = windowGO.AddComponent<TreeBrushWindow>();

            // When our tab is clicked, it'll open up the new window
            tbBuilderButton.windowContentGO = tbWindow;
        }

        private static UIWindowSettings MakeWindow(string windowName, string windowTag, bool pinnable, Vector2 size)
        {
            // Create the tb window GameObject
            GameObject windowGO = Instantiate(rectTfmPrefab);
            windowGO.name = windowName;

            // Size the tb window
            RectTransform tbRect = windowGO.GetComponent<RectTransform>();
            tbRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, size.y / 2.0f, size.y);
            tbRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, size.x / 2.0f, size.x);

            // Add various settings to the window
            UIWindowSettings settings = windowGO.AddComponent<UIWindowSettings>();
            settings.pinnable = pinnable;
            settings.uniqueTagString = windowTag;
            settings.title = windowName;

            return settings;
        }


        private void _handleButtonCreateError()
        {
            this.enabled = false;
            Debug.LogError("Error! Could not find DecorationBuilder Button. If this message persists, changes in Parcitekt have been made. If there is no update available, feel free to open an issue at https://github.com/kairosswag/treebrush/");
        }
    }
}
