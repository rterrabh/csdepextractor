using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Dependencies;
class DepCSharp
{
    static void Main(string[] args)
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(
@"using System;
 
namespace HelloWorld.Hello
{
    class ClassA{
        public void test(){
           ClassB x =  new ClassB();
           x.retornaClassC().retornaString();
        }
    }
 
}");

    SyntaxTree tree2 = CSharpSyntaxTree.ParseText(
@"using System;
 
namespace HelloWorld.Hello
{
    class ClassB{
        public ClassC retornaClassC(){
            return new ClassC();
        }
    }

    class ClassC{
        public string retornaString(){
            ClassB x = new ClassB();
            return ""str"";
        } 
    }
}");
    SyntaxTree[] trees = new SyntaxTree[2];
    trees[0] = tree;
    trees[1] = tree2;
        var compilation = CSharpCompilation.Create("Project")
                .AddReferences(MetadataReference.CreateFromFile(
                    typeof(object).GetTypeInfo().Assembly.Location))
                .AddSyntaxTrees(trees);
        var semanticModel = compilation.GetSemanticModel(tree);
        DepExtractor dep = new DepExtractor(trees);
        dep.start();
    }
}
