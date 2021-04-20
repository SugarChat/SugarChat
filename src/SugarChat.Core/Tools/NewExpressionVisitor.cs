using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SugarChat.Core.Tools
{
    public class NewExpressionVisitor<T> : ExpressionVisitor
    {
        private readonly T _obj;
        public Dictionary<string, object> PropAndValues { get; set; }
        public NewExpressionVisitor(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentException(nameof(obj));
            }
            _obj = obj;
            PropAndValues = new Dictionary<string, object>();
        }

        protected override Expression VisitNew(NewExpression node)
        {
            if (node.Arguments?.Any() == true)
            {
                List<Expression> argExpressions = new List<Expression>();
                for (int i = 0; i < node.Arguments.Count; i++)
                {
                    var argNode = node.Arguments[i];
                    var argExpression = base.Visit(argNode);
                    if (argExpression is ConstantExpression constantExpression)
                    {
                        PropAndValues[node.Members[i].Name] = constantExpression.Value;
                    }
                    argExpressions.Add(argExpression);
                }
                return Expression.New(node.Constructor, argExpressions, members: node.Members);
            }
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var fuc = Expression.Lambda(node, (ParameterExpression)node.Expression).Compile();
            var obj = fuc.DynamicInvoke(_obj);
            return Expression.Constant(obj);
        }
    }
}
