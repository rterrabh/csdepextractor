using System;
using System.IO;
using System.Collections.Generic;
namespace Dependencies{

    class IO{

        public static void SaveDependencies(ICollection<Dependency> dependencies, string path){
            using(var stream = new StreamWriter(File.Create(path))){
                foreach(var dep in dependencies){
                    stream.WriteLine(dep.ToString());
                }
            }
        }
    }
}
