namespace SqlDocStore.Linq
{
    using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;
    using Remotion.Linq.Parsing.Structure;
    using Remotion.Linq.Parsing.Structure.NodeTypeProviders;

    public static class QueryParserFactory
    {
        public static QueryParser CreateQueryParser()
        {
            var expressionTreeParser = new ExpressionTreeParser(
                CreateNodeTypeProvider(),
                ExpressionTreeParser.CreateDefaultProcessor(ExpressionTransformerRegistry.CreateDefault()));

            return new QueryParser(expressionTreeParser);
        }

        private static CompoundNodeTypeProvider CreateNodeTypeProvider()
        {
            var registry = MethodInfoBasedNodeTypeRegistry.CreateFromRelinqAssembly();

            var nodeTypeProvider = ExpressionTreeParser.CreateDefaultNodeTypeProvider();
            nodeTypeProvider.InnerProviders.Add(registry);

            return nodeTypeProvider;
        }
    }
}
