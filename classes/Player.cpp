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
	shape.setPosition(sf::Vector2f(static_cast<float>(x), static_cast<float>(y)));
	window.draw(shape);
}

void Player::getInput(sf::RenderWindow& window) {
	(void)window; // Suppress unused parameter warning
	if (sf::Keyboard::isKeyPressed(sf::Keyboard::Key::W)) {
		printf("W pressed\n");
	}
	if (sf::Keyboard::isKeyPressed(sf::Keyboard::Key::S)) {
		printf("S pressed\n");
	}
	if (sf::Keyboard::isKeyPressed(sf::Keyboard::Key::A)) {
		x -= speed;
	}
	if (sf::Keyboard::isKeyPressed(sf::Keyboard::Key::D)) {
		x += speed;
	}
}

void Player::playerLoop(sf::RenderWindow& window) {
    getInput(window);
}