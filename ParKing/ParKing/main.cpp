#include <iostream>
#include <SFML\Graphics.hpp>

int main()
{
	unsigned int windowWidth = 1600;
	unsigned int windowHeight = 900;

	sf::RenderWindow window(sf::VideoMode({ windowWidth, windowHeight }), "ParKing");
	window.setFramerateLimit(60);

	while (window.isOpen())
	{
		while (auto event = window.pollEvent())
		{
			if (event->is <sf::Event::Closed>())
			{
				window.close();
			}
		}
	}

	return 0;
}