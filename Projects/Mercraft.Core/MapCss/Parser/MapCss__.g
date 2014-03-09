lexer grammar MapCss;
options {
  language=CSharp3;

}

@members {
    /// true, if the scanner is in a state, where OSM tag names are 
    /// valid identifiers  '[highway=residential]'
	bool isOsmTagAllowed = false;	
	/// true, if the lexer is currently in a declaration block
	bool isInDeclarationBlock = false;
}

OP_AND : '&&' ;
OP_CONTAINS : '~=' ;
OP_ENDS_WITH : '$=' ;
OP_EQ : '=' ;
OP_GE : '>=' ;
OP_GT : '>' ;
OP_LE : '<=' ;
OP_LT : '<' ;
OP_MATCH : '=~' ;
OP_MOD : '%' ;
OP_MUL : '*' ;
OP_NEQ : '!=' ;
OP_OR : '||' ;
OP_PLUS : '+' ;
OP_STARTS_WITH : '^=' ;
OP_SUBSTRING : '*=' ;
T__114 : '!' ;
T__115 : '!.' ;
T__116 : '!:' ;
T__117 : '(' ;
T__118 : ')' ;
T__119 : ',' ;
T__120 : '-' ;
T__121 : '.' ;
T__122 : ':!' ;
T__123 : '::' ;
T__124 : '?' ;

// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 412
fragment EBACKSLASH: '\\\\';// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 413
fragment UNICODE: '\u0080'..'\uFFFF';// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 416
RGB: ('r' | 'R') ('g' | 'G') ('b' | 'B');// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 417
RGBA: ('r' | 'R') ('g' | 'G') ('b' | 'B') ('a' | 'A');// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 418
ROLE: ('r' | 'R') ('o' | 'O') ('l' | 'L') ('e' | 'E');// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 419
INDEX: ('i' | 'I') ('n' | 'N') ('d' | 'D') ('e' | 'E') ('x' | 'X');// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 420
EVAL: ('e' | 'E') ('v' | 'V') ('a' | 'A') ('l' | 'L');// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 421
LIST: ('l' | 'L') ('i' | 'I') ('s' | 'S') ('t' | 'T');// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 422
IMPORT: '@' ('i' | 'I') ('m' | 'M') ('p' | 'P') ('o' | 'O')('r' | 'R') ('t' | 'T');// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 424
fragment HWS: (' ' | '\t' | '\f');// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 425
fragment URLCONTENT: ('!' | '#' | '$' | '%' | '&' | '*'..'[' | ']'..'~' | NONASCII)+;// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 426
URL: ('u' | 'U') ('r' | 'R') ('l' | 'L') HWS* '(' HWS* 
     (
	         URLCONTENT
	   | '"' URLCONTENT '"'  
	   | '\'' URLCONTENT '\''
     )
     HWS* ')';// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 434
fragment DIGIT:  '0'..'9';// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 435
fragment CHAR: 'a'..'z' | 'A'..'Z';// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 439
fragment NONASCII: ~('\u0000' .. '\u009F');// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 440
fragment NMSTART: 'a'..'z' | 'A'..'Z' | '_' | NONASCII;// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 441
fragment NMCHAR: 'a'..'z' | 'A'..'Z' | '_' | '-' | NONASCII;// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 444
fragment NCOMPONENT: (CHAR | '_') (CHAR | DIGIT | '_' | '-')*;// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 445
fragment TAGSEPARATOR: (':') | ('.');// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 448
fragment CSS_IDENT:;// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 449
fragment OSM_TAG:;// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 450
IDENTS:
	'-' ?  NCOMPONENT (
	      {isOsmTagAllowed}? =>  (
	          (TAGSEPARATOR) =>  TAGSEPARATOR NCOMPONENT (TAGSEPARATOR NCOMPONENT)*  {$type=OSM_TAG;}
	        | {$type=CSS_IDENT;}	                    
	      )
	   |  {$type=CSS_IDENT;}
	);// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 462
LBRACKET 
  @after{isOsmTagAllowed=true;}
  : '[';// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 466
RBRACKET
  @after{isOsmTagAllowed=false;}
  : ']';// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 476
