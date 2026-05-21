#include "lib.h"

int main() {
    const unsigned int windowWidth = 1280;
    const unsigned int windowHeight = 720;

	Player player = Player();

    sf::RenderWindow window(
        sf::VideoMode({windowWidth, windowHeight}),
        "ParKing - SFML Platformer Starter",
        sf::Style::Close
    );
    window.setVerticalSyncEnabled(true);

	data mapData = parseMap("levels/1.txt");

	player.setMapData(mapData);

    while (window.isOpen()) {
        while (const auto event = window.pollEvent()) {
            if (event->is<sf::Event::Closed>()) {
                window.close();
            }
        }

        window.clear(sf::Color::Black);
		player.playerLoop(window);
		drawMap(window, mapData);
		player.draw(window);
        window.display();
    }

    return 0;
}
