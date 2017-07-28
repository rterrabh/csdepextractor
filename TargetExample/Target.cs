using System;

namespace Target{
    /*
    abstract class ClassA{
        public string test(){
            try{
                string s = null;
                Console.WriteLine(s.Length);
            }catch(ArgumentNullException ex){
                Console.WriteLine(ex.Message);
                throw new ArgumentNullException();
            }
            return "str";
        }

        abstract public int sugar();
    }

    interface Moveable{
        void move(int direction);
    }
    class ClassB : ClassA, Moveable{
        private string a;

        public void move(int direction){

        }

        public override int sugar(){
            return 5;
        }
    }
    class ClassC : ClassB{
        
    }
     */
    class Test{
        private string name;
        public string Name {get{return name;}}
        
        public string test(){
            try{
                Console.WriteLine("bla");
            }catch(Exception ex){
                Console.WriteLine(ex.Message);
            }
            return "bla";
        }

    }

    class TestB{
        public void foo(string a){

        }
        public bool test(){
            var t = new Test();
            //foo(t.test());
            return t.Name == "dads";
        }
    }
}