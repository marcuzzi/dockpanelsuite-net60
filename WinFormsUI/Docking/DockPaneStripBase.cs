using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Diagnostics.CodeAnalysis;

namespace WeifenLuo.WinFormsUI.Docking
{
    public abstract class DockPaneStripBase : Control
    {
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        protected internal class Tab : IDisposable
        {
            private IDockContent m_content;

            public Tab(IDockContent content)
            {
                m_content = content;
            }

            ~Tab()
            {
                Dispose(false);
            }

            public IDockContent Content
            {
                get { return m_content; }
            }

            public Form ContentForm
            {
                get { return m_content as Form; }
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
            }

            private Rectangle? _rect;

            public Rectangle? Rectangle
            {
                get
                {
                    if (_rect != null)
                    {
                        return _rect;
                    }

                    return _rect = System.Drawing.Rectangle.Empty;
                }

                set
                {
                    _rect = value;
                }
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        protected sealed class TabCollection : IEnumerable<Tab>
        {
            #region IEnumerable Members
            IEnumerator<Tab> IEnumerable<Tab>.GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                    yield return this[i];
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                    yield return this[i];
            }
            #endregion

            internal TabCollection(DockPane pane)
            {
                m_dockPane = pane;
            }

            private DockPane m_dockPane;
            public DockPane DockPane
            {
                get { return m_dockPane; }
            }

            public int Count
            {
                get { return DockPane.DisplayingContents.Count; }
            }

            public Tab this[int index]
            {
                get
                {
                    IDockContent content = DockPane.DisplayingContents[index];
                    if (content == null)
                        throw (new ArgumentOutOfRangeException(nameof(index)));
                    return content.DockHandler.GetTab(DockPane.TabStripControl);
                }
            }

            public bool Contains(Tab tab)
            {
                return (IndexOf(tab) != -1);
            }

            public bool Contains(IDockContent content)
            {
                return (IndexOf(content) != -1);
            }

            public int IndexOf(Tab tab)
            {
                if (tab == null)
                    return -1;

                return DockPane.DisplayingContents.IndexOf(tab.Content);
            }

            public int IndexOf(IDockContent content)
            {
                return DockPane.DisplayingContents.IndexOf(content);
            }
        }

        protected DockPaneStripBase(DockPane pane)
        {
            m_dockPane = pane;

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, false);
            AllowDrop = true;
        }

        private DockPane m_dockPane;
        protected DockPane DockPane
        {
            get { return m_dockPane; }
        }

        protected DockPane.AppearanceStyle Appearance
        {
            get { return DockPane.Appearance; }
        }

        private TabCollection m_tabs;

        protected TabCollection Tabs
        {
            get
            {
                return m_tabs ?? (m_tabs = new TabCollection(DockPane));
            }
        }

        internal void RefreshChanges()
        {
            if (IsDisposed)
                return;

            OnRefreshChanges();
        }

        protected virtual void OnRefreshChanges()
        {
        }

        protected internal abstract int MeasureHeight();

        protected internal abstract void EnsureTabVisible(IDockContent content);

        protected int HitTest()
        {
            return HitTest(PointToClient(Control.MousePosition));
        }

        protected internal abstract int HitTest(Point point);

        protected virtual bool MouseDownActivateTest(MouseEventArgs e)
        {
            return true;
        }

        public abstract GraphicsPath GetOutline(int index);

        protected internal virtual Tab CreateTab(IDockContent content)
        {
            return new Tab(content);
        }

        private Rectangle _dragBox = Rectangle.Empty;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            int index = HitTest();
            if (index != -1)
            {
                if (e.Button == MouseButtons.Middle)
                {
                    // Close the specified content.
                    TryCloseTab(index);
                }
                else
                {
                    IDockContent content = Tabs[index].Content;
                    if (DockPane.ActiveContent != content)
                    {
                        // Test if the content should be active
                        if (MouseDownActivateTest(e))
                            DockPane.ActiveContent = content;
                    }

                }
            }

            if (e.Button == MouseButtons.Left)
            {
                var dragSize = SystemInformation.DragSize;
                _dragBox = new Rectangle(new Point(e.X - (dragSize.Width / 2),
                                                e.Y - (dragSize.Height / 2)), dragSize);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Button != MouseButtons.Left || _dragBox.Contains(e.Location))
                return;

            if (DockPane.ActiveContent == null)
                return;

