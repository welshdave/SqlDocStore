﻿namespace SqlDocStore.Linq
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Remotion.Linq;
    using Remotion.Linq.Parsing.Structure;

    public class SqlDocStoreQueryProvider : QueryProviderBase
    {
        private readonly Type _queryableType;
        public SqlDocStoreQueryProvider(Type queryableType, IQueryParser queryParser, IQueryExecutor executor) : base(queryParser, executor)
        {
            _queryableType = queryableType;
        }

        public override IQueryable<T> CreateQuery<T>(Expression expression)
        {
            return (IQueryable<T>) Activator.CreateInstance(_queryableType.MakeGenericType(typeof(T)), this,
                expression);
        }
    }
}
