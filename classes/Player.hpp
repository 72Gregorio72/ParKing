#pragma once

#include "lib.h"
#include <SFML/Graphics.hpp>

class Player {
	public:
		Player();
		void draw(sf::RenderWindow& window);
		void getInput(const sf::Event& event, sf::RenderWindow& window);
		void playerLoop(const sf::Event& event, sf::RenderWindow& window);

		int getX() const { return x; }
		int getY() const { return y; }
		int getSpeed() const { return speed; }
		int getMaxHP() const { return maxHP; }
		int getCurrentHP() const { return currentHP; }

		void setX(int nx) { x = nx; }
		void setY(int ny) { y = ny; }
		void setSpeed(int ns) { speed = ns; }
		void setMaxHP(int mh) { maxHP = mh; }
		void setCurrentHP(int ch) { currentHP = ch; }

	private:
		int x;
		int y;
		int speed;
		int maxHP;
		int currentHP;

		// simple physics
		float vy;
		float gravity;
		float jumpVelocity;
		int width;
		int height;
		bool onGround;
};
