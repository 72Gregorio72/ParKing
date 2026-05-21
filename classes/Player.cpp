#include "lib.h"

Player::Player()
	: x(0),
	  y(0),
	  speed(1),
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

	y += gravity;

	checkCollision();
}

void Player::checkCollision() {
	if (!mapData.map || mapData.width == 0 || mapData.height == 0) {
		return;
	}

	sf::FloatRect playerBox(sf::Vector2f(static_cast<float>(x), static_cast<float>(y)), 
							sf::Vector2f(hitboxWidth, hitboxHeight));
	const float tileSize = 50.0f;

	for (int ty = 0; ty < mapData.height; ++ty) {
		for (int tx = 0; tx < mapData.width; ++tx) {
			if (mapData.map[ty][tx] == '1') {
				sf::FloatRect tileBox(sf::Vector2f(static_cast<float>(tx * tileSize), static_cast<float>(ty * tileSize)), 
									   sf::Vector2f(tileSize, tileSize));
				
				if (playerBox.position.x < tileBox.position.x + tileBox.size.x &&
					playerBox.position.x + playerBox.size.x > tileBox.position.x &&
					playerBox.position.y < tileBox.position.y + tileBox.size.y &&
					playerBox.position.y + playerBox.size.y > tileBox.position.y) {
					
					if (playerBox.position.y + playerBox.size.y > tileBox.position.y &&
						playerBox.position.y + playerBox.size.y < tileBox.position.y + tileSize / 2) {
						y = static_cast<int>(tileBox.position.y - static_cast<int>(hitboxHeight));
					}
				}
			}
		}
	}
}