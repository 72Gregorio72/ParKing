#include "lib.h"

Player::Player()
	: x(0),
	  y(0),
	  speed(5),
	  maxHP(100),
	  currentHP(maxHP) {
}

void Player::draw(sf::RenderWindow& window) {
	sf::RectangleShape shape({50.0f, 50.0f});
	shape.setFillColor(sf::Color::Red);
	shape.setPosition(static_cast<float>(x), static_cast<float>(y));
	window.draw(shape);
}

void Player::getInput(const sf::Event& event, sf::RenderWindow& window) {
	// printf("Event code: %d\n", event.key.code);
	if (event.type == 4) {
		if (event.key.code == 119) {
			printf("W pressed\n");
		}
		if (event.key.code == 115) {
			printf("S pressed\n");
		}
		if (event.key.code == 97) {
			x--;
		}
		if (event.key.code == 100) {
			x++;
		}
	}
}

void Player::playerLoop(const sf::Event& event, sf::RenderWindow& window) {
    getInput(event, window);
}