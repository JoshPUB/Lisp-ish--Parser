# Lisp-ish--Parser
## For now it is reccomended you view the RAW form of this file.
Another Project from CSE465 at Miami. Implemented a parser Lisp(ish) in CShapr.
lispish.exe , a parser for a simple LISP-like language. 
Reads a LISP expression and prints the parse tree.


Grammar for a simplified scheme grammar:
<br>
<Program> ::= {<SExpr>}
<br>
<SExpr> ::= <Atom> | <List>
<br>
<List> ::= () | ( <Seq> )
<br>
<Seq> ::= <SExpr> <Seq> | <SExpr>
<br>
<Atom> ::= ID | INT | REAL | STRING
<br>


The token types are described by the following regular expressions (use regex101.com to explore them):
<br>
LITERAL = [\(\)]
<br>
REAL = [+-]?[0-9]*\.[0-9]+
<br>
INT = [+-]?[0-9]+
<br>
STRING = "(?>\\.|[^\\"])*".  Multiline strings are not allowed.
<br>
ID = [^\s"\(\)]+
<br>
Anything else other than whitespace is an error ( INVALID)
