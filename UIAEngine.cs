extern alias wrapper;

using wrapper::System.Windows.Automation;

using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System;

namespace ElegantRecorder
{
    public class UIAEngine: AutomationEngine
    {
        public UIAEngine(ElegantRecorder App) : base(App)
        {
        }

        public static int GetChildIndex(AutomationElement element)
        {
            TreeWalker walker = TreeWalker.ControlViewWalker;
            int index = 0;

            var node = walker.GetNextSibling(element);

            while (node != null)
            {
                node = walker.GetNextSibling(node);
                index++;
            }

            return index;
        }

        public static AutomationElement GetTopLevelWindow(AutomationElement element, out int level)
        {
            TreeWalker walker = TreeWalker.ControlViewWalker;
            AutomationElement elementParent;
            AutomationElement node = element;

            level = 0;

            if (node == AutomationElement.RootElement) return node;

            while (true)
            {
                elementParent = walker.GetParent(node);
                level++;

                if (elementParent == null)
                {
                    return null;
                }

                if (elementParent == AutomationElement.RootElement) break;
                node = elementParent;
            }

            return node;
        }

        public static AutomationElement FindWindowByName(string name, bool restrictToExe, string exePath)
        {
            TreeWalker walker = TreeWalker.ControlViewWalker;

            var node = AutomationElement.RootElement;

            var window = walker.GetFirstChild(node);

            while (window != null)
            {
                if (window.Current.Name == name)
                {
                    if (!restrictToExe)
                        return window;

                    int pid = (int)window.GetCurrentPropertyValue(AutomationElement.ProcessIdProperty);
                    var process = Process.GetProcessById(pid);

                    if (Path.GetFileNameWithoutExtension(exePath).ToLower() == process.ProcessName.ToLower())
                        return window;
                }

                window = walker.GetNextSibling(window);
            }

            return null;
        }

        public static AutomationElement FindElementInWindow(string controlName, string automationId, int? controlType, AutomationElement window, int? level, int? childIndex)
        {
            int elCount = 0;
            var foundElements = new List<AutomationElement>();

            void TreeFinder(AutomationElement currentNode, int currLevel)
            {
                if (currLevel == level)
                {
                    var currentName = currentNode.Current.Name;

                    //cached control names that are null may become blank strings after caching
                    if (currentName == null)
                        currentName = "";

                    if (currentName == controlName &&
                        currentNode.Current.AutomationId == automationId &&
                        currentNode.Current.ControlType.Id == controlType)
                    {
                        foundElements.Add(currentNode);
                    }

                    return;
                }

                TreeWalker walker = TreeWalker.ControlViewWalker;

                AutomationElement node = walker.GetFirstChild(currentNode);

                while (node != null)
                {
                    TreeFinder(node, currLevel + 1);
                    node = walker.GetNextSibling(node);
                }
            }

            TreeFinder(window, 1);

            elCount = foundElements.Count;

            if (elCount > 1)
            {
                /*
                foreach(var el in foundElements)
                {
                    int idx = GetChildIndex(el);

                    if (idx == childIndex)
                        return el;
                }
                */

                return null;
            }

            if (elCount == 0)
                return null;

            return foundElements[0];
        }

        public static AutomationElement FindBoundingWindow(AutomationElement node, Point point)
        {
            TreeWalker walker = TreeWalker.ControlViewWalker;

            var original = node;

            node = walker.GetFirstChild(node);

            while (node != null)
            {
                if (node.Current.ControlType == ControlType.Window || node == AutomationElement.RootElement)
                {
                    AutomationElement win = FindBoundingWindow(node, point);

                    if (win != null)
                    {
                        return win;
                    }
                }

                node = walker.GetNextSibling(node);
            }

            Rect boundingRect = original.Current.BoundingRectangle;

            if (point.X >= boundingRect.X &&
                point.X <= boundingRect.X + boundingRect.Width &&
                point.Y >= boundingRect.Y &&
                point.Y <= boundingRect.Y + boundingRect.Height)
            {
                return original;
            }

            return null;
        }

