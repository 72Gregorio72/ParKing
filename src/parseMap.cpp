#include "lib.h"

data parseMap(const std::string& filename) {
	data result = {nullptr, 0, 0};
	std::ifstream file(filename);
	if (!file.is_open()) {
		std::cerr << "Error opening file: " << filename << std::endl;
		return result;
	}

	int width = 0, height = 0;
	std::string line;
	std::vector<std::string> mapGrid;

	while (std::getline(file, line)) {
		if (line.empty()) continue;

		if (line.find("x:") != std::string::npos) {
			std::istringstream iss(line);
			std::string key;
			iss >> key >> width;
		} else if (line.find("y:") != std::string::npos) {
			std::istringstream iss(line);
			std::string key;
			iss >> key >> height;
		} else if (line[0] == '0' || line[0] == '1') {
			mapGrid.push_back(line);
		}
	}

	if (width > 0 && height > 0 && mapGrid.size() == static_cast<size_t>(height)) {
		std::cout << "Map loaded: " << width << "x" << height << std::endl;
		
		// Allocate map
		result.map = new char*[height];
		for (int i = 0; i < height; ++i) {
			result.map[i] = new char[width + 1];
			strncpy(result.map[i], mapGrid[i].c_str(), width);
			result.map[i][width] = '\0';
		}
		result.width = width;
		result.height = height;
	} else {
		std::cerr << "Invalid map format" << std::endl;
	}
	
	return result;
}

void drawMap(sf::RenderWindow& window, const data& mapData) {
	if (!mapData.map || mapData.width == 0 || mapData.height == 0) {
		return;
	}
	
	const float tileSize = 50.0f;
	sf::RectangleShape tile({tileSize, tileSize});
	
	for (int y = 0; y < mapData.height; ++y) {
		for (int x = 0; x < mapData.width; ++x) {
			char cell = mapData.map[y][x];
			
			if (cell == '0') {
				tile.setFillColor(sf::Color::White);
			} else if (cell == '1') {
				tile.setFillColor(sf::Color::Blue);
			} else {
				continue;
			}
			
			tile.setPosition(sf::Vector2f(static_cast<float>(x * tileSize), static_cast<float>(y * tileSize)));
			window.draw(tile);
		}
	}
}