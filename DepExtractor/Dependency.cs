namespace Dependencies{
    class Dependency{
        public const string ACCESS = "access";
        public const string CREATE = "create";
        public const string DECLARE = "declare";
        public const string EXTEND = "extend";
        public const string IMPLEMENT = "implement";
        public const string USE_ANNOTATION = "useannotation";
        public const string THROW = "throw";

        private string origin;
        private string type;
        private string destin;

        public string Origin {get{return origin;}}
		public string Type{get{return type;}}
        public string Destin{get{return destin;}}

        public Dependency(string origin, string type, string destin){
            this.origin = origin;
            this.type = type;
            this.destin = destin;
        }

        public override int GetHashCode(){
            return type.Length;
        }

        public override bool Equals(object obj){
            if(obj is Dependency){
                Dependency dep = (Dependency) obj;
                return this.origin == dep.origin && this.type == dep.type
                    && this.destin == dep.destin;
            }
            return false;
        }
    }
}