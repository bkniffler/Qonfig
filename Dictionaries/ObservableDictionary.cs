using System;
using System.Collections.Generic;

namespace Qonfig.Dictionaries
{
    /// <summary>
    /// Event handler for added items event
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="item">Added name and value pair</param>
    public delegate void ItemAddedEventHandler(object sender, KeyValuePair<string, object> item);

    /// <summary>
    /// Dictionary that implements ItemAddedEventHandler, for notification about new items
    /// </summary>
    public class ObservableDictionary : Dictionary<string, object>
    {
        public event ItemAddedEventHandler ItemAdded;

        private readonly List<string> _nonPersistent = new List<string>();

        /// <summary>
        /// Is Item nonpersistent, so not to be stored?
        /// </summary>
        /// <param name="key">Name of item</param>
        public bool IsNonPersistent(string key)
        {
            if (_nonPersistent.Contains(key)) return true;
            return false;
        }

        /// <summary>
        /// Add item
        /// </summary>
        /// <param name="key">Name of item</param>
        /// <param name="value">Value of item</param>
        public new void Add(string key, object value)
        {
            var idx = 0;
            if (!TryGetKeyIndex(this, key, ref idx))
            {
                base.Add(key, value);
                OnItemAdded(key, value);
            }
        }

        /// <summary>
        /// Add nonpersistent item
        /// </summary>
        /// <param name="key">Name of item</param>
        /// <param name="value">Value of item</param>
        public void AddNonPersistent(string key, object value)
        {
            var idx = 0;
            if (!TryGetKeyIndex(this, key, ref idx))
            {
                base.Add(key, value);
                _nonPersistent.Add(key);
            }
        }

        /// <summary>
        /// Item without value added, go default
        /// </summary>
        /// <param name="key">Name of item</param>
        public new void Add(string key)
        {
            var idx = 0;
            if (!TryGetKeyIndex(this, key, ref idx))
            {
                base.Add(key, null);
                OnItemAdded(key, String.Empty);
            }
        }

        /// <summary>
        /// Helper
        /// </summary>
        /// <param name="observableDictionary"></param>
        /// <param name="key"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        protected bool TryGetKeyIndex(ObservableDictionary observableDictionary, string key, ref int idx)
        {
            foreach (var pair in observableDictionary)
            {
                if (pair.Key.Equals(key))
                {
                    return true;
                }
                idx++;
            }
            return false;
        }

        /// <summary>
        /// Event invocator
        /// </summary>
        /// <param name="name">Name of item</param>
        /// <param name="value">Value of item</param>
        protected virtual void OnItemAdded(string name, object value)
        {
            if (ItemAdded != null)
            {
                ItemAdded(this, new KeyValuePair<string, object>(name, value));
            }
        }

    }
}