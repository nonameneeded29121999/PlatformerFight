using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Core.EventSystem
{
    public class EventGroupNode : BaseEventNode
    {
        public List<BaseEventNode> m_Children;

        public EventGroupNode()
        {
            nodeTitle = "Event Group";

            textColor = Color.red;
        }

        public override void Draw()
        {
            base.Draw();
            if (m_Children == null || m_Children.Count == 0)
                return;

            for (int i = 0; i < m_Children.Count; i++)
            {
                DrawCurve(m_Children[i].nodeRect, m_Children[i] is SingleEventNode ? Color.cyan : Color.red);
                m_Children[i]?.Draw();
            }
        }

        private void DrawCurve(Rect nextRect, Color curveColor)
        {
            Vector3 startPos = new Vector3(nodeRect.x + nodeRect.width, nodeRect.y + nodeRect.height * 0.5f);
            Vector3 endPos = new Vector3(nextRect.x, nextRect.y + nextRect.height * 0.5f);
            Vector3 startTan = startPos + Vector3.right * 50f;
            Vector3 endTan = endPos + Vector3.left * 50f;
            Handles.DrawBezier(startPos, endPos, startTan, endTan, curveColor, null, 5);
        }

        public override bool ClickOn(Vector2 pos, out BaseEventNode node)
        {
            if (nodeRect.Contains(pos))
            {
                node = this;
                return true;
            }

            if (m_Children != null && m_Children.Count > 0)
            {
                for (int i = 0; i < m_Children.Count; i++)
                {
                    if (m_Children[i].ClickOn(pos, out node))
                    {
                        return true;
                    }
                }
            }
            node = null;
            return false;
        }

        public void AddNode(BaseEventNode node)
        {
            if (m_Children == null)
                m_Children = new List<BaseEventNode>();
            m_Children.Add(node);
        }

        public void DeleteNode(BaseEventNode node)
        {
            if (node == null || m_Children == null)
                return;
            m_Children.Remove(node);
        }

        public override bool IsNameRepeated(string name, int id)
        {
            if (eventName == name && id != this.id)
            {
                return true;
            }

            if (m_Children == null)
                return false;

            for (int i = 0; i < m_Children.Count; i++)
            {
                if (m_Children[i].IsNameRepeated(name, id))
                    return true;
            }

            return false;
        }

        public override void MoveNode(Vector2 delta)
        {
            base.MoveNode(delta);
            if (m_Children == null) return;

            for (int i = 0; i < m_Children.Count; i++)
            {
                m_Children[i].MoveNode(delta);
            }
        }

        public override void CopyNode(List<SerializableNode> containers)
        {
            if (!legal)
                return;

            SerializableNode snode = new SerializableNode();
            snode.type = "Group";
            snode.name = eventName;
            snode.id = id;
            snode.rect = nodeRect;
            if (m_Children != null)
            {
                snode.children = new List<int>();
                for (int i = 0; i < m_Children.Count; i++)
                {
                    snode.children.Add(m_Children[i].id);
                    m_Children[i].CopyNode(containers);
                }
            }
            containers.Add(snode);
        }

        public override void LoadNode(List<SerializableNode> containers, int id)
        {
            base.LoadNode(containers, id);
            var data = containers.Find(n => n.id == id);
            if (data != null && data.children != null)
            {
                for (int i = 0; i < data.children.Count; i++)
                {
                    var child = containers.Find(n => n.id == data.children[i]);
                    if (child != null)
                    {
                        BaseEventNode node = null;
                        if (child.type == "Single")
                        {
                            node = new SingleEventNode();
                        }
                        else if (child.type == "Group")
                        {
                            node = new EventGroupNode();
                        }                       

                        if (node != null)
                        {
                            node.parent = this;
                            node.LoadNode(containers, data.children[i]);
                            AddNode(node);
                        }
                    }
                }
            }
        }
    }
}
