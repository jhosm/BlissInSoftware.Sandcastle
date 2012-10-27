Feature: US01 - Book Dedication
	As a potential customer
	I want to read the dedication
	So that I can inspire myself.

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

Scenario: Book's dedication
	When I open the dedication page for "Domain Driven Design"
	Then I read
		"""
		To Mom
		And Dad
		"""