using System;
using System.IO;
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
        
        public HashSet<Dependency> extract(ICollection<string> pathes){
            Console.WriteLine("Extracting trees of cs files");
            List<SyntaxTree> trees = extractTreesFromPathes(pathes);
            Console.WriteLine("Compiling trees");
            Compilation comp = compile(trees);
            this.dependencies = new HashSet<Dependency>();
            Console.WriteLine("Extracting dependencies");
            foreach(var tree in trees){
                var root = (CompilationUnitSyntax) tree.GetRoot();
                this.semanticModel = comp.GetSemanticModel(tree);
                Visit(root);
            }
            return this.dependencies;
        }

        private List<SyntaxTree> extractTreesFromPathes(ICollection<string> pathes){
            List<SyntaxTree> trees = new List<SyntaxTree>();
            foreach(var path in pathes){
                trees.Add(CSharpSyntaxTree.ParseText(File.ReadAllText(path)));
            }
            return trees;
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
        public void extractAnnotations(SyntaxList<AttributeListSyntax> attributes){
            foreach(var attributeList in attributes){
                foreach(var annotation in attributeList.Attributes){
                    addDependency(this.currentClass,Dependency.USE_ANNOTATION, annotation.Name.ToString());
                }
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
            this.extractAnnotations(node.AttributeLists);
            base.VisitClassDeclaration(node);
        }
    
        //declare dependencies
        public override void VisitMethodDeclaration(MethodDeclarationSyntax node){
            var returnType = this.semanticModel.GetTypeInfo(node.ReturnType).Type;
            if(returnType != null){
                addDependency(this.currentClass, Dependency.DECLARE, returnType.ToString());
            }
            this.extractAnnotations(node.AttributeLists);
            base.VisitMethodDeclaration(node);
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node){
            this.extractAnnotations(node.AttributeLists);
            base.VisitFieldDeclaration(node);
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
            this.extractAnnotations(node.AttributeLists);
            base.VisitParameter(node);
        }

        public override void VisitCatchDeclaration(CatchDeclarationSyntax node){
            var exceptionType = this.semanticModel.GetTypeInfo(node.Type).Type;
            if(exceptionType != null){
                addDependency(this.currentClass, Dependency.DECLARE, exceptionType.ToString());
            }
            base.VisitCatchDeclaration(node);
        }

        //create dependencies
        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node){
            var typeInfo = this.semanticModel.GetTypeInfo(node).Type;
            if(typeInfo != null){
                addDependency(this.currentClass, Dependency.CREATE, typeInfo.ToString());
            }
            base.VisitObjectCreationExpression(node);
        }

        //access dependencies                

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node){
            var callerType = this.semanticModel.GetTypeInfo(node.Expression).Type;
            if(callerType != null){
                addDependency(this.currentClass, Dependency.ACCESS, callerType.ToString());
            }
            base.VisitMemberAccessExpression(node);
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node){
            foreach(var argument in node.ArgumentList.Arguments){
                var argumentType = this.semanticModel.GetTypeInfo(argument.Expression).Type;
                if(argumentType != null){
                    addDependency(this.currentClass, Dependency.DECLARE, argumentType.ToString());
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