using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
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

    static void Main(string[] args){
        string path;
        if(args.Length > 0){
            path = args[0];
        }else{
            path = Directory.GetCurrentDirectory();
            Console.WriteLine("No path given, using {0}", path);
        }
        Console.WriteLine("Extracting cs files from {0}", path);
        List<string> csFiles = ExtractCSFiles(path);
        HashSet<Dependency> dependencies = DepExtractor.getInstance().extract(csFiles);
        foreach(var dep in dependencies){
            Console.WriteLine(dep);
        }
        Console.WriteLine("Done");
    }
    
}
