/*
<program> ::= {<expr>}
<expr> ::= <term> { (+|-) <term>}
<term> ::= <factor> {(*|/) <factor>}
<factor> ::= ID | NUM | ( <expr> ) 

ID = [A-Za-z_][A-Za-z_0-9]*
NUM = [0-9]+(?>\.[0-9]+)?
STRING = "(\\.|[^"])*"
*/

using System;
using System.Collections.Generic;
using  System.Text.RegularExpressions;

public class ExprParser
{
    public  class Parser {
        List<Node> tokens = new List<Node>();
        int cur = 0;
        
        public Parser(Node [] tokens) {
            this.tokens = new List<Node>(tokens);
            this.tokens.Add(new Node(Symbols.INVALID, ""));
        }

        public Node ParseProgram() {
            // Console.WriteLine($"{cur,10}: Program");
            //<program> ::= {<expr>}
            var children = new List<Node>();

            while (tokens[cur].Symbol != Symbols.INVALID){
                children.Add(ParseExpr());
                // Console.WriteLine("here");
            }
            
            return new Node(Symbols.Program, children.ToArray());
        }

        public Node ParseExpr() {
            // Console.WriteLine($"{cur,10}: Expr");

            //<expr> ::= <term> { (+|-) <term>}
            var children = new List<Node>();
            children.Add(ParseTerm());

            while (tokens[cur].Text == "+" || tokens[cur].Text == "-"){
                children.Add(tokens[cur++]);
                children.Add(ParseTerm());
            }

            return new Node(Symbols.Expr, children.ToArray());
        }

        public Node ParseTerm() {
            // <term> ::= <factor> {(*|/) <factor>}
            // Console.WriteLine($"{cur,10}: Term");

            var children = new List<Node>();
            children.Add(ParseFactor());

            while (tokens[cur].Text == "*" || tokens[cur].Text == "/"){
                children.Add(tokens[cur++]);
                children.Add(ParseFactor());
            }

            return new Node(Symbols.Term, children.ToArray());
        }

        public Node ParseFactor() {
            //<factor> ::= ID | NUM | ( <expr> ) 
            if (tokens[cur].Symbol == Symbols.ID || tokens[cur].Symbol == Symbols.NUM){
                return new Node(Symbols.Factor,  tokens[cur++]);
            } else {
                var lparen = ParseLiteral("(");
                var expr = ParseExpr();
                var rparen = ParseLiteral(")");
                return new Node(Symbols.Factor, lparen, expr, rparen);
            }
        }
    
        public Node ParseLiteral(string lit) {
            if (tokens[cur].Text == lit){
                return tokens[cur++];
            } else {
                throw new Exception("Syntax error");
            }
        }
    }

    public enum Symbols{
        INVALID,
        ID, 
        NUM, 
        STRING,
        LIT,

        Program, 
        Expr, 
        Term, 
        Factor 
    }
    public class Node
    {
        public Symbols Symbol;
        public string Text = "";

        List<Node> children = new List<Node>();

        public Node(Symbols symbol, string text){
            this.Symbol = symbol;
            this.Text = text;
        }

        public Node(Symbols symbol, params Node[] children){
            this.Symbol = symbol;
            this.Text = "";
            this.children = new List<Node>(children);
        }

        public void Print(string prefix = "")
        {
            Console.WriteLine($"{prefix}{Symbol.ToString().PadRight(42-prefix.Length)} {Text}");
            foreach (var child in children){
                child.Print(prefix+"  ");
            }
        }
    }


    static public List<Node> Tokenize(String src)
    {
        var result = new List<Node>();
        int pos = 0;
        Match m;

        // ID = [A-Za-z_][A-Za-z_0-9]*
        // NUM = [0-9]+(?>\.[0-9]+)?
        // STRING = "(\\.|[^"])*"

        var WS = new Regex(@"\G\s");
        var NUM = new Regex(@"\G[0-9]+(?>\.[0-9]+)?");
        var STRING = new Regex(@"\G""(\\.|[^""])*""");
        var ID = new Regex(@"\G[A-Za-z_][A-Za-z_0-9]*");
        var LIT = new Regex(@"\G[\+\-\*\/\(\)]");

        while (pos < src.Length) {
            // Console.WriteLine($"pos={pos} src[pos]={src[pos]}");
            if ((m = WS.Match(src, pos)).Success) {
                pos += m.Length;
            } else if ((m = NUM.Match(src, pos)).Success){
                result.Add(new Node(Symbols.NUM, m.Value));
                pos += m.Length;
            } else if ((m = STRING.Match(src, pos)).Success){
                result.Add(new Node(Symbols.STRING, m.Value));
                pos += m.Length;
            } else if ((m = ID.Match(src, pos)).Success){
                result.Add(new Node(Symbols.ID, m.Value));
                pos += m.Length;
            } else if ((m = LIT.Match(src, pos)).Success){
                result.Add(new Node(Symbols.LIT, m.Value));
                pos += m.Length;
            } else {
                throw new Exception("Lexer error");
            }
        }
        return result;
    }

    static public Node Parse(Node[] tokens)
    {
        var p = new Parser(tokens);
        var tree = p.ParseProgram();
        return tree;
    }

    static private void CheckString(string lispcode)
    {
        try
        {
            Console.WriteLine(new String('=', 50));
            Console.Write("Input: ");
            Console.WriteLine(lispcode);
            Console.WriteLine(new String('-', 50));

            Node[] tokens = Tokenize(lispcode).ToArray();

            Console.WriteLine("Tokens");
            Console.WriteLine(new String('-', 50));
            foreach (Node node in tokens)
            {
                Console.WriteLine($"{node.Symbol,-21}\t:{node.Text}");
            }
            Console.WriteLine(new String('-', 50));

            Node parseTree = Parse(tokens);

            Console.WriteLine("Parse Tree");
            Console.WriteLine(new String('-', 50));
            parseTree.Print();
            Console.WriteLine(new String('-', 50));
        }
        catch (Exception)
        {
            Console.WriteLine("Threw an exception on invalid input.");
        }
    }


    public static void Main(string[] args)
    {
        //Here are some strings to test on in 
        //your debugger. You should comment 
        //them out before submitting!

        CheckString(@"(foo + fizz*buzz) + 2");
        // CheckString(@"(define foo ""bananas"")");
        // CheckString(@"(define foo ""Say \""Chease!\"""")");
        // CheckString(@"(define foo ""Say \""Chease!\)");
        // CheckString(@"(+ 3 4)");      
        // CheckString(@"(+ 3.14 (* 4 7))");
        // CheckString(@"(+ 3.14 (* 4 7)");

        //CheckString(Console.In.ReadToEnd());
    }
}

