using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Dependencies;
class DepCSharp
{

    static List<string> ExtractCSFiles(string sDir){
        List<string> csFiles = new List<string>();
        try{
            foreach (string f in Directory.GetFiles(sDir)){
                if(Path.GetExtension(f) == ".cs"){
                    csFiles.Add(f);
                }
            }
            foreach (string d in Directory.GetDirectories(sDir)){
                csFiles = csFiles.Concat(ExtractCSFiles(d)).ToList();
            }
        }catch (System.Exception excpt){
            Console.WriteLine(excpt.Message);
        }
        return csFiles;
    }

    static void Main(string[] args)
    {

        string path = "/home/elderjr/Documents/git_repositories/msdclcheck/toyexample/MsAuthenticate";
        Console.WriteLine("Extracting cs files from {0}", path);
        List<string> csFiles = ExtractCSFiles(path);
        List<SyntaxTree> trees = new List<SyntaxTree>();
        Console.WriteLine("Extracting trees of cs files");
        foreach(var f in csFiles){
            trees.Add(CSharpSyntaxTree.ParseText(File.ReadAllText(f)));
        }
        Console.WriteLine("Extracting dependencies");
        HashSet<Dependency> dependencies = DepExtractor.getInstance().start(trees);
    }
    
}
