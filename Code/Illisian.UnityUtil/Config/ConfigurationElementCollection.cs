using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Illisian.UnityUtil.Config
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="K"></typeparam>
	/// <typeparam name="V"></typeparam>
    public abstract class ConfigurationElementCollection<K, V> : ConfigurationElementCollection where V : ConfigurationElement, new()
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="ConfigurationElementCollection&lt;K, V&gt;"/> class.
		/// </summary>
        public ConfigurationElementCollection()
        {
        }

		/// <summary>
		/// Gets the type of the <see cref="T:System.Configuration.ConfigurationElementCollection"/>.
		/// </summary>
		/// <returns>
		/// The <see cref="T:System.Configuration.ConfigurationElementCollectionType"/> of this collection.
		///   </returns>
        public abstract override ConfigurationElementCollectionType CollectionType
        {
            get;
        }

		/// <summary>
		/// Gets the name used to identify this collection of elements in the configuration file when overridden in a derived class.
		/// </summary>
		/// <returns>
		/// The name of the collection; otherwise, an empty string. The default is an empty string.
		///   </returns>
        protected abstract override string ElementName
        {
            get;
        }

		/// <summary>
		/// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
		/// </summary>
		/// <returns>
		/// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
		/// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new V();
        }

		/// <summary>
		/// Gets the element key for a specified configuration element when overridden in a derived class.
		/// </summary>
		/// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for.</param>
		/// <returns>
		/// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
		/// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return GetElementKey((V)element);
        }

		/// <summary>
		/// Gets the element key.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns></returns>
        protected abstract K GetElementKey(V element);

		/// <summary>
		/// Adds the specified path.
		/// </summary>
		/// <param name="path">The path.</param>
        public void Add(V path)
        {
            BaseAdd(path);
        }

		/// <summary>
		/// Removes the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
        public void Remove(K key)
        {
            BaseRemove(key);
        }

		/// <summary>
		/// Gets or sets a property, attribute, or child element of this configuration element.
		/// </summary>
		/// <returns>
		/// The specified property, attribute, or child element
		///   </returns>
		///   
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">
		///   <paramref name="key"/> is read-only or locked.
		///   </exception>
        public V this[K key]
        {
            get { return (V)BaseGet(key); }
        }

		/// <summary>
		/// Gets or sets a property, attribute, or child element of this configuration element.
		/// </summary>
		/// <returns>
		/// The specified property, attribute, or child element
		///   </returns>
		///   
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">
		///   <paramref name="index"/> is read-only or locked.
		///   </exception>
        public V this[int index]
        {
            get { return (V)BaseGet(index); }
        }
    }
}
