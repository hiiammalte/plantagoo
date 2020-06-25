using System;
using System.Collections.Generic;

namespace Plantagoo.Filtering
{
	public class PagingReturnModel<T>
	{
		public int CurrentPage { get; private set; }
		public int TotalPages { get; private set; }
		public int PageSize { get; private set; }
		public int TotalCount { get; private set; }
		public List<T> Items { get; set; }

		public PagingReturnModel(List<T> items, int count, int pageNumber, int pageSize)
		{
			TotalCount = count;
			PageSize = pageSize;
			CurrentPage = pageNumber;
			TotalPages = (int)Math.Ceiling(count / (double)pageSize);
			Items = items;
		}
	}
}