            if (DockPane.DockPanel.AllowEndUserDocking && DockPane.AllowDockDragAndDrop && DockPane.ActiveContent.DockHandler.AllowEndUserDocking)
                DockPane.DockPanel.BeginDrag(DockPane.ActiveContent.DockHandler);
        }

        protected bool HasTabPageContextMenu
        {
            get { return DockPane.HasTabPageContextMenu; }
        }

        protected void ShowTabPageContextMenu(Point position)
        {
            DockPane.ShowTabPageContextMenu(this, position);
        }

        protected bool TryCloseTab(int index)
        {
            if (index >= 0 || index < Tabs.Count)
            {
                // Close the specified content.
                IDockContent content = Tabs[index].Content;
                DockPane.CloseContent(content);
                if (PatchController.EnableSelectClosestOnClose == true)
                    SelectClosestPane(index);

                return true;
            }
            return false;
        }

        private void SelectClosestPane(int index)
        {
            if (index > 0 && DockPane.DockPanel.DocumentStyle == DocumentStyle.DockingWindow)
            {
                index = index - 1;

                if (index >= 0 || index < Tabs.Count)
                    DockPane.ActiveContent = Tabs[index].Content;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Right)
                ShowTabPageContextMenu(new Point(e.X, e.Y));
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)Win32.Msgs.WM_LBUTTONDBLCLK)
            {
                base.WndProc(ref m);

                int index = HitTest();
                if (DockPane.DockPanel.AllowEndUserDocking && index != -1)
                {
                    IDockContent content = Tabs[index].Content;
                    if (content.DockHandler.CheckDockState(!content.DockHandler.IsFloat) != DockState.Unknown)
                        content.DockHandler.IsFloat = !content.DockHandler.IsFloat;
                }

                return;
            }

            base.WndProc(ref m);
            return;
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);

            int index = HitTest();
            if (index != -1)
            {
                IDockContent content = Tabs[index].Content;
                if (DockPane.ActiveContent != content)
                    DockPane.ActiveContent = content;
            }
        }

        protected void ContentClosed()
        {
            if (m_tabs.Count == 0)
            {
                DockPane.ClearLastActiveContent();
            }
        }

        protected abstract Rectangle GetTabBounds(Tab tab);

        internal static Rectangle ToScreen(Rectangle rectangle, Control parent)
        {
            if (parent == null)
                return rectangle;

            return new Rectangle(parent.PointToScreen(new Point(rectangle.Left, rectangle.Top)), new Size(rectangle.Width, rectangle.Height));
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DockPaneStripAccessibleObject(this);
        }

        public class DockPaneStripAccessibleObject : Control.ControlAccessibleObject
        {
            private DockPaneStripBase _strip;

            public DockPaneStripAccessibleObject(DockPaneStripBase strip)
                : base(strip)
            {
                _strip = strip;
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.PageTabList;
                }
            }

            public override int GetChildCount()
            {
                return _strip.Tabs.Count;
            }

            public override AccessibleObject GetChild(int index)
            {
                return new DockPaneStripTabAccessibleObject(_strip, _strip.Tabs[index], this);
            }

            public override AccessibleObject HitTest(int x, int y)
            {
                Point point = new Point(x, y);
                foreach (Tab tab in _strip.Tabs)
                {
                    Rectangle rectangle = _strip.GetTabBounds(tab);
                    if (ToScreen(rectangle, _strip).Contains(point))
                        return new DockPaneStripTabAccessibleObject(_strip, tab, this);
                }

                return null;
            }
        }

        protected class DockPaneStripTabAccessibleObject : AccessibleObject
        {
            private DockPaneStripBase _strip;
            private Tab _tab;

            private AccessibleObject _parent;

            internal DockPaneStripTabAccessibleObject(DockPaneStripBase strip, Tab tab, AccessibleObject parent)
            {
                _strip = strip;
                _tab = tab;

                _parent = parent;
            }

            public override AccessibleObject Parent
            {
                get
                {
                    return _parent;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.PageTab;
                }
            }

            public override Rectangle Bounds
            {
                get
                {
                    Rectangle rectangle = _strip.GetTabBounds(_tab);
                    return ToScreen(rectangle, _strip);
                }
            }

            public override string Name
            {
                get
                {
                    return _tab.Content.DockHandler.TabText;
                }
                set
                {
                    //base.Name = value;
                }
            }
        }

    }
}
