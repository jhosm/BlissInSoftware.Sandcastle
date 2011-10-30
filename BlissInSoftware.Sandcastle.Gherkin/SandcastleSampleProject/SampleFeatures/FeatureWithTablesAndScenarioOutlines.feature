Funcionalidade: US01 - Book Search
	As a potential customer
	I want to search for books by a simple string
	So that I can easily allocate books by something I remember from them.

Contexto:
	Dado the following books
		|Author			|Title								|Price	|
		|Martin Fowler	|Analysis Patterns					|50.20	|
		|Eric Evans		|Domain Driven Design				|46.34	|
		|Ted Pattison	|Inside Windows SharePoint Services	|31.49	|
		|Gojko Adzic	|Bridging the Communication Gap		|24.75	|

Esquema do Cenário: Simple search (scenario outline syntax)
	Quando I perform a simple search on '<search phrase>'
	Então the book list should exactly contain books <books>

	Exemplos:
		|search phrase			|books																	|
		|Domain					|'Domain Driven Design'													|
		|Windows Communication	|'Inside Windows SharePoint Services', 'Bridging the Communication Gap'	|