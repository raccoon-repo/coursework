using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.BookService.Proxies
{
	public class ProxiedSet<E> : ISet<E>
	{
		private ISet<E> proxiedSet;

		public ProxiedSet(ISet<E> set) {
			this.proxiedSet = set;
		}

		public int Count => proxiedSet.Count;

		public bool IsReadOnly => proxiedSet.IsReadOnly;

		public bool Add(E item)
		{
			return proxiedSet.Add(item);
		}

		public void Clear()
		{
			proxiedSet.Clear();
		}

		public bool Contains(E item)
		{
			return proxiedSet.Contains(item);
		}

		public void CopyTo(E[] array, int arrayIndex)
		{
			proxiedSet.CopyTo(array, arrayIndex);
		}

		public void ExceptWith(IEnumerable<E> other)
		{
			proxiedSet.ExceptWith(other);
		}

		public IEnumerator<E> GetEnumerator()
		{
			return proxiedSet.GetEnumerator();
		}

		public void IntersectWith(IEnumerable<E> other)
		{
			proxiedSet.IntersectWith(other);
		}

		public bool IsProperSubsetOf(IEnumerable<E> other)
		{
			return proxiedSet.IsProperSubsetOf(other);
		}

		public bool IsProperSupersetOf(IEnumerable<E> other)
		{
			return proxiedSet.IsProperSupersetOf(other);
		}

		public bool IsSubsetOf(IEnumerable<E> other)
		{
			return proxiedSet.IsSubsetOf(other);
		}

		public bool IsSupersetOf(IEnumerable<E> other)
		{
			return proxiedSet.IsSupersetOf(other);
		}

		public bool Overlaps(IEnumerable<E> other)
		{
			return proxiedSet.Overlaps(other);
		}

		public bool Remove(E item)
		{
			return proxiedSet.Remove(item);
		}

		public bool SetEquals(IEnumerable<E> other)
		{
			return proxiedSet.SetEquals(other);
		}

		public void SymmetricExceptWith(IEnumerable<E> other)
		{
			proxiedSet.SymmetricExceptWith(other);
		}

		public void UnionWith(IEnumerable<E> other)
		{
			proxiedSet.UnionWith(other);
		}

		void ICollection<E>.Add(E item)
		{
			proxiedSet.Add(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return proxiedSet.GetEnumerator();
		}
	}
}
