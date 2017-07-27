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

        private SemanticModel semanticModel;
        private HashSet<Dependency> dependencies;
        private string currentClass;

        private static DepExtractor instance = null;

        public static DepExtractor getInstance(){
            if(DepExtractor.instance == null){
                DepExtractor.instance = new DepExtractor();
            }
            return DepExtractor.instance;
        }
        
        public HashSet<Dependency> extract(ICollection<SyntaxTree> trees){
            Compilation comp = compile(trees);
            this.dependencies = new HashSet<Dependency>();
            foreach(var tree in trees){
                var root = (CompilationUnitSyntax) tree.GetRoot();
                this.semanticModel = comp.GetSemanticModel(tree);
                Visit(root);
            }
            return this.dependencies;
        }

        private Compilation compile(ICollection<SyntaxTree> trees){
            return CSharpCompilation.Create("Project")
                .AddReferences(MetadataReference.CreateFromFile(
                    typeof(object).GetTypeInfo().Assembly.Location))
                .AddSyntaxTrees(trees);
        }

        private void addDependency(string origin, string type, string destin){
            if(destin != "void" && destin != "?"){
                //Console.WriteLine(origin+","+type+","+destin);
                this.dependencies.Add(new Dependency(origin, type, destin));
            }
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node){
            this.currentClass = this.semanticModel.GetDeclaredSymbol(node).ToString();
            if(node.BaseList != null){
                var baseType = this.semanticModel.GetDeclaredSymbol(node).BaseType.ToString();
                foreach(var inheritance in node.BaseList.Types){
                    var inheritanceType = this.semanticModel.GetTypeInfo(inheritance.Type).Type.ToString();
                    if(inheritanceType != baseType){
                        addDependency(this.currentClass, Dependency.IMPLEMENT, inheritanceType);
                    }else{
                        addDependency(this.currentClass, Dependency.EXTEND, inheritanceType);
                    }
                }
            }
            base.VisitClassDeclaration(node);
        }


        //declare dependencies
        public override void VisitMethodDeclaration(MethodDeclarationSyntax node){
            var returnType = this.semanticModel.GetTypeInfo(node.ReturnType).Type;
            if(returnType != null){
                addDependency(this.currentClass, Dependency.DECLARE, returnType.ToString());
            }
            base.VisitMethodDeclaration(node);
        }

        public override void VisitVariableDeclaration(VariableDeclarationSyntax node){
            var variableType = this.semanticModel.GetTypeInfo(node.Type).Type;
            if(variableType != null){
                addDependency(this.currentClass, Dependency.DECLARE, variableType.ToString());
            }
            base.VisitVariableDeclaration(node);
        }

        public override void VisitParameter(ParameterSyntax node){
            var parameterType = this.semanticModel.GetDeclaredSymbol(node);
            if(parameterType != null){
                addDependency(this.currentClass, Dependency.DECLARE, parameterType.ToString());
            }
            base.VisitParameter(node);
        }

        public override void VisitCatchDeclaration(CatchDeclarationSyntax node){
            var exceptionType = this.semanticModel.GetTypeInfo(node.Type).Type;
            if(exceptionType != null){
                addDependency(this.currentClass, Dependency.DECLARE, exceptionType.ToString());
            }
        }

        //create dependencies
        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node){
            var symbolInfo = this.semanticModel.GetSymbolInfo(node.Type).Symbol;
            if(symbolInfo != null){
                addDependency(this.currentClass, Dependency.CREATE, symbolInfo.ToString());
            }
        }
                
        //access dependencies                
        public override void VisitInvocationExpression(InvocationExpressionSyntax node){
            var memberAccess = node.Expression as MemberAccessExpressionSyntax;
            if(memberAccess != null){
                var callerType = this.semanticModel.GetTypeInfo(memberAccess.Expression).Type;
                if(callerType != null){
                    addDependency(this.currentClass, Dependency.ACCESS, callerType.ToString());
                }
            }
            base.VisitInvocationExpression(node);
        }

        //throw dependencies
        public override void VisitThrowStatement(ThrowStatementSyntax node){
            var throwType = this.semanticModel.GetTypeInfo(node.Expression).Type;
            if(throwType != null){
                addDependency(this.currentClass, Dependency.THROW, throwType.ToString());
            }
            base.VisitThrowStatement(node);
        }
        
    }
}