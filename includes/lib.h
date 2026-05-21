#pragma once

//normal libs
#include <SFML/Graphics.hpp>
#include <iostream>
#include <cstdio>
#include <fstream>
#include <sstream>
#include <vector>
#include <string.h>
#include <cstring>

//data structures
typedef struct data {
	char **map;
	int width;
	int height;
} data;

//classes
#include "../classes/Player.hpp"

//map parsing
data parseMap(const std::string& filename);
void drawMap(sf::RenderWindow& window, const data& mapData);

//colors
#define RED "\033[31m"
#define RESET "\033[0m"
#define GREEN "\033[32m"
#define YELLOW "\033[33m"
#define BLUE "\033[34m"