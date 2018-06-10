namespace SqlDocStore.MsSql
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net.Http.Headers;
    using System.Text;
    using Linq;
    using SqlDocStore.Linq;
    using Remotion.Linq;
    using Remotion.Linq.Clauses;
    using Remotion.Linq.Parsing;

    public class MsSqlQueryCompiler : QueryModelVisitorBase, IQueryCompiler
    {
        public MsSqlQueryCompiler(IDocumentStore store)
        {
            _store = store;
            _query = new MsSqlQueryParts();
            
        }

        private IDocumentStore _store;
        
        private MsSqlQueryParts _query;

        private Dictionary<string,object> _parameters = new Dictionary<string, object>();

        public SqlQuery Compile(QueryModel queryModel)
        {
            base.VisitQueryModel(queryModel);

            var sqlQuery = new SqlQuery();
            sqlQuery.Sql = _query;
            foreach (var parameter in _parameters)
            {
                sqlQuery.Parameters.Add(parameter.Key,parameter.Value);
            }

            return sqlQuery;
        }

        public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
        {
            var selectors = new List<string>();
            if (selectClause.Selector.NodeType == ExpressionType.Extension)
            {
                //do nothing for now.
            }
        }

        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
        {
            _query.From = $"{_store.Settings.Schema}.{_store.Settings.Table} ";
        }

        protected override void VisitBodyClauses(ObservableCollection<IBodyClause> bodyClauses, QueryModel queryModel)
        {
            var wheres = bodyClauses.OfType<WhereClause>().ToList();
            foreach (var where in wheres)
            {
                VisitWhereClause(where, queryModel, wheres.IndexOf(where)); //Why do I need to do this? Shouldn't VisitWhereClause get called anyway?
            }

        }

        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
        {
            var where = new WhereClauseVisitor(queryModel.MainFromClause.ItemType);
            where.Visit(whereClause.Predicate);
            _query.Where = where.WhereClause;
            _parameters = where.Parameters;
            base.VisitWhereClause(whereClause, queryModel, index);
        }

    }
}
