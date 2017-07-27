using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dependencies{
    class DepExtractor : CSharpSyntaxWalker{

        private CSharpCompilation compilation;
        private SyntaxTree[] trees;
        private SemanticModel semanticModel;

        private string currentClass;

        public DepExtractor(SyntaxTree[] trees){
            this.trees = trees;
            compile(this.trees);
        }

        private void compile(SyntaxTree[] trees){
            this.compilation = CSharpCompilation.Create("HelloWorld")
                .AddReferences(MetadataReference.CreateFromFile(
                    typeof(object).GetTypeInfo().Assembly.Location))
                .AddSyntaxTrees(trees);
        }
        public void start(){
            foreach(var tree in this.trees){
                var root = (CompilationUnitSyntax)tree.GetRoot();
                this.semanticModel = this.compilation.GetSemanticModel(tree);
                Visit(root);
            }
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node){
            this.currentClass = this.semanticModel.GetDeclaredSymbol(node).ToString();
            base.VisitClassDeclaration(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node){
            foreach(var p  in node.ParameterList.Parameters){
                string parameterType = this.semanticModel.GetDeclaredSymbol(p).ToString();
                Console.WriteLine(this.currentClass+",declare,"+parameterType);
            }
            string returnType = node.ReturnType.ToString();
            if(returnType != "void"){
                Console.WriteLine(this.currentClass+",declare,"+returnType);
            }
            base.VisitMethodDeclaration(node);
        }

        public override void VisitVariableDeclaration(VariableDeclarationSyntax node){
            var variableType = this.semanticModel.GetSymbolInfo(node.Type).Symbol.ToString();
            Console.WriteLine(this.currentClass+",declare,"+variableType);
            base.VisitVariableDeclaration(node);
        }

        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node){
            string creationType = this.semanticModel.GetSymbolInfo(node.Type).Symbol.ToString();
            Console.WriteLine(this.currentClass+",create,"+creationType);
        }
                
        public override void VisitInvocationExpression(InvocationExpressionSyntax node){
            var memberAccess = node.Expression as MemberAccessExpressionSyntax;
            if(memberAccess != null){
                Console.WriteLine(this.currentClass+",access,"+this.semanticModel.GetTypeInfo(memberAccess.Expression).Type);
            }
            base.VisitInvocationExpression(node);
        }
        
    }
}