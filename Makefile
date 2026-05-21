CXX := g++
SFML_CFLAGS := $(shell pkg-config --cflags sfml-graphics sfml-window sfml-system 2>/dev/null)
SFML_LIBS := $(shell pkg-config --libs sfml-graphics sfml-window sfml-system 2>/dev/null)

ifeq ($(strip $(SFML_LIBS)),)
SFML_LIBS := -L/usr/local/lib -lsfml-graphics-s -lsfml-window-s -lsfml-system-s
endif

CXXFLAGS := -std=c++17 -Wall -Wextra -Wpedantic -O2 -Iincludes $(SFML_CFLAGS)
LDFLAGS := $(SFML_LIBS)

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
