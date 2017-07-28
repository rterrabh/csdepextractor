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
        string path = null, dependenciesPath;
        bool showDependencies = false, save = true;
        if(args.Length == 1 && args[0] == "-help"){
            Console.WriteLine("----- CCharp Dependencies Extractor ----");
            Console.WriteLine("   --path: indicates the path to analyse (default is the current path)");
            Console.WriteLine("   --not_save: indicates for not save dependencies found");
            Console.WriteLine("   --show_dependencies: show dependencies found in terminal");
            Console.WriteLine("   -help: show help");
        }else{
            for(int i = 0; i < args.Length; i++){
                if(args[i] == "--path" && (i+1) < args.Length){
                    path = args[i + 1];
                }else if(args[i] == "--show_dependencies"){
                    showDependencies = true;
                }else if(args[i] == "--not_save"){
                    save = false;
                }
            }
            if(path == null){
                path = Directory.GetCurrentDirectory();
                Console.WriteLine("No path given, using {0}", path);
            }
            dependenciesPath = path + "/dependencies.txt";
            Console.WriteLine("Extracting cs files from {0}", path);
            List<string> csFiles = ExtractCSFiles(path);
            HashSet<Dependency> dependencies = DepExtractor.getInstance().extract(csFiles);
            if(showDependencies){
                foreach(var dep in dependencies){
                    Console.WriteLine(dep);
                }
            }
            if(save){
                Console.WriteLine("Saving dependencies.txt in {0}", dependenciesPath);
                Dependencies.IO.SaveDependencies(dependencies, dependenciesPath);
            }
                Console.WriteLine("Done");
        }
    }
}