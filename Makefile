CXX := g++
CXXFLAGS := -std=c++17 -Wall -Wextra -Wpedantic -O2 -Iincludes
LDFLAGS := -lsfml-graphics -lsfml-window -lsfml-system

SRC_DIR := src
CLASS_DIR := classes
BUILD_DIR := build
TARGET := $(BUILD_DIR)/parking_platformer
SOURCES := $(wildcard $(SRC_DIR)/*.cpp) $(wildcard $(CLASS_DIR)/*.cpp)
OBJECTS := $(patsubst %.cpp,$(BUILD_DIR)/%.o,$(SOURCES))
INCLUDES := includes/lib.h

.PHONY: all run clean

all: $(TARGET)

$(BUILD_DIR):
	mkdir -p $(BUILD_DIR)

$(TARGET): $(BUILD_DIR) $(OBJECTS)
	$(CXX) $(OBJECTS) -o $@ $(LDFLAGS)

$(BUILD_DIR)/%.o: %.cpp | $(BUILD_DIR)
	mkdir -p $(dir $@)
	$(CXX) $(CXXFLAGS) -c $< -o $@

clean:
	rm -rf $(BUILD_DIR)

re: clean all

test: all
	./$(TARGET) includes/lib.h