        public static AutomationElement FindElementFromPoint(Point point)
        {
            var node = AutomationElement.RootElement;

            AutomationElement boundingWin = FindBoundingWindow(node, point);

            if (boundingWin == null)
                return null;

            var result = boundingWin;

            TreeWalker walker = TreeWalker.RawViewWalker;
            node = walker.GetFirstChild(result);

            while (node != null)
            {
                Rect boundingRect = node.Current.BoundingRectangle;

                if (point.X >= boundingRect.X &&
                    point.X <= boundingRect.X + boundingRect.Width &&
                    point.Y >= boundingRect.Y &&
                    point.Y <= boundingRect.Y + boundingRect.Height)
                {
                    result = node;
                    var tmp = walker.GetFirstChild(node);
                    node = tmp;
                    continue;
                }

                node = walker.GetNextSibling(node);
            }

            return result;
        }

        public override bool FillClickAction(ref UIAction uiAction, ref string status, MouseHookStruct currentMouseHookStruct)
        {
            var point = new Point(currentMouseHookStruct.pt.x, currentMouseHookStruct.pt.y);

            try
            {
                CacheRequest cacheRequest = new CacheRequest();
                cacheRequest.Add(AutomationElement.NameProperty);
                cacheRequest.Add(AutomationElement.AutomationIdProperty);
                cacheRequest.Add(AutomationElement.ControlTypeProperty);

                AutomationElement element = null;

                using (cacheRequest.Activate())
                {
                    element = AutomationElement.FromPoint(point);
                }

                string elementName = element.Cached.Name;
                string automationId = element.Cached.AutomationId;
                int elementTypeId = element.Cached.ControlType.Id;

                int level = 0;
                AutomationElement topLevelWindow = GetTopLevelWindow(element, out level);

                if (topLevelWindow == null)
                    throw new Exception("TopLevelWindow not found");

                int pid = (int)topLevelWindow.GetCurrentPropertyValue(AutomationElement.ProcessIdProperty);

                var process = Process.GetProcessById(pid);
                string processName = process.ProcessName;

                status = elementName + " : " + automationId + " [" + processName + "]";

                if (App.ElegantOptions.RestrictToExe)
                {
                    if (Path.GetFileNameWithoutExtension(App.ElegantOptions.ExePath).ToLower() != processName.ToLower())
                        return false;
                }
                else
                {
                    //ignore actions on the recorder GUI
                    if (processName == Process.GetCurrentProcess().ProcessName)
                        return false;
                }

                Rect boundingRect = (Rect)topLevelWindow.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);

                uiAction.EventType = "click";
                uiAction.ControlName = elementName;
                uiAction.AutomationId = automationId;
                uiAction.ControlType = elementTypeId;
                uiAction.TopLevelWindow = topLevelWindow.Current.Name;
                uiAction.OffsetX = (int)(point.X - boundingRect.X);
                uiAction.OffsetY = (int)(point.Y - boundingRect.Y);
                uiAction.Level = level;
                uiAction.ExtraInfo = (int)currentMouseHookStruct.dwExtraInfo;
                uiAction.Flags = (int)currentMouseHookStruct.flags;

                // performance hit
                //uiAction.ChildIndex = GetChildIndex(element);
            }
            catch (Exception ex)
            {
                status = "Cannot get automation element: " + ex.ToString();
            }

            return true;
        }

        public override bool ReplayClickAction(UIAction action, ref string status)
        {
            AutomationElement topLevelWindow = FindWindowByName(action.TopLevelWindow, App.ElegantOptions.RestrictToExe, App.ElegantOptions.ExePath);

            if (topLevelWindow == null)
            {
                status = "Failed to find window: " + action.TopLevelWindow;
                return false;
            }

            Rect windowRect = (Rect)topLevelWindow.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);

            var x = (int)windowRect.X + action.OffsetX;
            var y = (int)windowRect.Y + action.OffsetY;

            AutomationElement targetElement = FindElementInWindow(action.ControlName, action.AutomationId, action.ControlType, topLevelWindow, action.Level, action.ChildIndex);

            if (targetElement == null)
            {
                status = "Failed to find element: " + action.ControlName;
            }
            else
            {
                Rect elementRect = (Rect)targetElement.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);

                if (elementRect.X > x || x > elementRect.X + elementRect.Width ||
                    elementRect.Y > y || y > elementRect.Y + elementRect.Height)
                {
                    var point = targetElement.GetClickablePoint();
                    x = (int)point.X;
                    y = (int)point.Y;
                }
            }

            App.WinAPI.MouseClick((int)x, (int)y, (uint)action.Flags, (uint)action.ExtraInfo);

            return true;
        }
    }
}
