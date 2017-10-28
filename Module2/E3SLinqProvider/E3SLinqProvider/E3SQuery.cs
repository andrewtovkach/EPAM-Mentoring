﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace E3SLinqProvider
{
	class E3SQuery<T> : IQueryable<T>
	{
		private Expression expression;
		private global::E3SLinqProvider.E3SLinqProvider provider;

		public E3SQuery(Expression expression, global::E3SLinqProvider.E3SLinqProvider provider)
		{
			this.expression = expression;
			this.provider = provider;
		}

		public Type ElementType
		{
			get
			{
				return typeof(T);
			}
		}

		public Expression Expression
		{
			get
			{
				return expression;
			}
		}

		public IQueryProvider Provider
		{
			get
			{
				return provider;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return provider.Execute<IEnumerable<T>>(expression).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return provider.Execute<IEnumerable>(expression).GetEnumerator();
		}
	}
}