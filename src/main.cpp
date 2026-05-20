#include "lib.h"

int main() {
    const unsigned int windowWidth = 1280;
    const unsigned int windowHeight = 720;

	Player player = Player();
	player.setX(static_cast<int>(windowWidth / 2));
	player.setY(static_cast<int>(windowHeight / 2));

    sf::RenderWindow window(
        sf::VideoMode(windowWidth, windowHeight),
        "ParKing - SFML Platformer Starter",
        sf::Style::Close
    );
    window.setVerticalSyncEnabled(true);

    while (window.isOpen()) {
        sf::Event event;
        while (window.pollEvent(event)) {
            if (event.type == sf::Event::Closed) {
                window.close();
            }
        }

        window.clear(sf::Color::Black);
		player.playerLoop(event, window);
		player.draw(window);
        window.display();
    }

    return 0;
}
