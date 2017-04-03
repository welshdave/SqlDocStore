namespace SqlDocStore.MsSql.Linq
{
    using System;
    using System.Linq.Expressions;
    using Remotion.Linq;
    using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;
    using Remotion.Linq.Parsing.Structure;
    using Remotion.Linq.Parsing.Structure.NodeTypeProviders;

    public class MsSqlQueryParser : IQueryParser
    {
        private readonly QueryParser _parser;

        public MsSqlQueryParser(Action<MethodInfoBasedNodeTypeRegistry> registerNodeTypes = null)
        {
            var transformerRegistry = ExpressionTransformerRegistry.CreateDefault();
            
            var processor = ExpressionTreeParser.CreateDefaultProcessor(transformerRegistry);

            var nodeTypeRegistry = MethodInfoBasedNodeTypeRegistry.CreateFromRelinqAssembly();
            registerNodeTypes?.Invoke(nodeTypeRegistry);

            var expressionTreeParser =
                new ExpressionTreeParser(nodeTypeRegistry, processor);
            _parser = new QueryParser(expressionTreeParser);
        }
        public QueryModel GetParsedQuery(Expression expressionTreeRoot)
        {
            return _parser.GetParsedQuery(expressionTreeRoot);
        }
    }
}