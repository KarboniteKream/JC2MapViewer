/*
 JC2MapViewer
 Copyright 2010 - DerPlaya78
 
 this program is free software: you can redistribute it and/or modify
 it under the terms of the GNU Lesser General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU Lesser General Public License
 along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System.Collections.Generic;
using System.ComponentModel;
using JC2.Save;

namespace JC2MapViewer
{
    public class ChooserViewModel : INotifyPropertyChanged
    {
        #region Data

        bool? _isChecked = false;
        ChooserViewModel _parent;
        string _category;
        #endregion // Data

        public List<string> GetSelectedCategories()
        { 
            List<string> tmp = new List<string>();

            if (Children != null && Children.Count != 0)
            {
                foreach (var o in Children)
                {
                    tmp.AddRange(o.GetSelectedCategories());
                }
            }
            else {
                if (_category != null && _isChecked != null && _isChecked.Value) tmp.Add(_category);
            }
            return tmp;
        }

        private ChooserViewModel getChild(string name)
        {
            if (Children == null || Children.Count == 0) return null;
            foreach (var c in Children) if (c.Name == name) return c;
            return null;
        }

        public void UpdateCount(string category, int have, int shouldhave)
        {
            if (Children == null || Children.Count == 0) return;
            string[] path = category.Split('\\');
            ChooserViewModel m = getByPath(path);
            if (m != null)
            {
                if (have != shouldhave) m.IsInComplete = true;
                else m.IsInComplete = false;
                m.Text = string.Format("{0} ({1}/{2})", path[path.Length - 1], have, shouldhave);
            }
        }

        private ChooserViewModel getByPath(string[] path)
        {
            ChooserViewModel current = this;
            foreach (string s in path)
            {
                current = current.getChild(s);
                if (current == null) return null;
            }
            return current;
        }

        public static List<ChooserViewModel> CreateChooser()
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                return new List<ChooserViewModel>();
            }

            ChooserViewModel root = new ChooserViewModel("All Items", "All Items");
            foreach (string categoryPath in SavedObjectInfoLookup.Categories)
            {
                string[] path = categoryPath.Split('\\');
                ChooserViewModel newroot = root;
                ChooserViewModel oldroot = root;

                foreach (string s in path)
                {
                    newroot = newroot.getChild(s);
                    if (newroot == null)
                    {
                        newroot = new ChooserViewModel(s, s);
                        oldroot.Children.Add(newroot);
                    }

                    oldroot = newroot;
                }
                oldroot._category = categoryPath;
            }

            root.Initialize();
            return new List<ChooserViewModel> { root };
        }

        ChooserViewModel(string name, string text)
        {
            this.Name = name;
            this.Text = text;
            this.Children = new List<ChooserViewModel>();
        }

        void Initialize()
        {
            foreach (ChooserViewModel child in this.Children)
            {
                child._parent = this;
                child.Initialize();
            }
        }

        #region Properties

        public List<ChooserViewModel> Children { get; private set; }

        public bool IsInitiallySelected { get; private set; }

        public string Name { get; private set; }

        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; OnPropertyChanged("Text"); }
        }

        private bool _isInComplete;
        public bool IsInComplete
        {
            get { return _isInComplete; }
            set { _isInComplete = value; OnPropertyChanged("IsInComplete"); }
        }

        #region IsChecked

        /// <summary>
        /// Gets/sets the state of the associated UI toggle (ex. CheckBox).
        /// The return value is calculated based on the check state of all
        /// child FooViewModels.  Setting this property to true or false
        /// will set all children to the same check state, and setting it 
        /// to any value will cause the parent to verify its check state.
        /// </summary>
        public bool? IsChecked
        {
            get { return _isChecked; }
            set { this.SetIsChecked(value, true, true); }
        }

        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isChecked)
                return;

            _isChecked = value;

            if (updateChildren && _isChecked.HasValue)
                this.Children.ForEach(c => c.SetIsChecked(_isChecked, true, false));

            if (updateParent && _parent != null)
                _parent.VerifyCheckState();
            
            this.OnPropertyChanged("IsChecked");
        }

        void VerifyCheckState()
        {
            bool? state = null;
            for (int i = 0; i < this.Children.Count; ++i)
            {
                bool? current = this.Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }
            this.SetIsChecked(state, false, true);
        }

        #endregion // IsChecked

        #endregion // Properties

        #region INotifyPropertyChanged Members

        void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}