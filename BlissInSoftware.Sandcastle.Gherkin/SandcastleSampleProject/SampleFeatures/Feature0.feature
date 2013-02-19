# language: pt
@HU_123
Funcionalidade: Adição
  Para evitar erros bobos
  Como um péssimo matemático
  Eu quero saber como somar dois números

Olha o *bold*.

Olha o **bold**.

Olha o _itálico_.

Olha a lista:

*   Red
*   Green
*   Blue

Outra lista:

*   Lorem ipsum dolor sit amet, consectetuer adipiscing elit.
    Aliquam hendrerit mi posuere lectus. Vestibulum enim wisi,
    viverra nec, fringilla in, laoreet vitae, risus.
*   Donec sit amet nisl. Aliquam semper ipsum sit amet velit.
    Suspendisse id sem consectetuer libero luctus adipiscing.

Mais uma lista:

*   Lorem ipsum dolor sit amet, consectetuer adipiscing elit.
Aliquam hendrerit mi posuere lectus. Vestibulum enim wisi,
viverra nec, fringilla in, laoreet vitae, risus.
*   Donec sit amet nisl. Aliquam semper ipsum sit amet velit.
Suspendisse id sem consectetuer libero luctus adipiscing

Mais uma lista ordenada:

1.  This is a list item with two paragraphs. Lorem ipsum dolor
    sit amet, consectetuer adipiscing elit. Aliquam hendrerit
    mi posuere lectus.

    Vestibulum enim wisi, viverra nec, fringilla in, laoreet
    vitae, risus. Donec sit amet nisl. Aliquam semper ipsum
    sit amet velit.

2.  Suspendisse id sem consectetuer libero luctus adipiscing.

Uma lista identada:

*   Red
	1. yadayada
*   Green
*   Blue	
	
Esta é uma grande descrição.

![Figure 2](Lighthouse "Caption after with lead-in 2")

<mediaLink>
<caption placement="after" lead="Figure 1">Caption after with lead-in</caption>
<image placement="center" xlink:href="Lighthouse"/>
</mediaLink>

Segundo parágrafo da descrição.

Terceiro parágrafo da descrição.
  
Cenário: Adicionar dois números
    Dado que eu digitei 50 na calculadora
    E que eu digitei 70 na calculadora
    Quando eu aperto o botão de soma
    Então o resultado na calculadora deve ser 120
	
Cenário: Adicionar dois números1
    Dado que eu digitei 50 na calculadora
    E que eu digitei 70 na calculadora
    Quando eu aperto o botão de soma
    Então o resultado na calculadora deve ser 120	
	
Cenário: Um teste
    Dado que eu digitei 50 na calculadora
    E que eu digitei 70 na calculadora
    Quando eu aperto o botão de soma
    Então o écran deve-me apresentar
		"""
		50
		+
		70
		=
		120
		"""