#include <SFML/Graphics.hpp>

#include <algorithm>

int main() {
    const unsigned int windowWidth = 1280;
    const unsigned int windowHeight = 720;

    sf::RenderWindow window(
        sf::VideoMode(windowWidth, windowHeight),
        "ParKing - SFML Platformer Starter",
        sf::Style::Close
    );
    window.setVerticalSyncEnabled(true);

    const float playerWidth = 40.0f;
    const float playerHeight = 60.0f;
    const float moveSpeed = 320.0f;
    const float jumpVelocity = -620.0f;
    const float gravity = 1800.0f;

    sf::RectangleShape player({playerWidth, playerHeight});
    player.setFillColor(sf::Color(40, 180, 99));
    player.setPosition(120.0f, 120.0f);

    sf::RectangleShape ground({windowWidth * 1.0f, 100.0f});
    ground.setFillColor(sf::Color(52, 73, 94));
    ground.setPosition(0.0f, windowHeight - 100.0f);

    sf::RectangleShape platformA({240.0f, 24.0f});
    platformA.setFillColor(sf::Color(127, 140, 141));
    platformA.setPosition(260.0f, 500.0f);

    sf::RectangleShape platformB({220.0f, 24.0f});
    platformB.setFillColor(sf::Color(127, 140, 141));
    platformB.setPosition(640.0f, 390.0f);

    sf::Vector2f velocity(0.0f, 0.0f);
    bool onGround = false;

    sf::Clock clock;

    while (window.isOpen()) {
        sf::Event event;
        while (window.pollEvent(event)) {
            if (event.type == sf::Event::Closed) {
                window.close();
            }
        }

        const float dt = clock.restart().asSeconds();

        velocity.x = 0.0f;
        if (sf::Keyboard::isKeyPressed(sf::Keyboard::A) || sf::Keyboard::isKeyPressed(sf::Keyboard::Left)) {
            velocity.x = -moveSpeed;
        }
        if (sf::Keyboard::isKeyPressed(sf::Keyboard::D) || sf::Keyboard::isKeyPressed(sf::Keyboard::Right)) {
            velocity.x = moveSpeed;
        }

        if ((sf::Keyboard::isKeyPressed(sf::Keyboard::Space) || sf::Keyboard::isKeyPressed(sf::Keyboard::Up)) && onGround) {
            velocity.y = jumpVelocity;
            onGround = false;
        }

        velocity.y += gravity * dt;

        player.move(velocity.x * dt, 0.0f);
        player.move(0.0f, velocity.y * dt);

        onGround = false;

        auto resolveLanding = [&](const sf::RectangleShape& platform) {
            const sf::FloatRect playerBounds = player.getGlobalBounds();
            const sf::FloatRect platformBounds = platform.getGlobalBounds();

            if (playerBounds.intersects(platformBounds) && velocity.y >= 0.0f) {
                const float playerBottom = playerBounds.top + playerBounds.height;
                const float platformTop = platformBounds.top;

                if (playerBottom - velocity.y * dt <= platformTop + 8.0f) {
                    player.setPosition(player.getPosition().x, platformTop - playerBounds.height);
                    velocity.y = 0.0f;
                    onGround = true;
                }
            }
        };

        resolveLanding(ground);
        resolveLanding(platformA);
        resolveLanding(platformB);

        const float minX = 0.0f;
        const float maxX = windowWidth - playerWidth;
        player.setPosition(std::clamp(player.getPosition().x, minX, maxX), player.getPosition().y);

        if (player.getPosition().y > windowHeight + 300.0f) {
            player.setPosition(120.0f, 120.0f);
            velocity = {0.0f, 0.0f};
        }

        window.clear(sf::Color(236, 240, 241));
        window.draw(ground);
        window.draw(platformA);
        window.draw(platformB);
        window.draw(player);
        window.display();
    }

    return 0;
}
