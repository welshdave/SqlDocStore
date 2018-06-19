namespace SqlDocStore.MsSql
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using Linq;
    using SqlDocStore.Linq;
    using Remotion.Linq;
    using Remotion.Linq.Clauses;

    public class MsSqlQueryCompiler : QueryModelVisitorBase, IQueryCompiler
    {
        private readonly IDocumentStore _store;

        private readonly MsSqlQueryParts _query;

        private Dictionary<string, object> _parameters = new Dictionary<string, object>();

        public MsSqlQueryCompiler(IDocumentStore store)
        {
            _store = store;
            _query = new MsSqlQueryParts();
            
        }

        public SqlQuery Compile(QueryModel queryModel)
        {
            base.VisitQueryModel(queryModel);

            var sqlQuery = new SqlQuery {Sql = _query};
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
                var whereVisitor = new WhereClauseVisitor(queryModel.MainFromClause.ItemType);
                whereVisitor.Visit(where.Predicate);
                _query.Where = whereVisitor.WhereClause;
                _parameters = whereVisitor.Parameters;
            }

            var orderBy = bodyClauses.OfType<OrderByClause>().FirstOrDefault();
            if(orderBy != null)
                foreach (var ordering in orderBy.Orderings)
                {
                    var expression = ordering.Expression;
                    if (expression is MemberExpression exp)
                    {
                        _query.OrderBy.Add(new MsSqlOrderBy { Name = exp.Member.Name, Type = exp.Type, Direction = ordering.OrderingDirection});
                    }
                }
        }
    }
}
