using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using Parkitect.UI;

namespace TreeBrush
{
    class TreeBrush : MonoBehaviour
    {
        private bool ShallRun
        {
            get
            {
                return _shallRun && placingActivated;
            }
            set
            {
                _shallRun = value;
            }
        }
        private bool _shallRun = false;
        private volatile bool placingActivated = false;
        private List<SerializedMonoBehaviour> list = null;
        private static string buttonLabel = "Button untoggled";
        private static string debug = "no debug";
        private static float placementDelay = 1;
        private static int placementAmount = 20;
        private System.Random random = new System.Random();

        void Update()
        {
            if (!placingActivated) return;
            if (Input.GetMouseButton(0))
            {
                if (ShallRun == false)
                {
                    ShallRun = true;
                    StartCoroutine(SpawnForest());
                }
                ShallRun = true;
            }
            else
            {
                ShallRun = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                debug = "rectTransforms: \n";
                var windows = UIWindowsController.Instance.getWindows();
                foreach(var elem in windows)
                {
                    debug += elem.getName() + "\n" + elem + "\n";
                    elem.setSubtitle("dumdidum");
                }
               
            }
        }

        private IEnumerator SpawnForest()
        {
            debug += "spawning: placing|shallrun " + placingActivated + "|" + ShallRun + "\n";
            while (placingActivated && ShallRun)
            {
                debug = "";
                if (placingActivated && ShallRun)
                {
                    var directionGenerator = new CardinalDirection();
                    var park = GameController.Instance.park;
                    int tryNumber = 0;
                    Vector3 lastPos;
                    CursorHoverPosition(out lastPos);
                    for (int remaining = 0; remaining < placementAmount && ShallRun; ++remaining)
                    {
                        Vector3 cursorPos;
                        bool canPlace = CursorHoverPosition(out cursorPos);
                        bool added = false;
                        if (tryNumber == 0)
                        {
                            added = placeFir(cursorPos);
                            ++tryNumber;
                        }
                        int maxcount = 30; //ToDo: Put some thought into this number
                        while (!added && maxcount-- > 0)
                        {
                            var nextDir = directionGenerator.next;
                            Vector2 shift = CardinalDirection.getDirectionVector(nextDir).normalized;
                            shift = rotateVector(shift);
                            float scale = (tryNumber / 8) + 1;
                            if ((lastPos - cursorPos).magnitude > scale)
                                scale = 1;
                            shift = new Vector2(shift.x * scale, shift.y * scale);
                            var newVec = park.getTerrainPointAt(new Vector3(cursorPos.x + shift.x, cursorPos.y, cursorPos.z + shift.y));
                            added = placeFir(newVec);
                            ++tryNumber;
                            yield return new UnityEngine.WaitForSeconds(placementDelay / placementAmount);
                        }
                    }
                }
                yield return new UnityEngine.WaitForSeconds(0.1f);
            }
        }

        private Vector2 rotateVector(Vector2 org)
        {
            double degrees = random.NextDouble() * (Math.PI / 4);
            Vector2 vec = new Vector2();
            vec.x = Convert.ToSingle(org.x * Math.Cos(degrees) - org.y * Math.Sin(degrees));
            vec.y = Convert.ToSingle(org.x * Math.Sin(degrees) + org.y * Math.Cos(degrees));
            return vec;
        }

        private bool canPlaceAtCursor(out Vector3 cursorPos)
        {
            RaycastHit hit = new RaycastHit();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            cursorPos = new Vector3();
            if (Physics.Raycast(ray, out hit))
            {
                cursorPos = hit.point;
                return canPlaceAt(hit.point);
            }
            return false;
        }

        private bool canPlaceAt(Vector3 position)
        {
            var terrain = GameController.Instance.park.getTerrain(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z));
            return terrain.getHeighestHeight() + 0.1f > Mathf.Ceil(position.y) && !terrain.hasWater();
        }



        void OnGUI()
        {

            if (GUI.Button(new Rect(10, 10, 300, 20), buttonLabel))
            {
                placingActivated = !placingActivated;
                buttonLabel = "Button" + ((placingActivated) ? " toggled" : " untoggled");
                list = GameController.Instance.getSerializedObjects();
                //GameController.Instance.park.GetComponents
            }
            if (placingActivated)
            {
                string compString = "Info@: \n";

                compString += debug;
                GUI.Label(new Rect(10, 30, 300, 1500), compString);
            }
        }

        private bool CursorHoverPosition(out Vector3 position)
        {
            RaycastHit hit = new RaycastHit();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var tmp = Physics.Raycast(ray, out hit);
            position = hit.point;
            return tmp;
        }

        private bool placeFir(Vector3 position)
        {
            if (!canPlaceAt(position))
                return false;

            TreeEntity fir = null;
            foreach (var o in ScriptableSingleton<AssetManager>.Instance.getDecoObjects())
                if (o.getName().StartsWith("Pop") && o is TreeEntity) fir = o as TreeEntity;

            if (fir != null)
            {
                var tree = Instantiate(fir);
                tree.transform.position = position;
                tree.transform.forward = Vector3.forward;
                if (!tree.getCollidingGameObjects().Any())
                {
                    tree.Initialize();
                    tree.startAnimateSpawn(Vector3.zero);
                    return true;
                }
                else
                {
                    tree.isPreview = true;
                    tree.Kill();
                    return false;
                }
            }
            return false;
        }

    }

}
