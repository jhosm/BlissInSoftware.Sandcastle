@US335
Funcionalidade: Controlar acesso à listagem do FE de Validação com operação GAS própria
	Como Validador
	Consigo aceder à listagem do FE de Validação, dado possuir acesso a operação GAS adequada,
	De modo a poder consultar todos os processos de um dado conjunto de Processos de Negócio.
	
Cenário: Validador tem acesso à listagem
	Dado um utilizador com a operação GAS "keyPapiro_Listagem_BO"
	Quando acede à listagem do FE de Validação
	Então tem acesso concedido.

@Automated
Cenário: Validador não tem acesso à listagem
	Dado um utilizador sem a operação GAS "keyPapiro_Listagem_BO"
	Quando acede à listagem do FE de Validação
	Então tem acesso negado.