LBRACE
  @after{
    isInDeclarationBlock=true;
    isOsmTagAllowed=false;
  }
  : '{';// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 483
RBRACE
  @after{
    isInDeclarationBlock=false;
    isOsmTagAllowed=false;
  }
  : '}';// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 490
COLON
  @after{
    isOsmTagAllowed=isInDeclarationBlock;
  }
  : ':';// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 496
SEMICOLON
  @after{
    isOsmTagAllowed=false;
  }
  : ';';// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 503
fragment EDQUOTE: '\\"';// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 504
fragment ESQUOTE: '\\\'';// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 505
DQUOTED_STRING: '"' (' ' | '!' | '#'..'[' | ']'..'~' | UNICODE | EDQUOTE | EBACKSLASH )* '"';// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 506
SQUOTED_STRING: '\'' (' '..'&' | '('..'[' | ']'..'~' | UNICODE | ESQUOTE | EBACKSLASH)* '\'';// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 509
fragment HEXDIGIT: DIGIT | 'a'..'f' | 'A'..'F';// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 510
HEXCOLOR: '#' ((HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT) | (HEXDIGIT HEXDIGIT HEXDIGIT));// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 516
fragment PERCENTAGE:;// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 517
fragment PIXELS:;// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 518
fragment POINTS:;// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 519
fragment POSITIVE_FLOAT:;// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 520
fragment POSITIVE_INT:;// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 521
fragment NEGATIVE_FLOAT:;// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 522
fragment NEGATIVE_INT:;// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 523
fragment INCREMENT:;// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 524
fragment P: ('p' | 'P');// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 525
fragment T: ('t' | 'T');// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 526
fragment X: ('x' | 'X');// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 529
NUMBER
	: ('-'? DIGIT* ('.' DIGIT+)?) => s='-'? DIGIT* (d='.' DIGIT+)?
	    (
		   (P (T | X)) => 
			  P
			  (
				   T              {$type = POINTS;}
			     | X              {$type = PIXELS;}
		  	)
	      | ('%') => '%'          {$type = PERCENTAGE;}	    
	      | 
	        {
	           if ($s == null) {
	              $type = ($d == null ? POSITIVE_INT : POSITIVE_FLOAT);
	           } else {
	              $type = ($d == null ? NEGATIVE_INT : NEGATIVE_FLOAT);
	           }
   	        }
	  )	 	
	| ('+') => '+' DIGIT+         {$type = INCREMENT;}
	;// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 554
RANGE
	: '|z' (
		  '-' DIGIT+
		| DIGIT+ 
		| DIGIT+ '-' 
		| DIGIT+ '-' DIGIT+
	  )
	;// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 566
fragment REGEX_ESCAPE:   '\\\\' | '\\/' | '\\(' | '\\)' 
                       | '\\|' | '\\$' | '\\*' | '\\.' | '\\^' | '\\?' | '\\+' | '\\-'
                       | '\\n' | '\\r' | '\\t'
                       | '\\s' | '\\S'
                       | '\\d' | '\\D'
                       | '\\w' | '\\W';// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 572
fragment REGEX_START:  ' '..')' | '+'..'.' |'0'..'[' | ']'..'~' | UNICODE | REGEX_ESCAPE;// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 573
fragment REGEX_CHAR:  ' '..'.' |'0'..'[' | ']'..'~' | UNICODE | REGEX_ESCAPE;// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 580
fragment DIV:;// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 581
REGEXP:  '/'  (
              (REGEX_START REGEX_CHAR* '/') 
                    => REGEX_START REGEX_CHAR* '/'   {$type=REGEXP;}
           |  (.)   =>                               {$type=DIV;}
         );// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 590
WS:		    (' ' | '\t' | '\n' | '\r' | '\f') {$channel=Hidden;};// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 591
SL_COMMENT:   '//' (options {greedy=false;}: .)* '\r'? '\n' {$channel=Hidden;};// $ANTLR src "c:\Users\Ilya.Builuk\Documents\Source\MapCssEngine\MapCssEngine\Parser\MapCss.g" 592
ML_COMMENT:   '/*'  (options {greedy=false;} : .)* '*/' {$channel=Hidden;};